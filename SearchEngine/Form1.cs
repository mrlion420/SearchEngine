using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEngine
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Thread textCrawlerThread = new Thread(TextFileCrawler);
            textCrawlerThread.Start();

            while (textCrawlerThread.IsAlive)
            {
                Thread.Sleep(2000);
            }

        }

        private void TextFileCrawler()
        {
            Logger log = new Logger(Directory.GetCurrentDirectory() + @"\log.txt");
            //string url = Directory.GetCurrentDirectory();
            string[] topDirectories = Directory.GetDirectories(@"C:\");
            string sql = string.Empty;
            string[] restrictedDirectories = new string[] { "C:\\Windows", "C:\\$Recycle.Bin" ,"C:\\Recovery", "C:\\inetpub" , "C:\\Program Files", "C:\\Program Files (x86)", "C:\\ProgramData" , "C:\\System Volume Information" };

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

                        wordDict = ParseDocuments(filePaths, sqlConnection, wordDict);

                        // Check if the dictionary is empty or not 
                        if(wordDict.Count > 0)
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

        public bool isIndexedInDatabase(string filePath, SQLiteConnection sqlConnection)
        {
            bool isIndexedInDatabase = false;
            string sql = "select documentId from documents where documentName = '" + filePath + "'";
            SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                isIndexedInDatabase = true;
            }

            return isIndexedInDatabase;
        }

        public Dictionary<string, string> ParseDocuments(string[] filePaths, SQLiteConnection sqlConnection, Dictionary<string, string> wordDict)
        {
            string[] stopWords = new string[] { ",", ".", ";", ":", "'", "\"", "\\", "/", "|", "_", "-", "(", ")" };
            string sql = string.Empty;
            SQLiteCommand command;

            foreach (string filePath in filePaths)
            {
                if (isIndexedInDatabase(filePath, sqlConnection))
                {
                    long documentId = 0;
                    HashSet<string> isDocumentIdInsertedForWord = new HashSet<string>();

                    string[] stringArray = File.ReadAllText(filePath).Split(' ');

                    sql = "insert into documents (documentName, totalWords) values ('" + filePath + "'," + stringArray.Length + ")";
                    command = new SQLiteCommand(sql, sqlConnection);
                    command.ExecuteNonQuery();

                    sql = "select documentId from documents where documentName = '" + filePath + "'";
                    command = new SQLiteCommand(sql, sqlConnection);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        documentId = Convert.ToInt64(reader["documentId"]);
                    }

                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        string resultString = string.Empty;

                        string word = stringArray[i].ToLower();
                        word = RemoveStopWords(word, stopWords);

                        if (wordDict.ContainsKey(word))
                        {
                            if (isDocumentIdInsertedForWord.Contains(word))
                            {
                                resultString = "," + i;
                            }
                            else
                            {
                                resultString = ";" + documentId + ":" + i;
                                isDocumentIdInsertedForWord.Add(word);
                            }
                            // += the resulting string
                            wordDict[word] += resultString;
                        }
                        else
                        {
                            resultString = documentId + ":" + i;
                            wordDict.Add(word, resultString);
                            isDocumentIdInsertedForWord.Add(word);

                        }

                    }
                }

            }

            return wordDict;
        }

        public string RemoveStopWords(string word, string[] stopWords)
        {
            foreach(string stopWord in stopWords)
            {
                if (word.Contains(stopWord))
                {
                    word = word.Replace(stopWord, "");
                }
            }

            return word;
        }
    }
}
