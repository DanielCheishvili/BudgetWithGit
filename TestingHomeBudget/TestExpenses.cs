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
            Expenses expenses = new Expenses();

            // Assert 
            Assert.IsInstanceOfType(expenses, typeof(Expenses));

            Assert.IsTrue(typeof(Expenses).GetProperty("FileName").CanWrite == false);
            Assert.IsTrue(typeof(Expenses).GetProperty("DirName").CanWrite == false);

        }


        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_ReadFromFile_NotExist_ThrowsException()
        {
            // Arrange
            String badFile = "abc.txt";
            Expenses expenses = new Expenses();

            // Act and Assert
            Assert.ThrowsException<System.IO.FileNotFoundException>(() => expenses.ReadFromFile(badFile));

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_ReadFromFile_ValidateCorrectDataWasRead()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();

            // Act
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            List<Expense> list = expenses.List();
            Expense firstExpense = list[0];

            // Assert
            Assert.AreEqual(numberOfExpensesInFile, list.Count,"Number of list elements are correct");
            Assert.AreEqual(firstExpenseInFile.Id, firstExpense.Id, "ID of first element");
            Assert.AreEqual(firstExpenseInFile.Amount, firstExpense.Amount, "Amount of first element");
            Assert.AreEqual(firstExpenseInFile.Description, firstExpense.Description, "Description of first Element");
            Assert.AreEqual(firstExpenseInFile.Category, firstExpense.Category, "Category of First Element");

            String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
            Assert.AreEqual(dir, fileDir, "Property directory name has been set");
            Assert.AreEqual(testInputFile, expenses.FileName, "Property filename has been set");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_List_ReturnsListOfExpenses()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);

            // Act
            List<Expense> list = expenses.List();

            // Assert
            Assert.AreEqual(numberOfExpensesInFile, list.Count);

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_List_ModifyListDoesNotModifyExpensesInstance()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            List<Expense> list = expenses.List();

            // Act
            list[0].Amount = list[0].Amount + 21.03; 

            // Assert
            Assert.AreNotEqual(list[0].Amount, expenses.List()[0].Amount,"Modifying list should not modify Expense Object");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Add()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            int category = 57;
            double amount = 98.1;
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            expenses.Add(DateTime.Now,category,amount,"new expense");
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expenses.List().Count;

            // Assert
            Assert.AreEqual(numberOfExpensesInFile+1, sizeOfList,"List size incremented");
            Assert.AreEqual(maxIDInExpenseFile + 1, expensesList[sizeOfList - 1].Id, "Id set to max + 1");
            Assert.AreEqual(amount, expensesList[sizeOfList - 1].Amount, "Amount property set correctly");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Delete()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            int IdToDelete = 3;

            // Act
            expenses.Delete(IdToDelete);
            List<Expense> expensesList = expenses.List();
            int sizeOfList = expensesList.Count;

            // Assert
            Assert.AreEqual(numberOfExpensesInFile - 1, sizeOfList, "List size decremented");
            Assert.IsFalse(expensesList.Exists(e => e.Id == IdToDelete), "correct expense item deleted");

        }

        // ========================================================================

        [TestMethod]
        public void ExpensesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
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
        }


        // ========================================================================

        [TestMethod]
        public void ExpenseMethod_WriteToFile()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.ExpenseOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);

            // Act
            expenses.SaveToFile(outputFile);

            // Assert
            Assert.IsTrue(File.Exists(outputFile), "output file created");
            Assert.IsTrue(FileEquals(dir + "\\" + testInputFile, outputFile), "Input /output files are the same");
            String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
            Assert.AreEqual(dir, fileDir, "Property directory name has been set");
            Assert.AreEqual(fileName, expenses.FileName, "Property filename has been set");

            // Cleanup
            if (FileEquals(dir + "\\" + testInputFile, outputFile))
            {
                File.Delete(outputFile);
            }

        }

        // ========================================================================

        [TestMethod]
         public void ExpenseMethod_WriteToFile_VerifyNewExpenseWrittenCorrectly()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.ExpenseOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);

            // Act
            expenses.Add(DateTime.Now, 14, 35.27, "McDonalds");
            List<Expense> listBeforeSaving = expenses.List();
            expenses.SaveToFile(outputFile);
            expenses.ReadFromFile(outputFile);
            List<Expense> listAfterSaving = expenses.List();

            Expense beforeSaving = listBeforeSaving[listBeforeSaving.Count - 1];
            Expense afterSaving = listAfterSaving.Find(e => e.Id == beforeSaving.Id);

            // Assert
            Assert.AreEqual(beforeSaving.Id, afterSaving.Id, "IDs are the same");
            Assert.AreEqual(beforeSaving.Category, afterSaving.Category, "Categories are the same");
            Assert.AreEqual(beforeSaving.Description, afterSaving.Description, "Description is the same");
            Assert.AreEqual(beforeSaving.Amount, afterSaving.Amount, "Amount " + beforeSaving.Amount + " is correct");

        }

        // ========================================================================

        [TestMethod]
        public void ExpenseMethod_WriteToFile_WriteToLastFileWrittenToByDefault()
        {
            // Arrange
            String dir = GetSolutionDir();
            Expenses expenses = new Expenses();
            expenses.ReadFromFile(dir + "\\" + testInputFile);
            string fileName = TestConstants.ExpenseOutputTestFile;
            String outputFile = dir + "\\" + fileName;
            File.Delete(outputFile);
            expenses.SaveToFile(outputFile); // output file is now last file that was written to.
            File.Delete(outputFile);  // Delete the file

            // Act
            expenses.SaveToFile(); // should write to same file as before

            // Assert
            Assert.IsTrue(File.Exists(outputFile), "output file created");
            String fileDir = Path.GetFullPath(Path.Combine(expenses.DirName, ".\\"));
            Assert.AreEqual(dir, fileDir, "Property directory name has been set");
            Assert.AreEqual(fileName, expenses.FileName, "Property filename has been set");

            // Cleanup
            if (FileEquals(dir + "\\" + testInputFile, outputFile))
            {
                File.Delete(outputFile);
            }

        }

        // ========================================================================



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

