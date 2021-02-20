using System;
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

            SQLiteCommand cmd = new SQLiteCommand(dbConnection);
            cmd.CommandText = "DROP TABLE IF EXISTS expenses";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categories";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "DROP TABLE IF EXISTS categoryTypes";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categoryTypes(Id INTEGER PRIMARY KEY, Description TEXT)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO categoryTypes (Id,Description) VALUES (@Id, @desc)";
            cmd.Parameters.AddWithValue("@desc","Income");
            cmd.Parameters.AddWithValue("@Id",1);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO categoryTypes (Id,Description) VALUES (@Id, @desc)";
            cmd.Parameters.AddWithValue("@desc", "Expense");
            cmd.Parameters.AddWithValue("@Id", 2);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO categoryTypes (Id,Description) VALUES (@Id, @desc)";
            cmd.Parameters.AddWithValue("@desc", "Credit");
            cmd.Parameters.AddWithValue("@Id", 3);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"INSERT INTO categoryTypes (Id,Description) VALUES (@Id, @desc)";
            cmd.Parameters.AddWithValue("@desc", "Savings");
            cmd.Parameters.AddWithValue("@Id", 4);
            cmd.Prepare();
            cmd.ExecuteNonQuery();

            cmd.CommandText = @"CREATE TABLE categories (Id INTEGER PRIMARY KEY, Description TEXT, TypeId INTEGER REFERENCES categoryTypes(Id))";
            cmd.ExecuteNonQuery();

            
            cmd.CommandText = @"CREATE TABLE expenses (Id INTEGER PRIMARY KEY, Date TEXT, Description TEXT, Amount DOUBLE, CategoryId INTEGER REFERENCES categories(Id))";
            cmd.ExecuteNonQuery();
        }
        public static void openExistingDatabase(String path)
        {
            string cs = $"Data Source={path}; Foreign Keys=1";
            dbConnection = new SQLiteConnection(cs);
            dbConnection.Open();


        }
    }
}
