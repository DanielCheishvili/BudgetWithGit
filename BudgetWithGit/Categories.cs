﻿using System;
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
        //private static String DefaultFileName = "budgetCategories.txt";
        private List<Category> _Cats = new List<Category>();
        private string _FileName;
        private string _DirName;
        private SQLiteConnection dbConnection;


        /// <summary>
        /// Gets the filename.
        /// </summary>
        public String FileName { get { return _FileName; } }

        /// <summary>
        /// Gets the directory name.
        /// </summary>
        public String DirName { get { return _DirName; } }

        
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

            string selectID = $"select * from categories where id = {i}";

            SQLiteCommand cmd = new SQLiteCommand(selectID, this.dbConnection);
            SQLiteDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            Category cat = new Category(rdr.GetInt32(0), rdr.GetString(1), (Category.CategoryType)rdr.GetInt32(2));
            rdr.Close();

            return cat; 
        }

        /// <summary>
        /// populate categories from a file. Resets the current categories.
        /// </summary>
        /// 
        /// <param name="filepath"> A file path that the user will specifiy but originally set to null</param>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current categories,
            // ---------------------------------------------------------------
            _Cats.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath/*, DefaultFileName*/);

            // ---------------------------------------------------------------
            // If file exists, read it
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }

        /// <summary>
        /// save to a file. saves the file in an XML format.
        /// </summary>
        /// <param name="filepath">A file path that the user will specifiy but originally set to null</param>
        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath/*, DefaultFileName*/);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
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

       
        private void Add(Category cat)
        {
            _Cats.Add(cat);
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
            /*List<Category> newList = new List<Category>();
            foreach (Category category in _Cats)
            {
                newList.Add(new Category(category));
            }
            return newList;*/

            string selectCategory = "select * from categories ORDER BY id ASC;";


            SQLiteCommand cmd = new SQLiteCommand(selectCategory, this.dbConnection);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            List<Category> newList = new List<Category>();
            while (rdr.Read())
            {
                newList.Add(new Category(rdr.GetInt32(0),rdr.GetString(1),(Category.CategoryType)rdr.GetInt32(2)));
            }
            return newList;
            
        }

        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {

            // ---------------------------------------------------------------
            // read the categories from the xml file, and add to this instance
            // ---------------------------------------------------------------
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                foreach (XmlNode category in doc.DocumentElement.ChildNodes)
                {
                    String id = (((XmlElement)category).GetAttributeNode("ID")).InnerText;
                    String typestring = (((XmlElement)category).GetAttributeNode("type")).InnerText;
                    String desc = ((XmlElement)category).InnerText;

                    Category.CategoryType type;
                    switch (typestring.ToLower())
                    {
                        case "income":
                            type = Category.CategoryType.Income;
                            break;
                        case "expense":
                            type = Category.CategoryType.Expense;
                            break;
                        case "credit":
                            type = Category.CategoryType.Credit;
                            break;
                        case "savings":
                            type = Category.CategoryType.Savings;
                            break;
                        default:
                            type = Category.CategoryType.Expense;
                            break;
                    }
                    this.Add(new Category(int.Parse(id), desc, type));
                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadXMLFile: Reading XML " + e.Message);
            }

        }


        // ====================================================================
        // write all categories in our list to XML file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            try
            {
                // create top level element of categories
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Categories></Categories>");

                // foreach Category, create an new xml element
                foreach (Category cat in _Cats)
                {
                    XmlElement ele = doc.CreateElement("Category");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = cat.Id.ToString();
                    ele.SetAttributeNode(attr);
                    XmlAttribute type = doc.CreateAttribute("type");
                    type.Value = cat.Type.ToString();
                    ele.SetAttributeNode(type);

                    XmlText text = doc.CreateTextNode(cat.Description);
                    doc.DocumentElement.AppendChild(ele);
                    doc.DocumentElement.LastChild.AppendChild(text);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("_WriteXMLFile: Reading XML " + e.Message);
            }

        }

    }
}

