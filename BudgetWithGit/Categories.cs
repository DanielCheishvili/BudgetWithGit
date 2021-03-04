using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Xml;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    // ====================================================================
    // CLASS: categories
    //        - A collection of category items,
    //        - Read / write to file
    //        - etc
    // ====================================================================

    /// <summary>
    /// A collection of category items that also reads / write to a file 
    /// and saves a file as well as removing and adding categories. For files it uses
    /// the BudgetFiles.cs in order to save and write to file.
    /// </summary>
    public class Categories
    {

        private SQLiteConnection dbConnection;

        
        /// <summary>
        /// Consturctor that calls a function that sets the categories to default
        /// </summary>
        public Categories()
        {
            SetCategoriesToDefaults();
        }

        public Categories(SQLiteConnection conn, bool newDb)
        {
            this.dbConnection = conn;
            if(newDb)
            {
                SetCategoriesToDefaults();              
            }
            //open connection
            
        }
        public Category UpdateProperties(int id, string newDescr, Category.CategoryType type)
        {
            Category catUpdate = GetCategoryFromId(id);
            catUpdate.Description = newDescr;
            catUpdate.Type = type;

            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
            cmd.CommandText = "UPDATE categories SET description = @desc, TypeId = @type where id = @id";
            cmd.Parameters.AddWithValue("@desc", catUpdate.Description);
            cmd.Parameters.AddWithValue("@type", (int)catUpdate.Type);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            return catUpdate;
        }

        /// <summary>
        /// Finds a specific category from the list where the id is the one that is specified.
        /// </summary>
        /// 
        /// <exception cref="Exception">Throws if the category is empty.</exception>
        /// 
        /// <param name="i">The specified ID</param>
        /// 
        /// <returns>The object category</returns>
        public Category GetCategoryFromId(int i)
        {

            string selectID = $"select Id, Description, TypeId from categories where id = @Id";


            SQLiteCommand cmd = new SQLiteCommand(selectID, this.dbConnection);
            cmd.Parameters.AddWithValue("@Id",i);
            cmd.Prepare();

            SQLiteDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            Category cat = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2));
            rdr.Close();

            return cat; 
        }
       

        /// <summary>
        /// Sets the categories to default. It resets all current cateogories if any
        /// </summary>
        public void SetCategoriesToDefaults()
        {
            // ---------------------------------------------------------------
            // reset any current categories,
            // ---------------------------------------------------------------
            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
            cmd.CommandText = "DELETE FROM categories";
            cmd.ExecuteNonQuery();
            // ---------------------------------------------------------------
            // Add Defaults
            // ---------------------------------------------------------------
            Add("Utilities", Category.CategoryType.Expense);
            Add("Rent", Category.CategoryType.Expense);
            Add("Food", Category.CategoryType.Expense);
            Add("Entertainment", Category.CategoryType.Expense);
            Add("Education", Category.CategoryType.Expense);
            Add("Miscellaneous", Category.CategoryType.Expense);
            Add("Medical Expenses", Category.CategoryType.Expense);
            Add("Vacation", Category.CategoryType.Expense);
            Add("Credit Card", Category.CategoryType.Credit);
            Add("Clothes", Category.CategoryType.Expense);
            Add("Gifts", Category.CategoryType.Expense);
            Add("Insurance", Category.CategoryType.Expense);
            Add("Transportation", Category.CategoryType.Expense);
            Add("Eating Out", Category.CategoryType.Expense);
            Add("Savings", Category.CategoryType.Savings);
            Add("Income", Category.CategoryType.Income);

        }

        /// <summary>
        /// Adds categories to the category list.
        /// </summary>
        /// 
        /// <param name="desc">The Desciption of the category</param>
        /// <param name="type">The Type of the category</param>
        public void Add(String desc, Category.CategoryType type)
        {
          
            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
            cmd.CommandText = "DELETE TABLE IF EXISTS categoryTypes";

            cmd.CommandText = "INSERT INTO categoryTypes (Description) VALUES (@Description)";
            cmd.Parameters.AddWithValue("@Description", desc);

            cmd.CommandText = "INSERT INTO categories(Description,TypeId) VALUES (@Description, @TypeId)";

            cmd.Parameters.AddWithValue("@Description", desc);
            cmd.Parameters.AddWithValue("@TypeId", (int)type);

            cmd.Prepare();
            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// Deletes the category from the category list.
        /// </summary>
        /// <param name="Id">The id of the category</param>
        public void Delete(int Id)
        {
            
            try
            {

                SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);

                cmd.CommandText = "DELETE FROM categories WHERE id = @Id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch(Exception e)
            {
                Console.WriteLine("Not allowed to delete in database (foreign key constraint)", e.Message);
            }
            
        }

        /// <summary>
        /// Adds the new categories to the list.
        /// makes a new copy of list, so user cannot modify what is part of this instance.
        /// </summary>
        /// 
        /// <returns>The list of categories</returns>
        public List<Category> List()
        {
            string selectCategory = "select Id, Description, TypeId from categories ORDER BY id ASC;";


            SQLiteCommand cmd = new SQLiteCommand(selectCategory, this.dbConnection);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            List<Category> newList = new List<Category>();
            while (rdr.Read())
            {
                newList.Add(new Category(rdr.GetInt32(0),rdr.GetString(1),(Category.CategoryType)rdr.GetInt32(2)));
            }
            return newList;
            
        }
       

    }
}

