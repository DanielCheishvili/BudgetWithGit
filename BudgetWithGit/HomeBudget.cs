using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================


namespace Budget
{
    // ====================================================================
    // CLASS: HomeBudget
    //        - Combines categories Class and expenses Class
    //        - One File defines Category and Budget File
    //        - etc
    // ====================================================================

    /// <summary>
    /// Combines categories class and expenses class. The file verfication will be used from 
    /// the BudgetFile.cs class.
    /// <see href="Budget.Categories.html"/>
    /// <see href="Budget.Expenses.html"/>
    /// </summary>
    /// <example>
    /// <code>
    ///        HomeBudget homeBudget = new HomeBudget(); //No budget file, using default constructor
    ///        BudgetItemsByCategory budgetCategory = new BudgetItemsByCategory();
    ///
    ///        homeBudget.categories.Add("Description", Category.CategoryType.Credit);
    ///
    ///        homeBudget.GetBudgetItems(null, null, true, 10); 
    ///        homeBudget.GetBudgetItemsByMonth(new DateTime(2020, 1, 1), new DateTime(2021, 1, 1), true, 11);
    ///        homeBudget.GeBudgetItemsByCategory(null, null, true, 12);
    ///        homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);
    ///
    ///        homeBudget.categories.Delete(11);
    ///        homeBudget.SaveToFile("FilePath");
    /// </code>
    /// </example>
    /// 

    public class HomeBudget
    {
        private string _FileName;
        private string _DirName;
        private Categories _categories;
        private Expenses _expenses;

        // ====================================================================
        // Properties
        // ===================================================================

        // Properties (location of files etc)

        /// <summary>
        /// Gets the filename.
        /// </summary>
        public String FileName { get { return _FileName; } }

        /// <summary>
        /// Gets the directory name.
        /// </summary>
        public String DirName { get { return _DirName; } }

        /// <summary>
        /// The public propery that returns the full path if a valid file name and directory name specified.
        /// Otherwise it returns a null file path which will use the default file path.
        /// </summary>
        public String PathName
        {
            get
            {
                if (_FileName != null && _DirName != null)
                {
                    return Path.GetFullPath(_DirName + "\\" + _FileName);
                }
                else
                {
                    return null;
                }
            }
        }

        // Properties (categories and expenses object)

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets the expenses.
        /// </summary>
        public Expenses expenses { get { return _expenses; } }

        /// <summary>
        /// constructor with default categories with no expenses.
        /// </summary>
       /* public HomeBudget()
        {
            _categories = new Categories();
            _expenses = new Expenses();
        }*/
        public HomeBudget(String databaseFile,bool newDB = false)
        {
            if (!newDB && File.Exists(databaseFile))
            {
                Database.openExistingDatabase(databaseFile);
            }
            else
            {
                Database.newDatabase(databaseFile);
                newDB = true;
            }

            _categories = new Categories(Database.dbConnection, newDB);
            _expenses = new Expenses(Database.dbConnection);
            // read the expenses from the xml file
            
        }

        /// <summary>
        /// Constructor with an existing budget.
        /// </summary>
        /// 
        /// <param name="budgetFileName">The file name of the budget items.</param>
       

        #region GetList

        /// <summary>
        /// Gets all expenses list. It joins the categories list with 
        /// the expenses list. It shows the budget in a negative format since paying expenses
        /// will deduct money.
        /// </summary>
        /// 
        /// <param name="Start">The start date</param>
        /// <param name="End">The end date/ due date</param>
        /// <param name="FilterFlag">The unwanted item filter.</param>
        /// <param name="CategoryID">The category id of budget item</param>
        ///
        /// <returns>The list of items</returns>
        public List<BudgetItem> GetBudgetItems(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // ------------------------------------------------------------------------
            // return joined list within time frame
            // ------------------------------------------------------------------------
            Start = Start ?? new DateTime(1900, 1, 1);
            End = End ?? new DateTime(2500, 1, 1);

            var query =  from c in _categories.List()
                        join e in _expenses.List() on c.Id equals e.Category
                        where e.Date >= Start && e.Date <= End
                        select new { CatId = c.Id, ExpId = e.Id, e.Date, Category = c.Description, e.Description, e.Amount };

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;

            foreach (var e in query.OrderBy(q => q.Date))
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != e.CatId)
                {
                    continue;
                }

                // keep track of running totals
                total = total + e.Amount;
                items.Add(new BudgetItem
                {
                    CategoryID = e.CatId,
                    ExpenseID = e.ExpId,
                    ShortDescription = e.Description,
                    Date = e.Date,
                    Amount = e.Amount,
                    Category = e.Category,
                    Balance = total
                });
            }

