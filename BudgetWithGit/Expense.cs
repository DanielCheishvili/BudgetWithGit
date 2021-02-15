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
    /// Creates an individual expense for the budget program.
    /// </summary>
    public class Expense
    {

        /// <summary>
        ///  Gets the Id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///  Gets the date.
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        ///  Gets and sets the amount.
        /// </summary>
        public Double Amount { get; set; }

        /// <summary>
        ///  Gets and sets the description.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        ///  Gets and sets the category.
        /// </summary>
        public int Category { get; set; }

        // ====================================================================
        // Constructor
        //    NB: there is no verification the expense category exists in the
        //        categories object
        // ====================================================================

        /// <summary>
        /// Constructor to initalize the object. There is no verification the expense category exists in the
        /// categories object.
        /// </summary>
        /// 
        /// <param name="id">The Id of the expense</param>
        /// <param name="date">The date of the expense</param>
        /// <param name="category">The category of the expense</param>
        /// <param name="amount">The amount of the expense</param>
        /// <param name="description">The description of the expense</param>
        public Expense(int id, DateTime date, int category, Double amount, String description)
        {
            this.Id = id;
            this.Date = date;
            this.Category = category;
            this.Amount = amount;
            this.Description = description;
        }



        /// <summary>
        /// Copy constructor - does a deep copy
        /// </summary>
        /// 
        /// <param name="obj">Holds the expense object </param>
        public Expense (Expense obj)
        {
            this.Id = obj.Id;
            this.Date = obj.Date;
            this.Category = obj.Category;
            this.Amount = obj.Amount;
            this.Description = obj.Description;
           
        }
    }
}
