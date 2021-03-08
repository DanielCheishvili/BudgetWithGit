using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;

namespace Budget
{
    [TestClass]
    public class TestHomeBudget_GetBudgetItems
    {
        string testInputFile = TestConstants.testExpensesInputFile;
        

        // ========================================================================
        // Get Expenses Method tests
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_NoStartEnd_NoFilter()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,false);
            List<Expense> listExpenses = homeBudget.expenses.List();
            List<Category> listCategories = homeBudget.categories.List();

            // Act
            List<BudgetItem> budgetItems =  homeBudget.GetBudgetItems(null,null,false,9);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseID == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.Category);
                Assert.AreEqual(budgetItem.Category, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryID, expense.Category, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ShortDescription, expense.Description, "Expense description ok");
            }
       }

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_NoStartEnd_NoFilter_VerifyBalanceProperty()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, false, 9);

            // Assert
            double balance = 0;
            foreach (BudgetItem budgetItem in budgetItems)
            {
                balance = balance + budgetItem.Amount;
                Assert.AreEqual(balance, budgetItem.Balance, "Balance for expense id ", budgetItem.ExpenseID, " is good");
            }

        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            int filterCategory = 9;
            List<Expense> listExpenses = TestConstants.filteredbyCat9();
            List<Category> listCategories = homeBudget.categories.List();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null, true, filterCategory);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseID == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.Category);
                Assert.AreEqual(budgetItem.Category, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryID, expense.Category, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ShortDescription, expense.Description, "Expense description ok");
            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_2018_filterDate()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            List<Expense> listExpenses = TestConstants.filteredbyYear2018();
            List<Category> listCategories = homeBudget.categories.List();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), false, 0);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseID == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.Category);
                Assert.AreEqual(budgetItem.Category, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryID, expense.Category, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ShortDescription, expense.Description, "Expense description ok");
            }
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_2018_filterDate_verifyBalance()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            List<Expense> listExpenses = TestConstants.filteredbyCat9();
            List<Category> listCategories = homeBudget.categories.List();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(null, null,  true, 9);
            double total = budgetItems[budgetItems.Count-1].Balance;
            

            // Assert
            Assert.AreEqual(TestConstants.filteredbyCat9Total, total,"budgetitem balance is correct");
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetItems_2018_filterDateAndCat10()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            List<Expense> listExpenses = TestConstants.filteredbyYear2018AndCategory10();
            List<Category> listCategories = homeBudget.categories.List();

            // Act
            List<BudgetItem> budgetItems = homeBudget.GetBudgetItems(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31), true, 10);

            // Assert
            Assert.AreEqual(listExpenses.Count, budgetItems.Count, "correct number of budget items");
            foreach (Expense expense in listExpenses)
            {
                BudgetItem budgetItem = budgetItems.Find(b => b.ExpenseID == expense.Id);
                Category category = listCategories.Find(c => c.Id == expense.Category);
                Assert.AreEqual(budgetItem.Category, category.Description, "Category description ok");
                Assert.AreEqual(budgetItem.CategoryID, expense.Category, "Category id is ok");
                Assert.AreEqual(budgetItem.Amount, expense.Amount, "Amount is ok");
                Assert.AreEqual(budgetItem.ShortDescription, expense.Description, "Expense description ok");
            }
        }
     
    }
}