            return items;
        }

        /// <summary>
        /// Group all expenses month by month. Groups by month and year and creates a new list to store it in.
        /// Calculates the total of the month and creates a list of details. Adds it to the created list.
        /// </summary>
        /// 
        /// <param name="Start">The start date </param>
        /// <param name="End">The due date</param>
        /// <param name="FilterFlag">The unwanted item filter.</param>
        /// <param name="CategoryID">The category id of the budget items</param>
        /// 
        /// <returns>A list of budget items </returns>
        public List<BudgetItemsByMonth> GetBudgetItemsByMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<BudgetItem> items = GetBudgetItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by year/month
            // -----------------------------------------------------------------------
            var GroupedByMonth = items.GroupBy(c => c.Date.Year.ToString("D4") + "-" + c.Date.Month.ToString("D2"));

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByMonth>();
            foreach (var MonthGroup in GroupedByMonth)
            {
                // calculate total for this month, and create list of details
                double total = 0;
                var details = new List<BudgetItem>();
                foreach (var item in MonthGroup)
                {
                    total = total + item.Amount;
                    details.Add(item);
                }

                // Add new BudgetItemsByMonth to our list
                summary.Add(new BudgetItemsByMonth
                {
                    Month = MonthGroup.Key,
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }

        /// <summary>
        /// Group all expenses by category. Creats a new list that will hold the grouped categoires.
        /// </summary>
        /// 
        /// <param name="Start">The start date </param>
        /// <param name="End">The due date</param>
        /// <param name="FilterFlag">The unwanted item filter.</param>
        /// <param name="CategoryID">The category id of the budget items</param>
        /// 
        /// <returns>A list of budget items</returns>
        public List<BudgetItemsByCategory> GeBudgetItemsByCategory(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items first
            // -----------------------------------------------------------------------
            List<BudgetItem> items = GetBudgetItems(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // Group by Category
            // -----------------------------------------------------------------------
            var GroupedByCategory = items.GroupBy(c => c.Category);

            // -----------------------------------------------------------------------
            // create new list
            // -----------------------------------------------------------------------
            var summary = new List<BudgetItemsByCategory>();
            foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
            {
                // calculate total for this category, and create list of details
                double total = 0;
                var details = new List<BudgetItem>();
                foreach (var item in CategoryGroup)
                {
                    total = total + item.Amount;
                    details.Add(item);
                }

                // Add new BudgetItemsByCategory to our list
                summary.Add(new BudgetItemsByCategory
                {
                    Category = CategoryGroup.Key,
                    Details = details,
                    Total = total
                });
            }

            return summary;
        }



        // ============================================================================
        // Group all expenses by category and Month
        // creates a list of ExpandoObjects... which are objects that can have
        //   properties added to it on the fly.
        // ... for each element of the list (expenses by month), the ExpandoObject will have a property
        //     Month = (year/month) (string)
        //     Total = Double total for that month
        //     and for each category that had an entry in that month...
        //     1) Name of category , 
        //     2) and a property called "details: <name of category>" 
        //  
        // ... the last element of the list will contain an ExpandoObject
        //     with the properties for each category, equal to the totals for that
        //     category, and the name of the "Month" property will be "Totals"
        // ============================================================================

        /// <summary>
        /// Groups all expenses by category and month. Gets all the 
        /// items by month and breaks the month details into categories. 
        /// Calculates the final total by category/month. Creates a list of ExpandoObjects(Objects that can have
        /// properties added to it on the fly).
        /// </summary>
        /// 
        /// <param name="Start">The start date</param>
        /// <param name="End">The due date</param>
        /// <param name="FilterFlag">The unwanted item filter.</param>
        /// <param name="CategoryID">The category id of the budget items</param>
        /// 
        /// <returns>A list of records of all the months/categories</returns>
        public List<Dictionary<string,object>> GetBudgetDictionaryByCategoryAndMonth(DateTime? Start, DateTime? End, bool FilterFlag, int CategoryID)
        {
            // -----------------------------------------------------------------------
            // get all items by month 
            // -----------------------------------------------------------------------
            List<BudgetItemsByMonth> GroupedByMonth = GetBudgetItemsByMonth(Start, End, FilterFlag, CategoryID);

            // -----------------------------------------------------------------------
            // loop over each month
            // -----------------------------------------------------------------------
            var summary = new List<Dictionary<string, object>>();
            var totalsPerCategory = new Dictionary<String, Double>();

            foreach (var MonthGroup in GroupedByMonth)
            {
                // create record object for this month
                Dictionary<string, object> record = new Dictionary<string, object>();
                record["Month"] = MonthGroup.Month;
                record["Total"] = MonthGroup.Total;

                // break up the month details into categories
                var GroupedByCategory = MonthGroup.Details.GroupBy(c => c.Category);

                // -----------------------------------------------------------------------
                // loop over each category
                // -----------------------------------------------------------------------
                foreach (var CategoryGroup in GroupedByCategory.OrderBy(g => g.Key))
                {

                    // calculate totals for the cat/month, and create list of details
                    double total = 0;
                    var details = new List<BudgetItem>();

                    foreach (var item in CategoryGroup)
                    {
                        total = total + item.Amount;
                        details.Add(item);
                    }

                    // add new properties and values to our record object
                    record["details:" + CategoryGroup.Key] =  details;
                    record[CategoryGroup.Key] = total;

                    // keep track of totals for each category
                    if (totalsPerCategory.TryGetValue(CategoryGroup.Key, out Double CurrentCatTotal))
                    {
                        totalsPerCategory[CategoryGroup.Key] = CurrentCatTotal + total;
                    }
                    else
                    {
                        totalsPerCategory[CategoryGroup.Key] = total;
                    }
                }

                // add record to collection
                summary.Add(record);
            }
            // ---------------------------------------------------------------------------
            // add final record which is the totals for each category
            // ---------------------------------------------------------------------------
            Dictionary<string, object> totalsRecord = new Dictionary<string, object>();
            totalsRecord["Month"] = "TOTALS";

            foreach (var cat in categories.List())
            {
                try
                {
                    totalsRecord.Add(cat.Description, totalsPerCategory[cat.Description]);
                }
                catch { }
            }
            summary.Add(totalsRecord);


            return summary;
        }




        #endregion GetList

    }
}
