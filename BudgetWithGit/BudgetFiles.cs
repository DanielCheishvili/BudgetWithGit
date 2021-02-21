using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

// ============================================================================
// (c) Sandy Bultena 2018
// * Released under the GNU General Public License
// ============================================================================

namespace Budget
{

    /// <summary>
    /// A verification process that determines whether it is possible to write and read to/from a file.
    /// a default path is specified if no file path is provided and will be stored in the 
    /// specified file path "USERPROFILE\Appdata\Local\Budget\.
    /// </summary>
    public class BudgetFiles
    {
        private static String DefaultSavePath = @"Budget\";
        private static String DefaultAppData = @"%USERPROFILE%\AppData\Local\";//explain quickly

        /// <summary>
        /// Checks if there is an existing file path to read from. If no file path exists
        /// it would use the default file location which is specified.
        /// It also checks if the file path that you enter exists. 
        /// </summary>
        /// 
        /// <exception cref="FileNotFoundException">Thrown when file path not found.</exception>
        /// 
        /// <param name="FilePath">The file path specified by the user</param>
        /// <param name="DefaultFileName">The default file path if no file path is specified</param>
        /// 
        /// <returns>The full file path </returns>
        public static String VerifyReadFromFileName(String FilePath/*, String DefaultFileName*/)
        {

            // ---------------------------------------------------------------
            // if file path is not defined, use the default one in AppData
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                throw new FileNotFoundException("file path is not defined.");
                //FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
            }

            // ---------------------------------------------------------------
            // does FilePath exist?
            // ---------------------------------------------------------------
            if (!File.Exists(FilePath))
            {
                throw new FileNotFoundException("ReadFromFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ----------------------------------------------------------------
            // valid path
            // ----------------------------------------------------------------
            return FilePath;

        }

        /// <summary>
        /// Checks if there is an existing file path to write to. If no file path exists and or directory
        /// it would use the default file and directory location which is specified.
        /// It also checks if the file path that you enter exists and or if it readable,
        /// meaning if its read only or not.
        /// </summary>
        /// 
        /// <exception cref="Exception">Thrown when directory does not exist</exception>
        /// <exception cref="Exception">Thrown if the file is read only</exception>
        /// 
        /// <param name="FilePath">File path specified by the user</param>
        /// <param name="DefaultFileName">The default file path if no file path is specified</param>
        /// 
        /// <returns>The full file path </returns>
        public static String VerifyWriteToFileName(String FilePath/*, String DefaultFileName*/)
        {
            // ---------------------------------------------------------------
            // if the directory for the path was not specified, then use standard application data
            // directory
            // ---------------------------------------------------------------
            if (FilePath == null)
            {
                /* // create the default appdata directory if it does not already exist
                 String tmp = Environment.ExpandEnvironmentVariables(DefaultAppData);
                 if (!Directory.Exists(tmp))
                 {
                     Directory.CreateDirectory(tmp);
                 }

                 // create the default Budget directory in the appdirectory if it does not already exist
                 tmp = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath);
                 if (!Directory.Exists(tmp))
                 {
                     Directory.CreateDirectory(tmp);
                 }
                 Database.newDatabase(FilePath);*/
                //FilePath = Environment.ExpandEnvironmentVariables(DefaultAppData + DefaultSavePath + DefaultFileName);
                throw new FileNotFoundException("file path is not defined.");
            }




            // ---------------------------------------------------------------
            // does directory where you want to save the file exist?
            // ... this is possible if the user is specifying the file path
            // ---------------------------------------------------------------
            String folder = Path.GetDirectoryName(FilePath);
            String delme = Path.GetFullPath(FilePath);
            if (!Directory.Exists(folder))
            {
                throw new Exception("SaveToFileException: FilePath (" + FilePath + ") does not exist");
            }

            // ---------------------------------------------------------------
            // can we write to it?
            // ---------------------------------------------------------------
            if (File.Exists(FilePath))
            {
                FileAttributes fileAttr = File.GetAttributes(FilePath);
                if ((fileAttr & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    throw new Exception("SaveToFileException:  FilePath(" + FilePath + ") is read only");
                }
            }

            // ---------------------------------------------------------------
            // valid file path
            // ---------------------------------------------------------------
            return FilePath;

        }



    }
}
