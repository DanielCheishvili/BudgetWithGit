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
        //private static String DefaultFileName = "budget.txt";
        private List<Expense> _Expenses = new List<Expense>();
        private string _FileName;
        private string _DirName;
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
        /// Gets the filename.
        /// </summary>
        public String FileName { get { return _FileName; } }

        /// <summary>
        /// Gets the directory name.
        /// </summary>
        public String DirName { get { return _DirName; } }

        /// <summary>
        /// populate exepnses from a file. Resets the current expenses.
        /// </summary>
        /// 
        /// <param name="filepath"> A file path that the user will specifiy but originally set to null</param>
        public void ReadFromFile(String filepath = null)
        {

            // ---------------------------------------------------------------
            // reading from file resets all the current expenses,
            // so clear out any old definitions
            // ---------------------------------------------------------------
            _Expenses.Clear();

            // ---------------------------------------------------------------
            // reset default dir/filename to null 
            // ... filepath may not be valid, 
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyReadFromFileName(filepath/*, DefaultFileName*/);

            // ---------------------------------------------------------------
            // read the expenses from the xml file
            // ---------------------------------------------------------------
            _ReadXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use?
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);


        }

        /// <summary>
        /// save to a file. saves the file in an XML format.
        /// </summary>
        /// 
        /// <param name="filepath">A file path that the user will specifiy but originally set to null</param>
        public void SaveToFile(String filepath = null)
        {
            // ---------------------------------------------------------------
            // if file path not specified, set to last read file
            // ---------------------------------------------------------------
            if (filepath == null && DirName != null && FileName != null)
            {
                filepath = DirName + "\\" + FileName;
            }

            // ---------------------------------------------------------------
            // just in case filepath doesn't exist, reset path info
            // ---------------------------------------------------------------
            _DirName = null;
            _FileName = null;

            // ---------------------------------------------------------------
            // get filepath name (throws exception if it doesn't exist)
            // ---------------------------------------------------------------
            filepath = BudgetFiles.VerifyWriteToFileName(filepath/*, DefaultFileName*/);

            // ---------------------------------------------------------------
            // save as XML
            // ---------------------------------------------------------------
            _WriteXMLFile(filepath);

            // ----------------------------------------------------------------
            // save filename info for later use
            // ----------------------------------------------------------------
            _DirName = Path.GetDirectoryName(filepath);
            _FileName = Path.GetFileName(filepath);
        }



        // ====================================================================
        // Add expense
        // ====================================================================
        private void Add(Expense exp)
        {
            _Expenses.Add(exp);
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


        // ====================================================================
        // read from an XML file and add categories to our categories list
        // ====================================================================
        private void _ReadXMLFile(String filepath)
        {


            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(filepath);

                // Loop over each Expense
                foreach (XmlNode expense in doc.DocumentElement.ChildNodes)
                {
                    // set default expense parameters
                    int id = int.Parse((((XmlElement)expense).GetAttributeNode("ID")).InnerText);
                    String description = "";
                    DateTime date = DateTime.Parse("2000-01-01");
                    int category = 0;
                    Double amount = 0.0;

                    // get expense parameters
                    foreach (XmlNode info in expense.ChildNodes)
                    {
                        switch (info.Name)
                        {
                            case "Date":
                                date = DateTime.Parse(info.InnerText);
                                break;
                            case "Amount":
                                amount = Double.Parse(info.InnerText);
                                break;
                            case "Description":
                                description = info.InnerText;
                                break;
                            case "Category":
                                category = int.Parse(info.InnerText);
                                break;
                        }
                    }

                    // have all info for expense, so create new one
                    this.Add(new Expense(id, date, category, amount, description));

                }

            }
            catch (Exception e)
            {
                throw new Exception("ReadFromFileException: Reading XML " + e.Message);
            }
        }


        // ====================================================================
        // write to an XML file
        // if filepath is not specified, read/save in AppData file
        // ====================================================================
        private void _WriteXMLFile(String filepath)
        {
            // ---------------------------------------------------------------
            // loop over all categories and write them out as XML
            // ---------------------------------------------------------------
            try
            {
                // create top level element of expenses
                XmlDocument doc = new XmlDocument();
                doc.LoadXml("<Expenses></Expenses>");

                // foreach Category, create an new xml element
                foreach (Expense exp in _Expenses)
                {
                    // main element 'Expense' with attribute ID
                    XmlElement ele = doc.CreateElement("Expense");
                    XmlAttribute attr = doc.CreateAttribute("ID");
                    attr.Value = exp.Id.ToString();
                    ele.SetAttributeNode(attr);
                    doc.DocumentElement.AppendChild(ele);

                    // child attributes (date, description, amount, category)
                    XmlElement d = doc.CreateElement("Date");
                    XmlText dText = doc.CreateTextNode(exp.Date.ToString());
                    ele.AppendChild(d);
                    d.AppendChild(dText);

                    XmlElement de = doc.CreateElement("Description");
                    XmlText deText = doc.CreateTextNode(exp.Description);
                    ele.AppendChild(de);
                    de.AppendChild(deText);

                    XmlElement a = doc.CreateElement("Amount");
                    XmlText aText = doc.CreateTextNode(exp.Amount.ToString());
                    ele.AppendChild(a);
                    a.AppendChild(aText);

                    XmlElement c = doc.CreateElement("Category");
                    XmlText cText = doc.CreateTextNode(exp.Category.ToString());
                    ele.AppendChild(c);
                    c.AppendChild(cText);

                }

                // write the xml to FilePath
                doc.Save(filepath);

            }
            catch (Exception e)
            {
                throw new Exception("SaveToFileException: Reading XML " + e.Message);
            }
        }

    }
}

