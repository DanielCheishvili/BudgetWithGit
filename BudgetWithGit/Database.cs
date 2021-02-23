using System;
using System.Data.SQLite;
using System.IO;

namespace Budget
{
    public class Database
    {
        public static SQLiteConnection dbConnection;

        public static void newDatabase(String path)
        {
            string cs = $"Data Source={path}; Foreign Keys=1";
            dbConnection = new SQLiteConnection(cs);
            dbConnection.Open();

            SQLiteCommand cmd = new SQLiteCommand(dbConnection);
            cmd.CommandText = "DROP TABLE IF EXISTS expenses";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY, Description TEXT)";
            cmd.ExecuteNonQuery();

            for (int i = 1; i <= Enum.GetNames(typeof(Category.CategoryType)).Length; i++)
            {
                cmd.CommandText = "INSERT INTO categoryTypes(Id, Description) VALUES(@id, @desc)";
                cmd.Parameters.AddWithValue("@id", i);
                cmd.Parameters.AddWithValue("@desc", Enum.GetName(typeof(Category.CategoryType), i));
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
           

            cmd.CommandText = @"CREATE TABLE categories (Id INTEGER PRIMARY KEY, Description TEXT, TypeId INTEGER REFERENCES categoryTypes(Id))";
            cmd.ExecuteNonQuery();

            
            cmd.CommandText = @"CREATE TABLE expenses (Id INTEGER PRIMARY KEY, Date TEXT, Description TEXT, Amount DOUBLE, CategoryId INTEGER REFERENCES categories(Id))";
            cmd.ExecuteNonQuery();
        }
        public static void openExistingDatabase(String path)
        {
            if(File.Exists(path))
            {
                string cs = $"Data Source={path}; Foreign Keys=1";
                dbConnection = new SQLiteConnection(cs);
                dbConnection.Open();
            }
            else
            {
                throw new FileNotFoundException("The file " + path + " was not found");
            }
            


        }
    }
}
