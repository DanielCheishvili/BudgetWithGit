using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace Budget
{
    [TestClass]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        String testInputFile = TestConstants.testExpensesInputFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;
        Expense firstExpenseInFile = new Expense(1, new DateTime(2021, 1, 10), 10, 12, "hat (on credit)");


        // ========================================================================

        [TestMethod]
        public void ExpensesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            // Act
            Expenses expenses = new Expenses(conn);

            // Assert 
            Assert.IsInstanceOfType(expenses, typeof(Expenses));

            Assert.IsTrue(typeof(Expenses).GetProperty("FileName").CanWrite == false);
            Assert.IsTrue(typeof(Expenses).GetProperty("DirName").CanWrite == false);
            Database.CloseDatabaseAndReleaseFile();


        }


        // ========================================================================

        
       
        // ========================================================================

    

        [TestMethod]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.openExistingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);

            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.AreEqual(numberOfExpensesInFile, list.Count);
            Database.CloseDatabaseAndReleaseFile();


        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_List_ModifyListDoesNotModifyExpensesInstance()
        {
            // Arrange

            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.openExistingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            List<Expense> list = expenses.List();

            // Act
            list[0].Amount = list[0].Amount + 21.03; 

            // Assert
            Assert.AreNotEqual(list[0].Amount, expenses.List()[0].Amount,"Modifying list should not modify Expense Object");
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Add()
        {
            // Arrange
            
            
            int category = 10;
            double amount = 98.1;
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);

            // Act
            expenses.Add(DateTime.Now,category,amount,"new expense");
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.AreEqual(numberOfExpensesInFile+1, sizeOfList,"List size incremented");
            Assert.AreEqual(maxIDInExpenseFile + 1, expensesList[sizeOfList - 1].Id, "Id set to max + 1");
            Assert.AreEqual(amount, expensesList[sizeOfList - 1].Amount, "Amount property set correctly");
            Database.CloseDatabaseAndReleaseFile();


        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);

            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
     
            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.AreEqual(numberOfExpensesInFile - 1, sizeOfList, "List size decremented");
            Assert.IsFalse(expensesList.Exists(e => e.Id == IdToDelete), "correct expense item deleted");
            Database.CloseDatabaseAndReleaseFile();


        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            int IdToDelete = 1006;
            int sizeOfList = expenses.List().Count;

            // Act
            try
            {
                expenses.Delete(IdToDelete);
                Assert.AreEqual(sizeOfList, expenses.List().Count, "No Expense was removed from list");
            }

            // Assert
            catch
            {
                Assert.IsTrue(false, "Invalid ID causes Delete to break");
            }
            Database.CloseDatabaseAndReleaseFile();

        }


        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

        private String GetSolutionDir() {

            // this is valid for C# .Net Foundation (not for C# .Net Core)
            return Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\"));
        }

        // source taken from: https://www.dotnetperls.com/file-equals

        private bool FileEquals(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            if (file1.Length == file2.Length)
            {
                for (int i = 0; i < file1.Length; i++)
                {
                    if (file1[i] != file2[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }
    }
}

