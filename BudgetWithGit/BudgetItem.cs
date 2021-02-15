using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    
    /// <summary>
    /// A single budget item, includes Category and Expense that sets and gets
    /// the appropriate fields.
    /// </summary>
    public class BudgetItem
    {
        /// <summary>
        /// Gets and sets the category Id.
        /// </summary>
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets and sets the expense Id.
        /// </summary>
        public int ExpenseID { get; set; }

        /// <summary>
        /// Gets and sets the Date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets and sets the category.
        /// </summary>
        public String Category { get; set; }

        /// <summary>
        /// Gets and sets a short desciption.
        /// </summary>
        public String ShortDescription { get; set; }

        /// <summary>
        /// Gets and sets the amount.
        /// </summary>
        public Double Amount { get; set; }

        /// <summary>
        /// Gets and sets the balance.
        /// </summary>
        public Double Balance { get; set; }

    }

    /// <summary>
    /// Takes the budget items and calcualtes it monthly.
    /// </summary>
    public class BudgetItemsByMonth
    {
        /// <summary>
        /// Gets and sets the Month.
        /// </summary>
        public String Month { get; set; }

        /// <summary>
        /// Gets and sets the details.
        /// </summary>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Gets and sets the total.
        /// </summary>
        public Double Total { get; set; }
    }

    /// <summary>
    /// Takes the budget items and calcualtes it by the category.
    /// </summary>
    public class BudgetItemsByCategory
    {
        /// <summary>
        /// Gets and sets the category.
        /// </summary>
        public String Category { get; set; }

        /// <summary>
        /// Gets and sets the details.
        /// </summary>
        public List<BudgetItem> Details { get; set; }

        /// <summary>
        /// Gets and sets the total.
        /// </summary>
        public Double Total { get; set; }

    }


}
