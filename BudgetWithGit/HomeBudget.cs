using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Dynamic;
using System.Data.SQLite;
using System.Globalization;

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
    /// Combines categories class and expenses class.
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
        private Categories _categories;
        private Expenses _expenses;

        /// <summary>
        /// Gets the categories.
        /// </summary>
        public Categories categories { get { return _categories; } }

        /// <summary>
        /// Gets the expenses.
        /// </summary>
        public Expenses expenses { get { return _expenses; } }

        /// <summary>
        /// The only constructor of the HomeBudget class that connects
        /// to a database.Checks if the file exists and wether or not we opening a new database
        /// or using an existing one.
        /// </summary>
        /// <param name="databaseFile">The database file</param>
        /// <param name="newDB">The boolean that checks if its a new database or not</param>
        public HomeBudget(String databaseFile,bool newDB = false)
        {
            //checks if the file exits and if its an existing database.
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
            
            
        }
        #region GetList

        /// <summary>
        /// Gets all expenses list. It joins the categories list with 
        /// the expenses list using an Inner Join query. 
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
            DateTime realStart = Start ?? new DateTime(1900, 1, 1);
            DateTime realEnd = End ?? new DateTime(2500, 1, 1);

            SQLiteCommand cmd = new SQLiteCommand(Database.dbConnection);
            cmd.CommandText = @"SELECT c.Id, c.Description, c.TypeId, e.Id, e.Date, e.Description, e.Amount
                                from categories as c 
                                INNER JOIN expenses as e on c.id == e.CategoryId 
                                WHERE e.Date >= @realStart AND e.date <= @realEnd
                                ORDER BY e.Date ASC";
            cmd.Parameters.AddWithValue("@realStart", realStart);
            cmd.Parameters.AddWithValue("@realEnd", realEnd);
            cmd.Prepare();

            // ------------------------------------------------------------------------
            // create a BudgetItem list with totals,
            // ------------------------------------------------------------------------
            List<BudgetItem> items = new List<BudgetItem>();
            Double total = 0;
            SQLiteDataReader rdr = cmd.ExecuteReader();

            while(rdr.Read())
            {
                // filter out unwanted categories if filter flag is on
                if (FilterFlag && CategoryID != rdr.GetInt32(0))
                {
                    continue;
                }
                BudgetItem budgetItem = new BudgetItem();

                // keep track of running totals
                total += rdr.GetDouble(6);
                budgetItem.CategoryID = rdr.GetInt32(0);
                budgetItem.ExpenseID = rdr.GetInt32(3);
                budgetItem.ShortDescription = rdr.GetString(5);
                budgetItem.Date = DateTime.ParseExact(rdr.GetString(4), "yyyy-MM-dd", CultureInfo.InvariantCulture);
                budgetItem.Amount = rdr.GetDouble(6);
                budgetItem.Category = rdr.GetString(1);
                budgetItem.Balance = total;
                items.Add(budgetItem);

            }
            return items;
        }

        /// <summary>
        /// Group all expenses month by month. Groups by month and year and creates a new list to store it in.
        /// Calculates the total of the month and creates a list of details. Adds it to the created list.
        /// uses the query from the getBudgetItems therefore no need to reuse the code.
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
        /// uses the query from the getBudgetItems therefore no need to reuse the code.
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
