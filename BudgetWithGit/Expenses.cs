using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Data.SQLite;
using System.Globalization;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{
    /// <summary>
    /// A collection of expense items that saves and reads from a database file.
    /// as well as removing and adding expenses. 
    /// </summary

    public class Expenses
    {
 
        private SQLiteConnection dbConnection;
        /// <summary>
        ///Constructor checks if the database is on.
        /// </summary>
        /// <param name="conn">The database connection</param>
        public Expenses(SQLiteConnection conn)
        {
            if(conn == null)
            {
                throw new Exception("No connection to database");
            }
            this.dbConnection = conn;
            

        }

        /// <summary>
        /// Adds expenses to the exepnses table.
        /// </summary>
        /// 
        /// <param name="date">Date due of the expense</param>
        /// <param name="category">Category of the expense</param>
        /// <param name="amount">Amount due of the expnese</param>
        /// <param name="description">Description of the expanse</param>
        public void Add(DateTime date, int category, Double amount, String description)
        {
            var cmd = new SQLiteCommand(this.dbConnection);

            cmd.CommandText = "INSERT INTO expenses(Date , Description , Amount , CategoryId) VALUES (@Date , @Description , @Amount , @CategoryId)";
            cmd.Parameters.AddWithValue("@Date", date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@Description", description);
            cmd.Parameters.AddWithValue("@Amount", amount);
            cmd.Parameters.AddWithValue("@CategoryId", category);

            cmd.Prepare();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// Deletes the expanses from the exepenses table.
        /// </summary>
        /// <param name="Id">Id of the existing expense</param>
        public void Delete(int Id)
        {
            try
            {

                SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);

                cmd.CommandText = "DELETE FROM expenses WHERE id = @Id";
                cmd.Parameters.AddWithValue("@id", Id);
                cmd.Prepare();
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Not allowed to delete in database (foreign key constraint)", e.Message);
            }


        }

        /// <summary>
        /// Retrieves all the columns of the expenses table.
        /// </summary>
        /// 
        /// <returns>The list of expenses</returns>
        public List<Expense> List()
        {
            string selectCategory = "select Id, Date, Amount, Description, CategoryId from expenses ORDER BY id ASC;";


            SQLiteCommand cmd = new SQLiteCommand(selectCategory, this.dbConnection);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            List<Expense> newList = new List<Expense>();
            while (rdr.Read())
            {
                newList.Add(new Expense(rdr.GetInt32(0), DateTime.ParseExact(rdr.GetString(1), "yyyy-MM-dd", CultureInfo.InvariantCulture), rdr.GetInt32(4), rdr.GetDouble(2),rdr.GetString(3)));
            }
            
            return newList;
        }
        /// <summary>
        /// Finds a specific expense from the table where the id is the one that is specified using SQL queries.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Expense GetExpenseFromId(int i )
        {
            string selectID = $"SELECT id,Date, Amount,Description,CategoryId FROM expenses WHERE id = @id";
            SQLiteCommand cmd = new SQLiteCommand(selectID, this.dbConnection);
            cmd.Parameters.AddWithValue("@id", i);
            cmd.Prepare();

            SQLiteDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            Expense expense = new Expense(rdr.GetInt32(0), DateTime.ParseExact(rdr.GetString(1), "yyyy-MM-dd", CultureInfo.InvariantCulture), rdr.GetInt32(4), rdr.GetDouble(2), rdr.GetString(3));
            rdr.Close();

            return expense;
        }

        /// <summary>
        /// Updates the properties of the expenes table using SQL queries.Id is not updated
        /// </summary>
        /// <param name="id">The id of the expense</param>
        /// <param name="newDate">The date to be updated</param>
        /// <param name="newCategory">The category to be updated</param>
        /// <param name="newAmount">The amount to be updated</param>
        /// <param name="newDescription">The description to be updated</param>
        /// <returns>The updated expenses table</returns>
        public Expense UpdateProperties(int id, DateTime newDate, int newCategory, Double newAmount, String newDescription)
        {
            Expense expUpdate = GetExpenseFromId(id);
            expUpdate.Date = newDate;
            expUpdate.Category = newCategory;
            expUpdate.Amount = newAmount;
            expUpdate.Description = newDescription;

            SQLiteCommand cmd = new SQLiteCommand(this.dbConnection);
            cmd.CommandText = "UPDATE expenses SET Date = @date, CategoryId = @cat, Amount = @amt, Description = @desc where id = @id";
            cmd.Parameters.AddWithValue("@date", expUpdate.Date.ToString("yyyy-MM-dd"));
            cmd.Parameters.AddWithValue("@cat", expUpdate.Category);
            cmd.Parameters.AddWithValue("@amt", expUpdate.Amount);
            cmd.Parameters.AddWithValue("@desc", expUpdate.Description);
            cmd.Parameters.AddWithValue("@id", expUpdate.Id);
            cmd.Prepare();
            cmd.ExecuteNonQuery();
            return expUpdate;
        }

    }
}

