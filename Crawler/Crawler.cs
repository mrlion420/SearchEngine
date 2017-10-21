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
            Logger log = new Logger(Directory.GetCurrentDirectory() + @"\log.txt");
            try
            {
                string url = Directory.GetCurrentDirectory();
                string sql = string.Empty;
                string[] filePaths = Directory.GetFiles(url, "*.txt");
                Dictionary<string, string> wordDict = new Dictionary<string, string>();
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

                FileHelper fileHelper = new FileHelper();
                wordDict = fileHelper.ParseDocuments(filePaths, sqlConnection, wordDict);

                sqlConnection.Close();

            } catch (Exception ex)
            {
                log.write(ex.ToString());
            }

        }

        protected override void OnStop()
        {
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
                        documentName varchar not null
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
