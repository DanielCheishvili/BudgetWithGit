using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;

namespace Budget
{
    [TestClass]
    public class TestHomeBudget
    {
        string testInputFile = TestConstants.testBudgetFile;
        

        // ========================================================================

        [TestMethod]
        public void HomeBudgetObject_New_NoFileSpecified()
        {
            // Arrange

            // Act
            HomeBudget homeBudget  = new HomeBudget("abc.txt");

            // Assert 
            Assert.IsInstanceOfType(homeBudget, typeof(HomeBudget));

            Assert.IsTrue(typeof(HomeBudget).GetProperty("FileName").CanWrite == false, "Filename read only");
            Assert.IsTrue(typeof(HomeBudget).GetProperty("DirName").CanWrite == false, "Dirname read only");
            Assert.IsTrue(typeof(HomeBudget).GetProperty("PathName").CanWrite == false, "Pathname read only");
            Assert.IsTrue(typeof(HomeBudget).GetProperty("categories").CanWrite == false, "categories read only");
            Assert.IsTrue(typeof(HomeBudget).GetProperty("expenses").CanWrite == false, "expenses read only");

            Assert.AreEqual(0, homeBudget.expenses.List().Count, "No expenses");
            Assert.AreNotEqual(0, homeBudget.categories.List().Count, "Default categories present");
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetObject_New_WithFilename()
        {
            // Arrange
            string file = GetSolutionDir() + "\\" + testInputFile;
            int numExpenses = TestConstants.numberOfExpensesInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            // Act
            HomeBudget homeBudget = new HomeBudget(file);

            // Assert 
            Assert.IsInstanceOfType(homeBudget, typeof(HomeBudget));
            Assert.AreEqual(numExpenses, homeBudget.expenses.List().Count, "Correct number of expenses read");
            Assert.AreEqual(numCategories, homeBudget.categories.List().Count, "Correct number of categories read");

        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgeMethod_ReadFromFile_ReadsCorrectData()
        {
            // Arrange
            string file = GetSolutionDir() + "\\" + testInputFile;
            int numExpenses = TestConstants.numberOfExpensesInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;
            Expense firstExpenseInFile = TestConstants.firstExpenseInFile;
            Category firstCategoryInFile = TestConstants.firstCategoryInFile;
            HomeBudget homeBudget = new HomeBudget("abc.txt");

            // Act
            homeBudget.ReadFromFile(file);
            Expense firstExpense = homeBudget.expenses.List()[0];
            Category firstCategory = homeBudget.categories.List()[0];


            // Assert 
            Assert.AreEqual(numExpenses, homeBudget.expenses.List().Count, "Correct number of expenses read");
            Assert.AreEqual(numCategories, homeBudget.categories.List().Count, "Correct number of categories read");
            Assert.AreEqual(firstExpenseInFile.Description, firstExpense.Description, "expense descr is correct");
            Assert.AreEqual(firstCategoryInFile.Description, firstCategory.Description, "category descr is correct");
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_SaveToFile_FilesAreCreated()
        {
            // Arrange
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            int numExpenses = TestConstants.numberOfExpensesInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            HomeBudget homeBudget = new HomeBudget(inFile);
            String outputFile = GetSolutionDir() + "\\" + TestConstants.outputTestBudgetFile;

            String path = Path.GetDirectoryName(Path.GetFullPath(outputFile));
            String file = Path.GetFileNameWithoutExtension(outputFile);
            String ext = Path.GetExtension(outputFile);
            String output_budget = outputFile;
            String output_expenses = Path.Combine(path, file + "_expenses.exps");
            String output_categories = Path.Combine(path, file + "_categories.cats");

            File.Delete(output_budget);
            File.Delete(output_expenses);
            File.Delete(output_categories);

            // Act
            homeBudget.SaveToFile(outputFile);

            // Assert 

            Assert.IsTrue(File.Exists(output_budget), output_budget + " file exists");
            Assert.IsTrue(File.Exists(output_expenses), output_expenses + " file exists");
            Assert.IsTrue(File.Exists(output_categories), output_categories + "file exists");

        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_SaveToFile_FilesAreWrittenTo()
        {
            // Arrange
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            int numExpenses = TestConstants.numberOfExpensesInFile;
            int numCategories = TestConstants.numberOfCategoriesInFile;

            HomeBudget homeBudget = new HomeBudget(inFile);
            String outputFile = GetSolutionDir() + "\\" + TestConstants.outputTestBudgetFile;

            String path = Path.GetDirectoryName(Path.GetFullPath(outputFile));
            String file = Path.GetFileNameWithoutExtension(outputFile);
            String ext = Path.GetExtension(outputFile);
            String output_budget = outputFile;
            String output_expenses = Path.Combine(path, file + "_expenses.exps");
            String output_categories = Path.Combine(path, file + "_categories.cats");
            string input_expenses = Path.Combine(GetSolutionDir(), TestConstants.testExpensesInputFile);
            string input_categories = Path.Combine(GetSolutionDir(), TestConstants.testCategoriesInputFile);

            File.Delete(output_budget);
            File.Delete(output_expenses);
            File.Delete(output_categories);

            // Act
            homeBudget.SaveToFile(outputFile);

            // Assert 
            Assert.IsTrue(File.Exists(output_budget), output_budget + " file exists");
            Assert.IsTrue(File.Exists(output_expenses), output_expenses + " file exists");
            Assert.IsTrue(File.Exists(output_categories), output_categories + "file exists");

            string[] contents = File.ReadAllLines(output_budget);
            Assert.IsTrue(contents.Length==2);
            Assert.IsTrue(contents[0] == file + "_categories.cats", "categorie file " + contents[0]);
            Assert.IsTrue(contents[1] == file + "_expenses.exps", "expenses file " + contents[1]);

            Assert.IsTrue(File.Exists(output_budget));
            Assert.IsTrue(FileSameSize(input_categories, output_categories),
                "Same number of bytes in categories file, assume files are same - " +
                "testing for accuracy is in categories test file");
            Assert.IsTrue(FileSameSize(input_expenses, output_expenses),
                 "Same number of bytes in expenses file, assume files are same - " +
                 "testing for accuracy is in expenses test file");

        }

 

        // ========================================================================

        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

        private String GetSolutionDir()
        {

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

        private bool FileSameSize(string path1, string path2)
        {
            byte[] file1 = File.ReadAllBytes(path1);
            byte[] file2 = File.ReadAllBytes(path2);
            return (file1.Length == file2.Length);
        }

    }
}

