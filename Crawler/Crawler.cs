using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crawler
{
    public partial class Crawler : ServiceBase
    {

        public Crawler()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Logger log = new Logger(Path.GetDirectoryName(Application.ExecutablePath) + @"\log.txt");
            List<string> fileTypes = new List<string>();
            fileTypes.Add("*.txt");
            //fileTypes.Add("*.html");

            try
            {
                
                foreach(string fileType in fileTypes)
                {
                    Thread thread = new Thread(() => FileCrawler(fileType));
                    thread.Start();
                }
                

            }catch(Exception ex)
            {
                log.write(ex.ToString());
            }
            
        }

        protected override void OnStop()
        {
            Logger log = new Logger(Path.GetDirectoryName(Application.ExecutablePath) + @"\log.txt");
            try
            {
                
            }
            catch (Exception ex)
            {
                log.write(ex.ToString());
            }
            
        }

        private void FileCrawler(string pattern)
        {
            Logger log = new Logger(Path.GetDirectoryName(Application.ExecutablePath) + @"\log.txt");
            try
            {
                DriveInfo[] logicalDriveInfoArray = DriveInfo.GetDrives();
                foreach (DriveInfo logicalDrive in logicalDriveInfoArray)
                {
                    DriveType driveType = logicalDrive.DriveType;
                    if (driveType == DriveType.Fixed)
                    {
                        string driveName = logicalDrive.Name;
                        string[] topDirectories = Directory.GetDirectories(logicalDrive.Name);
                        string sql = string.Empty;
                        string[] restrictedDirectories = new string[] { "C:\\Windows", "C:\\$Recycle.Bin", "C:\\Recovery", "C:\\inetpub", "C:\\Program Files", "C:\\Program Files (x86)", "C:\\ProgramData", "C:\\System Volume Information" };
                        string[] tempDirectories = new string[] { "AppData", "Local", "LocalLow", "Roaming", "tools_r25.2.3-windows" };

                        foreach (string restrictedDirectory in restrictedDirectories)
                        {
                            topDirectories = topDirectories.Where((x) => !x.Equals(restrictedDirectory)).ToArray();
                        }

                        foreach (string tempDirectory in tempDirectories)
                        {
                            topDirectories = topDirectories.Where((x) => !x.Contains(tempDirectory)).ToArray();
                        }

                        foreach (string directory in topDirectories)
                        {
                            // Catch the access denied errors.
                            // Crawler will move on to another directory if access denied error occurs.
                            List<string> filePaths = GetFiles(directory, pattern);
                            foreach (string tempdirectory in tempDirectories)
                            {
                                filePaths = filePaths.Where((x) => !x.Contains(tempdirectory)).ToList();
                            }
                            // Check if any text files exist or not
                            if (filePaths.Count > 0)
                            {
                                Dictionary<string, string> wordDict = new Dictionary<string, string>();
                                Dictionary<string, bool> isDocumentIdInsertedDict = new Dictionary<string, bool>();
                                SQLiteConnection sqlConnection;

                                string dbName = Path.GetDirectoryName(Application.ExecutablePath) + @"\searchEngine.db";
                                // Check if database exists or not
                                if (!File.Exists(dbName))
                                {
                                    sqlConnection = InitializeDatabase(dbName);
                                }
                                else
                                {
                                    // Connect to Database if database exists
                                    sqlConnection = new SQLiteConnection("DataSource=" + dbName);
                                    sqlConnection.Open();

                                }

                                FileHelper file = new FileHelper();
                                try
                                {
                                    wordDict = file.ParseDocuments(filePaths, sqlConnection, wordDict);
                                }
                                catch(Exception ex)
                                {
                                    log.write(ex.ToString());
                                }
                                
                                // Check if the dictionary is empty or not 
                                if (wordDict.Count > 0)
                                {
                                    // Start sql transaction
                                    InsertOrUpdateReverseIndex(sqlConnection, wordDict);
                                    // Finally Close the sql connection
                                    sqlConnection.Close();
                                }

                            }
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                log.write(ex.ToString());
            }
            

        }

        public void InsertOrUpdateReverseIndex(SQLiteConnection sqlConnection, Dictionary<string, string> wordDict)
        {
            Logger log = new Logger(Path.GetDirectoryName(Application.ExecutablePath) + @"\log.txt");

            using (SQLiteTransaction transaction = sqlConnection.BeginTransaction())
            {
                //int count = 1;
                foreach (KeyValuePair<string, string> keyValuePair in wordDict)
                {
                    string key = keyValuePair.Key;
                    string value = keyValuePair.Value;
                    string position = string.Empty;
                    string result = string.Empty;
                    key = RemoveWhitespace(key);
                    try
                    {
                        string sql = "select * from reverseIndex where term ='" + key + "'";

                        //log.write(count + " --> " + sql);

                        //count++;
                        SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                        SQLiteDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            position = reader.GetString(1);
                        }
                        if (key.Equals("this"))
                        {
                            log.write(key + "--->" + value);
                        }
                        
                        if (!string.IsNullOrEmpty(position))
                        {
                            result = position + ";" + value;
                            sql = "update reverseIndex set position ='" + result + "' where term ='" + key + "'";
                        }
                        else
                        {
                            result = value;
                            sql = "insert into reverseIndex (term, position) values ('" + key + "','" + result + "')";
                        }

                        command = new SQLiteCommand(sql, sqlConnection);
                        command.ExecuteNonQuery();
                    }
                    catch (Exception ex)
                    {
                        
                    }
                }
                transaction.Commit();
            }
            
        }

        private string RemoveWhitespace(string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
         
        private bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        private string RemoveUnicode(string input)
        {
            return System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(input));
        }

        private List<string> GetFiles(string path, string pattern)
        {
            var files = new List<string>();

            try
            {
                files.AddRange(Directory.GetFiles(path, pattern, SearchOption.TopDirectoryOnly));
                foreach (var directory in Directory.GetDirectories(path))
                    files.AddRange(GetFiles(directory, pattern));
            }
            catch (UnauthorizedAccessException) { }

            return files;
        }

        protected SQLiteConnection InitializeDatabase(string databaseName)
        {
            // Create new database 
            SQLiteConnection.CreateFile(databaseName);
            SQLiteConnection sqlConnection = new SQLiteConnection("DataSource=" + databaseName);
            SQLiteCommand command;

            sqlConnection.Open();
            // Create documents table
            string createTableSQL =
                    @"create table documents(
                        documentId integer not null primary key autoincrement,
                        documentName varchar not null,
                        totalWords integer not null
                    )";

            command = new SQLiteCommand(createTableSQL, sqlConnection);
            command.ExecuteNonQuery();
            // Create reverseIndex table
            createTableSQL =
                @"create table reverseIndex(
                    term varchar not null primary key,
                    position varchar not null
                    )";

            command = new SQLiteCommand(createTableSQL, sqlConnection);
            command.ExecuteNonQuery();

            return sqlConnection;
        }
    }
}
