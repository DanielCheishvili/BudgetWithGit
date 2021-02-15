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
    /// An individual category for the budget program.
    /// </summary>
    public class Category
    {
        
        /// <summary>
        /// Gets and sets the Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets and sets the description.
        /// </summary>
        public String Description { get; set; }

        /// <summary>
        /// Gets and sets the ENUM type.
        /// </summary>
        public CategoryType Type { get; set; }

        /// <summary>
        /// An enum with the categories type.
        /// </summary>
        public enum CategoryType
        {
            /// <summary>
            /// Income category.
            /// </summary>
            Income,

            /// <summary>
            /// Expense category. 
            /// </summary>
            Expense,

            /// <summary>
            /// Credit category.
            /// </summary>
            Credit,

            /// <summary>
            /// Saving category.
            /// </summary>
            Savings
        };

       
        /// <summary>
        /// Constructor to initalize the object
        /// </summary>
        /// 
        /// <param name="id">The ID of the category</param>
        /// <param name="description"> The desciprtion of the category</param>
        /// <param name="type">The type of the category</param>
        public Category(int id, String description, CategoryType type = CategoryType.Expense)
        {
            this.Id = id;
            this.Description = description;
            this.Type = type;
        }

        /// <summary>
        /// a copy of a constructor that holds the category object.
        /// </summary>
        /// 
        /// <param name="category">The category with the id, description and type</param>
        public Category(Category category)
        {
            this.Id = category.Id;;
            this.Description = category.Description;
            this.Type = category.Type;
        }
        
        /// <summary>
        /// Outputs the description of the category.
        /// </summary>
        /// 
        /// <returns>The Description of the category</returns>
        public override string ToString()
        {
            return Description;
        }

    }
}

