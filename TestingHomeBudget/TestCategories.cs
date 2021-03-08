using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Budget;
using System.Data.SQLite;

namespace Budget
{
    [TestClass]
    public class TestCategories
    {
        public int numberOfCategoriesInFile = TestConstants.numberOfCategoriesInFile;
        public String testInputFile = TestConstants.testDBInputFile;
        public int maxIDInCategoryInFile = TestConstants.maxIDInCategoryInFile;
        Category firstCategoryInFile = TestConstants.firstCategoryInFile;
        int IDWithSaveType = TestConstants.CategoryIDWithSaveType;

        // ========================================================================

        [TestMethod]
        public void CategoriesObject_New()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Categories categories = new Categories(conn, true);

            // Assert 
            Assert.IsInstanceOfType(categories,typeof(Categories));
            Database.CloseDatabaseAndReleaseFile();


        }

        // ========================================================================

        [TestMethod]
        public void CategoriesObject_New_CreatesDefaultCategories()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Categories categories = new Categories(conn, true);

            // Assert 
            Assert.IsFalse(categories.List().Count == 0, "Non zero categories");
            Database.CloseDatabaseAndReleaseFile();


        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_ReadFromDatabase_ValidateCorrectDataWasRead()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String existingDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.openExistingDatabase(existingDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Categories categories = new Categories(conn, false);
            List<Category> list = categories.List();
            Category firstCategory = list[0];

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile, list.Count, "Number of list elements are correct");
            Assert.AreEqual(firstCategoryInFile.Id, firstCategory.Id, "ID of first element");
            Assert.AreEqual(firstCategoryInFile.Description, firstCategory.Description, "Description of first Element");
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_List_ReturnsListOfCategories()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.openExistingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);

            // Act
            List<Category> list = categories.List();

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile, list.Count);
            Database.CloseDatabaseAndReleaseFile();

        }


        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_Add()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            string descr = "New Category";
            Category.CategoryType type = Category.CategoryType.Income;

            // Act
            categories.Add(descr,type);
            List<Category> categoriesList = categories.List();
            int sizeOfList = categories.List().Count;

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile + 1, sizeOfList, "List size incremented");
            Assert.AreEqual(descr, categoriesList[sizeOfList - 1].Description, "Description property set correctly");
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_Delete()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messy.db";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            int IdToDelete = 3;

            // Act
            categories.Delete(IdToDelete);
            List<Category> categoriesList = categories.List();
            int sizeOfList = categoriesList.Count;

            // Assert
            Assert.AreEqual(numberOfCategoriesInFile - 1, sizeOfList, "List size decremented");
            Assert.IsFalse(categoriesList.Exists(e => e.Id == IdToDelete), "correct Category item deleted");
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_Delete_InvalidIDDoesntCrash()
        {
            // Arrange
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String goodDB = $"{folder}\\{TestConstants.testDBInputFile}";
            String messyDB = $"{folder}\\messyDB";
            System.IO.File.Copy(goodDB, messyDB, true);
            Database.openExistingDatabase(messyDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            int IdToDelete = 9999;
            int sizeOfList = categories.List().Count;

            // Act
            try
            {
                categories.Delete(IdToDelete);
                Assert.AreEqual(sizeOfList, categories.List().Count, "No Category was removed from list");
            }

            // Assert
            catch
            {
                Assert.IsTrue(false, "Invalid ID causes Delete to break");
            }
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_GetCategoryFromId()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\{TestConstants.testDBInputFile}";
            Database.openExistingDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, false);
            int catID = 15;

            // Act
            Category category = categories.GetCategoryFromId(catID);

            // Assert
            Assert.AreEqual(catID,category.Id);
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_SetCategoriesToDefaults()
        {

            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;

            // Act
            Categories categories = new Categories(conn, true);
            List<Category> originalList = categories.List();

            // modify list of categories
            categories.Delete(1);
            categories.Delete(2);
            categories.Delete(3);
            categories.Add("Another one ", Category.CategoryType.Credit);

            //"just double check that initial conditions are correct");
            Assert.AreNotEqual(originalList.Count, categories.List().Count, "unequal list sizes");

            // Act
            categories.SetCategoriesToDefaults();

            // Assert
            Assert.AreEqual(originalList.Count, categories.List().Count);
            foreach (Category defaultCat in originalList)
            {
                Assert.IsTrue(categories.List().Exists(c => c.Description == defaultCat.Description && c.Type == defaultCat.Type));
            }
            Database.CloseDatabaseAndReleaseFile();

        }

        // ========================================================================

        [TestMethod]
        public void CategoriesMethod_UpdateCategory()
        {
            // Arrange
            String folder = TestConstants.GetSolutionDir();
            String newDB = $"{folder}\\newDB.db";
            Database.newDatabase(newDB);
            SQLiteConnection conn = Database.dbConnection;
            Categories categories = new Categories(conn, true);
            String newDescr = "Presents";
            int id = 11;

            // Act
            categories.UpdateProperties(id,newDescr, Category.CategoryType.Income);
            Category category = categories.GetCategoryFromId(id);

            // Assert 
            Assert.AreEqual(newDescr, category.Description);
            Assert.AreEqual(Category.CategoryType.Income, category.Type);
            Database.CloseDatabaseAndReleaseFile();

        }

    }
}

