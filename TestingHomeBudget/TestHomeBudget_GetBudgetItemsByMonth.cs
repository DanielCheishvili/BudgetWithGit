﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;

namespace Budget
{
    [TestClass]
    public class TestHomeBudget_GetBudgetItemsByMonth
    {
        string testInputFile = TestConstants.testExpensesInputFile;
        


        // ========================================================================
        // Get Expenses By Month Method tests
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItemsByMonth_NoStartEnd_NoFilter()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            int maxRecords = TestConstants.budgetItemsByMonth_MaxRecords;
            BudgetItemsByMonth firstRecord = TestConstants.budgetItemsByMonth_FirstRecord;

            // Act
            List<BudgetItemsByMonth> budgetItemsByMonth = homeBudget.GetBudgetItemsByMonth(null, null, false, 9);
            BudgetItemsByMonth firstRecordTest = budgetItemsByMonth[0];

            // Assert
            Assert.AreEqual(maxRecords, budgetItemsByMonth.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Month, firstRecordTest.Month, "First Record Month OK");
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
        public void HomeBudgetMethod_GetBudgetItemsByMonth_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            int maxRecords = TestConstants.budgetItemsByMonth_FilteredByCat9_number;
            BudgetItemsByMonth firstRecord = TestConstants.budgetItemsByMonth_FirstRecord_FilteredCat9;

            // Act
            List<BudgetItemsByMonth> budgetItemsByMonth = homeBudget.GetBudgetItemsByMonth(null, null, true, 9);
            BudgetItemsByMonth firstRecordTest = budgetItemsByMonth[0];

            // Assert
            Assert.AreEqual(maxRecords, budgetItemsByMonth.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Month, firstRecordTest.Month, "First Record Month OK");
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
        public void HomeBudgetMethod_GetBudgetItemsByMonth_2018_filterDateAndCat9()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);

            List<Expense> listExpenses = TestConstants.filteredbyYear2018();
            List<Category> listCategories = homeBudget.categories.List();
            List<BudgetItemsByMonth> validBudgetItemsByMonth = TestConstants.getBudgetItemsBy2018_01_filteredByCat9();
            BudgetItemsByMonth firstRecord = TestConstants.budgetItemsByMonth_FirstRecord_FilteredCat9;

            // Act
            List<BudgetItemsByMonth> budgetItemsByMonth = homeBudget.GetBudgetItemsByMonth(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), true, 9);
            BudgetItemsByMonth firstRecordTest = budgetItemsByMonth[0];

            // Assert
            Assert.AreEqual(validBudgetItemsByMonth.Count, budgetItemsByMonth.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Month, firstRecordTest.Month, "First Record Month OK");
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
        public void HomeBudgetMethod_GetBudgetItemsByMonth_2018_filterDate()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            HomeBudget homeBudget = new HomeBudget(messyDB, false);

            List<BudgetItemsByMonth> validBudgetItemsByMonth = TestConstants.getBudgetItemsBy2018_01();
            BudgetItemsByMonth firstRecord = validBudgetItemsByMonth[0];


            // Act
            List<BudgetItemsByMonth> budgetItemsByMonth = homeBudget.GetBudgetItemsByMonth(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 9);
            BudgetItemsByMonth firstRecordTest = budgetItemsByMonth[0];

            // Assert
            Assert.AreEqual(validBudgetItemsByMonth.Count, budgetItemsByMonth.Count, "correct number of budget items");

            // verify 1st record
            Assert.AreEqual(firstRecord.Month, firstRecordTest.Month, "First Record Month OK");
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

    }
}

