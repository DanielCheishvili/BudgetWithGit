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
    /// A collection of exepnses items that also reads / write to a file 
    /// and saves a file as well as removing and adding exenses. For files, it uses
    /// the BudgetFiles.cs in order to save and write to file.
    /// </summary>
    public class Expenses
    {
 
        private SQLiteConnection dbConnection;
        public Expenses(SQLiteConnection conn)
        {
            if(conn == null)
            {
                throw new Exception("No connection to database");
            }
            this.dbConnection = conn;
            

        }

        /// <summary>
        /// Adds expenses to the exepnses list.
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
        /// Deletes the expanses from the exepenses list.
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
       /// Adds the expenses to the list of expenses.
       /// </summary>
       /// 
       /// <returns>The list of expenses</returns>
        public List<Expense> List()
        {
            string selectCategory = "select * from expenses ORDER BY id ASC;";


            SQLiteCommand cmd = new SQLiteCommand(selectCategory, this.dbConnection);
            SQLiteDataReader rdr = cmd.ExecuteReader();

            List<Expense> newList = new List<Expense>();
            while (rdr.Read())
            {
                newList.Add(new Expense(rdr.GetInt32(0), DateTime.ParseExact(rdr.GetString(1), "yyyy-MM-dd", CultureInfo.InvariantCulture), rdr.GetInt32(4), rdr.GetDouble(2),rdr.GetString(3)));
            }
            
            return newList;
        }

    }
}

