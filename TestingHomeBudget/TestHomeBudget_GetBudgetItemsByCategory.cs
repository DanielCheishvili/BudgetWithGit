using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;

namespace Budget
{
    [TestClass]
    public class TestHomeBudget_GetBudgetItemsByCategory
    {
        string testInputFile = TestConstants.testExpensesInputFile;
        


        // ========================================================================
        // Get Expenses By Month Method tests
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItemsByCategory_NoStartEnd_NoFilter()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB, false);
            int maxRecords = TestConstants.budgetItemsByCategory_MaxRecords; 
            BudgetItemsByCategory firstRecord = TestConstants.budgetItemsByCategory_FirstRecord;

            // Act
            List<BudgetItemsByCategory> budgetItemsByCategory = homeBudget.GeBudgetItemsByCategory(null, null, false, 9);
            BudgetItemsByCategory firstRecordTest = budgetItemsByCategory[0];

            // Assert
            Assert.AreEqual(maxRecords, budgetItemsByCategory.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Category, firstRecordTest.Category, "First Record Category OK");
            Assert.AreEqual(firstRecord.Total, firstRecordTest.Total, "First Record Total OK");
            Assert.AreEqual(firstRecord.Details.Count, firstRecordTest.Details.Count, "Number of Budget Items OK");
            for (int record = 0; record < firstRecord.Details.Count; record++)
            {
                BudgetItem validItem = firstRecord.Details[record];
                BudgetItem testItem = firstRecordTest.Details[record];
                Assert.AreEqual(validItem.Amount, testItem.Amount, "Budget item " + record + " amount is OK");
                Assert.AreEqual(validItem.CategoryID, testItem.CategoryID, "Budget item " + record + " category ID is OK");
                Assert.AreEqual(validItem.ExpenseID, testItem.ExpenseID, "Budget item " + record + " expense ID is OK");

            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItemsByCategory_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            int maxRecords14 = TestConstants.budgetItemsByCategory14;
            int maxRecords20 = TestConstants.budgetItemsByCategory20;

            // Act
            List<BudgetItemsByMonth> budgetItemsByCategory = homeBudget.GetBudgetItemsByMonth(null, null, true, 14);

            // Assert
            Assert.AreEqual(maxRecords14, budgetItemsByCategory.Count, "correct number of budget items for cat 14");


            // Act
            budgetItemsByCategory = homeBudget.GetBudgetItemsByMonth(null, null, true, 20);

            // Assert
            Assert.AreEqual(maxRecords20, budgetItemsByCategory.Count, "correct number of budget items for cat 20");

        }
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItemsByCategory_2018_filterDateAndCat9()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            List<BudgetItemsByCategory> validBudgetItemsByCategory = TestConstants.getBudgetItemsByCategory2018_Cat9();
            BudgetItemsByCategory firstRecord = validBudgetItemsByCategory[0];

            // Act
            List<BudgetItemsByCategory> budgetItemsByCategory = homeBudget.GeBudgetItemsByCategory(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), true, 9);
            BudgetItemsByCategory firstRecordTest = budgetItemsByCategory[0];

            // Assert
            Assert.AreEqual(validBudgetItemsByCategory.Count, budgetItemsByCategory.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Category, firstRecordTest.Category, "First Record Month OK");
            Assert.AreEqual(firstRecord.Total, firstRecordTest.Total, "First Record Total OK");
            Assert.AreEqual(firstRecord.Details.Count, firstRecordTest.Details.Count, "Number of Budget Items OK");
            for (int record = 0; record < firstRecord.Details.Count; record++)
            {
                BudgetItem validItem = firstRecord.Details[record];
                BudgetItem testItem = firstRecordTest.Details[record];
                Assert.AreEqual(validItem.Amount, testItem.Amount, "Budget item " + record + " amount is OK");
                Assert.AreEqual(validItem.CategoryID, testItem.CategoryID, "Budget item " + record + " category ID is OK");
                Assert.AreEqual(validItem.ExpenseID, testItem.ExpenseID, "Budget item " + record + " expense ID is OK");

            }
        }


        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItemsByCategory_2018_filterDate()
        {
            // Arrange
            string folder = GetSolutionDir();
            string inFile = GetSolutionDir() + "\\" + testInputFile;
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB, false);
            List<BudgetItemsByCategory> validBudgetItemsByCategory = TestConstants.getBudgetItemsByCategory2018();
            BudgetItemsByCategory firstRecord = validBudgetItemsByCategory[0];


            // Act
            List<BudgetItemsByCategory> budgetItemsByCategory = homeBudget.GeBudgetItemsByCategory(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 9);
            BudgetItemsByCategory firstRecordTest = budgetItemsByCategory[0];

            // Assert
            Assert.AreEqual(validBudgetItemsByCategory.Count, budgetItemsByCategory.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Category, firstRecordTest.Category, "First Record Month OK");
            Assert.AreEqual(firstRecord.Total, firstRecordTest.Total, "First Record Total OK");
            Assert.AreEqual(firstRecord.Details.Count, firstRecordTest.Details.Count, "Number of Budget Items OK");
            for (int record = 0; record < firstRecord.Details.Count; record++)
            {
                BudgetItem validItem = firstRecord.Details[record];
                BudgetItem testItem = firstRecordTest.Details[record];
                Assert.AreEqual(validItem.Amount, testItem.Amount, "Budget item " + record + " amount is OK");
                Assert.AreEqual(validItem.CategoryID, testItem.CategoryID, "Budget item " + record + " category ID is OK");
                Assert.AreEqual(validItem.ExpenseID, testItem.ExpenseID, "Budget item " + record + " expense ID is OK");

            }
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
    }
}

