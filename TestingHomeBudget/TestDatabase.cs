using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace BudgetCodeTests
{
    [TestClass]
    public class TestDatabase
    {

        [TestMethod]
        public void SQLite_TestNewDatabase_TablesCreated()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> tables = new List<String>() { "categoryTypes", "expenses", "categories" };

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            string cmd = " .tables";
            List<String> databaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput("\""+path + "\\" + filename+"\""+ cmd);
            if (databaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no tables created in new database ");
            }

            String table_string = databaseOutput[0];
            foreach (String table in tables)
            {
                Assert.IsTrue(table_string.Contains(table), $"table {table} in database");
            }
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ForeignKeyConstraintsEnabled()
        {

            // For SQLite, you need to use the following as a connection string
            // if you want your foreign key constraints to work.

            // string cs = $"Data Source={filepath}; Foreign Keys=1";

            // PS: Validate externally that having the above connection string does indeed
            //     turn on foreign key constraints :)

            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            String connectionString = Database.dbConnection.ConnectionString;
            Assert.IsTrue(connectionString.Contains("Foreign Keys=1"), "FK Constraints enabled");
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ColumnsInTableExpenses()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "CategoryId", "Amount", "Date", "Description" };

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(expenses)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput("\"" + path + "\\" + filename + "\"" + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no columns in table expenses ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.AreNotEqual(-1, index, $"column {column} found in table expenses");
            }
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ColumnsInTableCategory()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id", "Description", "TypeId" };

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(categories)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput("\"" + path + "\\" + filename + "\"" + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no columns in table categories ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.AreNotEqual(-1, index, $"column {column} found in table categories");
            }
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_ColumnsInTableCategoryTypes()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            List<String> columns = new List<string>() { "Id","Description" };

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            string cmd = " \".mode list\" \"pragma table_info(CategoryTypes)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput("\"" + path + "\\" + filename + "\"" + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no columns in table types ");
            }

            // Assert
            foreach (String column in columns)
            {
                int index = DatabaseOutput.FindIndex(s => s.Contains($"|{column}|"));
                Assert.AreNotEqual(-1, index, $"column {column} found in table CategoryTypes");
            }
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_RequiredForeignKeysCategories()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            Dictionary<String, String> FKtable = new Dictionary<String,String>()
            {
                {"table", "categoryTypes"},
                { "from", "TypeId" },
                {"to", "Id" },
            };

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            string cmd = " \".mode line\" \"pragma foreign_key_list(categories)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput("\"" + path + "\\" + filename + "\"" + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no foreign in table categories ");
            }

            // Assert
            foreach (KeyValuePair<string, string> kvp in FKtable)
            {
                String FKProperty = $"{kvp.Key} = {kvp.Value}";
                int index = DatabaseOutput.FindIndex(s => s.Contains(FKProperty));
                Assert.AreNotEqual(-1, index, $"{FKProperty} in table categories");
            }
        }

        [TestMethod]
        public void SQLite_TestNewDatabase_RequiredForeignKeysExpenses()
        {
            // Arrange
            string path = TestConstants.GetSolutionDir();
            string filename = "newdb.db";
            Dictionary<String, String> FKtable = new Dictionary<String, String>()
            {
                {"table", "categories"},
                { "from", "CategoryId" },
                {"to", "Id" },
            };

            // Act
            Database.newDatabase(TestConstants.GetSolutionDir() + "\\" + filename);

            // Assert
            string cmd = " \".mode line\" \"pragma foreign_key_list(expenses)\"";
            List<String> DatabaseOutput = DatabaseCommandLine.ExecuteAndReturnOutput("\"" + path + "\\" + filename + "\"" + cmd);
            if (DatabaseOutput.Count < 1)
            {
                Assert.IsTrue(false, "There were no foreign in table expenses ");
            }

            // Assert
            foreach (KeyValuePair<string, string> kvp in FKtable)
            {
                String FKProperty = $"{kvp.Key} = {kvp.Value}";
                int index = DatabaseOutput.FindIndex(s => s.Contains(FKProperty));
                Assert.AreNotEqual(-1, index, $"{FKProperty} in table expenses");
            }
        }

    }




    public class DatabaseCommandLine
    {
        static public List<String> ExecuteAndReturnOutput(string DatabaseCmd)
        {
            // https://stackoverflow.com/questions/206323/how-to-execute-command-line-in-c-get-std-out-results

            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = "sqlite3";

            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = DatabaseCmd;

            pProcess.StartInfo.UseShellExecute = false;

            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;

            //Start the process
            pProcess.Start();

            //Wait for process to finish
            pProcess.WaitForExit();

            //Get program output
            string strOutput = pProcess.StandardOutput.ReadToEnd();

            // Convert the output to a list of strings
            List<String> output = new List<string>();
            using (System.IO.StringReader reader = new System.IO.StringReader(strOutput))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    output.Add(line);
                }
            }

            return output;

        }
    }
}
