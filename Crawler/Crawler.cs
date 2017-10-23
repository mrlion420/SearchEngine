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
            Thread textCrawlerThread = new Thread(TextFileCrawler);
            textCrawlerThread.Start();

            while (textCrawlerThread.IsAlive)
            {
                Thread.Sleep(2000);
            }
        }

        protected override void OnStop()
        {

        }

        private void TextFileCrawler()
        {
            Logger log = new Logger(Directory.GetCurrentDirectory() + @"\log.txt");
            //string url = Directory.GetCurrentDirectory();

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

                    foreach (string restrictedDirectory in restrictedDirectories)
                    {
                        topDirectories = topDirectories.Where((x) => !x.Equals(restrictedDirectory)).ToArray();
                    }

                    foreach (string directory in topDirectories)
                    {
                        // Catch the access denied errors.
                        // Crawler will move on to another directory if access denied error occurs.
                        try
                        {
                            string[] filePaths = Directory.GetFiles(directory, "*.txt", SearchOption.AllDirectories);
                            // Check if any text files exist or not
                            if (filePaths.Length > 0)
                            {
                                Dictionary<string, string> wordDict = new Dictionary<string, string>();
                                Dictionary<string, bool> isDocumentIdInsertedDict = new Dictionary<string, bool>();

                                SQLiteCommand command;
                                SQLiteConnection sqlConnection;

                                string dbName = Directory.GetCurrentDirectory() + @"\searchEngine.db";
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
                                wordDict = file.ParseDocuments(filePaths, sqlConnection, wordDict);

                                // Check if the dictionary is empty or not 
                                if (wordDict.Count > 0)
                                {
                                    // Start sql transaction
                                    using (SQLiteTransaction transaction = sqlConnection.BeginTransaction())
                                    {
                                        foreach (KeyValuePair<string, string> keyValuePair in wordDict)
                                        {
                                            string key = keyValuePair.Key;
                                            string value = keyValuePair.Value;

                                            sql = "insert into reverseIndex (term, position) values ('" + key + "','" + value + "')";
                                            command = new SQLiteCommand(sql, sqlConnection);
                                            command.ExecuteNonQuery();
                                        }
                                        transaction.Commit();
                                    }
                                    // Finally Close the sql connection
                                    sqlConnection.Close();
                                }

                            }

                        }
                        catch (UnauthorizedAccessException ex)
                        {
                            log.write(ex.ToString());
                        }
                    }
                }
            }

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
