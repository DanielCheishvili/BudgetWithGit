using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Dynamic;

namespace Budget
{
    [TestClass]
    public class TestHomeBudget_GetBudgetDictionaryByCategoryAndMonth
    {
        string testInputFile = TestConstants.testExpensesInputFile;



        // ========================================================================
        // Get Expenses By Month Method tests
        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyNumberOfRecords()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB, false);

            int maxRecords = TestConstants.budgetItemsByCategoryAndMonth_MaxRecords;
            Dictionary<string, object> firstRecord = TestConstants.getBudgetItemsByCategoryAndMonthFirstRecord();

            // Act
            List<Dictionary<string, object>> budgetItemsByCategoryAndMonth = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);

            // Assert
            Assert.AreEqual(maxRecords+1,budgetItemsByCategoryAndMonth.Count,"All records plus TOTALS are accounted for");
            Database.CloseDatabaseAndReleaseFile();
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyFirstRecord()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);

            int maxRecords = TestConstants.budgetItemsByCategoryAndMonth_MaxRecords; 
            Dictionary<string,object> firstRecord = TestConstants.getBudgetItemsByCategoryAndMonthFirstRecord();

            // Act
            List<Dictionary<string,object>> budgetItemsByCategoryAndMonth = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);
            Dictionary<string,object> firstRecordTest = budgetItemsByCategoryAndMonth[0];

            // Assert
            Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(firstRecord,firstRecordTest));
            Database.CloseDatabaseAndReleaseFile();
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_NoFilter_VerifyTotalsRecord()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB, false);

            int maxRecords = TestConstants.budgetItemsByCategoryAndMonth_MaxRecords;
            Dictionary<string, object> totalsRecord = TestConstants.getBudgetItemsByCategoryAndMonthTotalsRecord();

            // Act
            List<Dictionary<string, object>> budgetItemsByCategoryAndMonth = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, false, 9);
            Dictionary<string, object> totalsRecordTest = budgetItemsByCategoryAndMonth[budgetItemsByCategoryAndMonth.Count - 1];

            // Assert
            // ... loop over all key/value pairs 
            Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(totalsRecord, totalsRecordTest), "Totals Record is Valid");
            Database.CloseDatabaseAndReleaseFile();
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_NoStartEnd_FilterbyCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB,  false);
            List<Dictionary<string, object>> expectedResults =TestConstants.getBudgetItemsByCategoryAndMonthCat10();

            // Act
            List<Dictionary<string, object>> gotResults = homeBudget.GetBudgetDictionaryByCategoryAndMonth(null, null, true, 10);

            // Assert
            Assert.AreEqual(expectedResults.Count, gotResults.Count, "correct number of budget items for cat 10");
            for (int record = 0; record < expectedResults.Count; record++)
            {
                Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(expectedResults[record],
                    gotResults[record]), "Record:" + record + " is Valid");

            }
            Database.CloseDatabaseAndReleaseFile();
        }

        // ========================================================================

        [TestMethod]
        public void HomeBudgetMethod_GetBudgetDictionaryByCategoryAndMonth_2020()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            HomeBudget homeBudget = new HomeBudget(messyDB, false);
            List<Dictionary<string, object>> expectedResults = TestConstants.getBudgetItemsByCategoryAndMonth2020();

            // Act
            List<Dictionary<string, object>> gotResults = homeBudget.GetBudgetDictionaryByCategoryAndMonth(new DateTime(2020,1,1), new DateTime(2020,12,31), false, 10);

            // Assert
            Assert.AreEqual(expectedResults.Count, gotResults.Count, "correct number of budget items for cat 10");
            for (int record = 0; record < expectedResults.Count; record++)
            {
                Assert.IsTrue(AssertDictionaryForExpenseByCategoryAndMonthIsOK(expectedResults[record],
                    gotResults[record]), "Record:" + record + " is Valid");

            }
            Database.CloseDatabaseAndReleaseFile();
        }




        // ========================================================================

        // -------------------------------------------------------
        // helpful functions, ... they are not tests
        // -------------------------------------------------------

        Boolean AssertDictionaryForExpenseByCategoryAndMonthIsOK(Dictionary<string,object> recordExpeted, Dictionary<string,object> recordGot)
        {
            try
            {
                foreach (var kvp in recordExpeted)
                {
                    String key = kvp.Key as String;
                    Object recordExpectedValue = kvp.Value;
                    Object recordGotValue = recordGot[key];


                    // ... validate the budget items
                    if (recordExpectedValue != null && recordExpectedValue.GetType() == typeof(List<BudgetItem>))
                    {
                        List<BudgetItem> expectedItems = recordExpectedValue as List<BudgetItem>;
                        List<BudgetItem> gotItems = recordGotValue as List<BudgetItem>;
                        for (int budgetItemNumber = 0; budgetItemNumber < expectedItems.Count; budgetItemNumber++)
                        {
                            Assert.AreEqual(expectedItems[budgetItemNumber].Amount, gotItems[budgetItemNumber].Amount,
                                "Item:" + budgetItemNumber + " key:" + kvp.Key + ", Amount ok");
                            Assert.AreEqual(expectedItems[budgetItemNumber].CategoryID, gotItems[budgetItemNumber].CategoryID,
                                "Item:" + budgetItemNumber + " key:" + kvp.Key + ", Category ID ok");
                            Assert.AreEqual(expectedItems[budgetItemNumber].ExpenseID, gotItems[budgetItemNumber].ExpenseID,
                                "Item:" + budgetItemNumber + " key:" + kvp.Key + ", Expense ID ok");
                        }
                    }

                    // else ... validate the value for the specified key
                    else
                    {
                        Assert.AreEqual(recordExpectedValue, recordGotValue, "Key:" + key + " is OK");
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }


    }
}

