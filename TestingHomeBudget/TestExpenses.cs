using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;
using System.Globalization;

namespace Budget
{
    [TestClass]
    public class TestExpenses
    {
        int numberOfExpensesInFile = TestConstants.numberOfExpensesInFile;
        int maxIDInExpenseFile = TestConstants.maxIDInExpenseFile;


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
        [TestMethod]
        public void ExpensesMethod_GetExpenseFromId()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.openExistingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Expenses expenses = new Expenses(conn);
            int expID = 1;

            // Act
            Expense expense = expenses.GetExpenseFromId(expID);

            // Assert
            Assert.AreEqual(expID, expense.Id);

            Database.CloseDatabaseAndReleaseFile();

        }

        [TestMethod]
        public void ExpensesMethod_UpdateExpense()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories cats = new Categories(conn, false);

            String catDesc = "Category";
            Category.CategoryType type = Category.CategoryType.Savings;
            cats.Add(catDesc, type);

            Expenses expenses = new Expenses(conn);
            expenses.Add(DateTime.ParseExact("2020-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture), 1, 1000, "hat");
            int cat = 1;
            DateTime dateTime = DateTime.ParseExact("2020-11-01", "yyyy-MM-dd", CultureInfo.InvariantCulture);
            Double amt = 12;
            String newDescr = "clothes";
            int id = 1;

            // Act
            expenses.UpdateProperties(id, dateTime, cat,amt,newDescr );
            Expense expense = expenses.GetExpenseFromId(id);

            // Assert 
            Assert.AreEqual(dateTime, expense.Date);
            Assert.AreEqual(cat, expense.Category);
            Assert.AreEqual(amt, expense.Amount);
            Assert.AreEqual(newDescr, expense.Description);

            Database.CloseDatabaseAndReleaseFile();

        }
    }
}

