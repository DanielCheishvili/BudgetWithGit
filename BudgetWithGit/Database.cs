using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Budget
{
    public class Database
    {
         public static SQLiteConnection dbConnection;

        public Database()
        {

        }
        public Database(String path)
        {

        }
        public static void newDatabase(String path)
        {
            string cs = $"Data Source={path}; Foreign Keys=1";
            dbConnection = new SQLiteConnection(cs);
            dbConnection.Open();
            Categories category = new Categories();
            
        }
        public static void openExistingDatabase(String path)
        {
            string cs = $"Data Source={path}; Foreign Keys=1";
            dbConnection = new SQLiteConnection(cs);
            dbConnection.Open();

            Categories categories = new Categories(dbConnection, true);
            string descr = "New Category";
            Category.CategoryType type = Category.CategoryType.Income;
            categories.Add(descr,type);

 
        }
    }
}
