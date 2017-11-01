using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace SearchEngine
{
    public partial class Form1 : Form
    {
        private string selectedPath = string.Empty;
        private List<string> fileTypes = new List<string>();
        private Logger log = new Logger(Path.GetDirectoryName(Application.ExecutablePath) + @"\log.txt");
        private int totalFileCount = 0;
        private double lastCount = 0;

        private List<Dictionary<double, double>> cosineDict = new List<Dictionary<double, double>>();
        private string wordToBeRemoved = string.Empty;
        private List<string> exactWordList = new List<string>();

        AutoResetEvent resetEvent = new AutoResetEvent(false);
        
        public Form1()
        {
            InitializeComponent();
        }

        // Method with Thread locker
        private void UpdateText(string message)
        {
            resetEvent.WaitOne();
            UpdateEventTextBox(message);
            resetEvent.Set();
        }

        public void UpdateEventTextBox(string message)
        {
            Invoke(new Action(() =>
            {
                eventTxtBx.Text += message + Environment.NewLine;
            }));
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            fileTypes.Add("*.txt");
            fileTypes.Add("*.html");
            fileTypes.Add("*.cs");

            string dbName = Path.GetDirectoryName(Application.ExecutablePath) + @"\searchEngine.db";
            // Check if database exists or not
            if (!File.Exists(dbName))
            {
                InitializeDatabase(dbName);
            }
            //CheckForIllegalCrossThreadCalls = false;
        }

        public void UpdateProgressBar()
        {
            Invoke(new Action(() =>
            {
                lastCount++;
                double result = ((lastCount) / totalFileCount) * 100;
                progressBar.Value = Convert.ToInt32(result);
            }));
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchKeyWord = searchTxtBx.Text;
            exactWordList = GetAndRemoveExactWordList(ref searchKeyWord);
            wordToBeRemoved = GetAndRemoveWordToBeRemoved(ref searchKeyWord);

            List<List<string>> scoreList = findKeyword(searchKeyWord);
            cosineDict = calculateVectorSpace(scoreList);
            BindComboxBox(scoreList);
            //SortAndBindData(cosineDict, 0, wordToBeRemoved, exactWordList);

            string li = "";
            foreach (List<string> scl in scoreList)
            {
                foreach (string a in scl)
                {
                    li = li + a + "***";
                }
                li = li + "---";
            }
            //textBox2.Text = li;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
            {
                selectedPath = folderBrowserDialog.SelectedPath;
                crawlLocationTxtbx.Text = selectedPath;
            }
        }

        private void btnCrawl_Click(object sender, EventArgs e)
        {
            UpdateEventTextBox("Crawl Location - " + selectedPath);
            foreach(string fileType in fileTypes)
            {
                List<string> filePaths = GetFiles(selectedPath, fileType);
                totalFileCount += filePaths.Count;
            }
            
            foreach (string fileType in fileTypes)
            {
                Thread thread = new Thread(() => FileCrawler(fileType));
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void searchQueryComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SortAndBindData(cosineDict, searchQueryComboBox.SelectedIndex, wordToBeRemoved, exactWordList);
        }

        #region Utility Methods

        private void FileCrawler(string fileType)
        {
            string text = "Started Crawling for file type -> " + fileType;
            resetEvent.Set();
            UpdateText(text);
            
            List<string> filePaths = GetFiles(selectedPath, fileType);
            if (filePaths.Count > 0)
            {
                CrawlData(filePaths, fileType);
            }

            text = "Crawling for " + fileType + " has finished.";
            resetEvent.Set();
            UpdateText(text);
        }

        private List<List<string>> findKeyword(string word)
        {
            List<List<string>> tfIdfScoreList = new List<List<string>>();
            List<string> seperatedScore;
            try
            {
                string dbName = Directory.GetCurrentDirectory() + @"\searchEngine.db";
                SQLiteConnection sqlConnection = new SQLiteConnection("DataSource=" + dbName);
                sqlConnection.Open();

                string sql1 = @"select * from documents";
                SQLiteCommand command1 = new SQLiteCommand(sql1, sqlConnection);
                SQLiteDataReader reader1 = command1.ExecuteReader();

                List<List<int>> allDocList = new List<List<int>>();
                int totalWords = 0;
                int di = 0;
                int noOfTotalDoc = 0;
                while (reader1.Read())
                {
                    List<int> allDocIdList = new List<int>();
                    totalWords = reader1.GetInt32(0);
                    di = reader1.GetInt32(2);
                    allDocIdList.Add(di);
                    allDocIdList.Add(totalWords);
                    allDocList.Add(allDocIdList);
                    noOfTotalDoc++;
                }

                string[] queryArr = getOrQuery(word);

                for (int x = 0; x < queryArr.Length; x++)
                {
                    seperatedScore = new List<string>();
                    string[] orgWords = queryArr[x].Split(' ');
                    string[] words = removeSpaceItem(orgWords);
                    int noOfKeyWords = words.Length;
                    string wordScoreStr;
                    foreach (string w in words)
                    {
                        wordScoreStr = "";
                        wordScoreStr = wordScoreStr + w + ":";
                        double queryTfVal = getQueryTfVal(w, words);

                        string sql = @"select position from reverseIndex where term = '" + w + "'";

                        SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                        SQLiteDataReader reader = command.ExecuteReader();
                        string position = "";

                        while (reader.Read())
                        {
                            position = reader.GetString(0);
                        }
                        if (!String.Equals(position, ""))
                        {
                            string[] docArr = position.Split(';');
                            int noOfTargetDoc = docArr.Length;
                            double idfVal = Math.Log(noOfTotalDoc / noOfTargetDoc);
                            double querytfIdfVal = queryTfVal * idfVal;
                            string querytfIdfVal_str = querytfIdfVal.ToString("0.00000");
                            wordScoreStr = wordScoreStr + querytfIdfVal_str + ";";
                            double doctfIdfVal = 0.0;
                            foreach (string da in docArr)
                            {
                                int frequency = 0;

                                string[] docPosArr = da.Split(':');
                                string[] posArr = docPosArr[1].Split(',');
                                int docId = Int32.Parse(docPosArr[0]);

                                int totalWordsInDoc = 0;
                                double tfVal = 0.0;
                                frequency = posArr.Length;
                                foreach (List<int> li in allDocList)
                                {
                                    if (docId == li[1])
                                    {
                                        totalWordsInDoc = li[0];
                                        break;
                                    }
                                }

                                tfVal = (double)frequency / (double)totalWordsInDoc;
                                doctfIdfVal = tfVal * idfVal;
                                string doctfIdfVal_str = doctfIdfVal.ToString("0.00000");
                                wordScoreStr = wordScoreStr + docId + ":" + doctfIdfVal_str + ";";
                            }
                        }
                        else
                        {
                            wordScoreStr = wordScoreStr + "0;";
                        }

                        //  tfIdfScoreList.Add(wordScoreStr);
                        seperatedScore.Add(wordScoreStr);
                    }
                    tfIdfScoreList.Add(seperatedScore);
                }

                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                string message = ex.Message + " \n Exit program?";
                string caption = "";
                MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                DialogResult result;

                // Displays the MessageBox.

                result = MessageBox.Show(this, message, caption, buttons,
                    MessageBoxIcon.Question, MessageBoxDefaultButton.Button1,
                    MessageBoxOptions.RightAlign);

                if (result == DialogResult.Yes)
                {
                    this.Close();
                    Application.Exit();
                }

            }
            return tfIdfScoreList;

        }

        private void CrawlData(List<string> filePaths , string pattern)
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
                wordDict = file.ParseDocuments(filePaths, sqlConnection, wordDict, this);
            }
            catch (Exception ex)
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

        private double getQueryTfVal(string word, string[] words)
        {
            double tf = 0.0;
            int cun = 0;
            foreach (string w in words)
            {
                if (String.Equals(w, word)) cun++;
            }
            tf = (double)cun / words.Length;
            return tf;
        }

        private string[] getOrQuery(string word)
        {
            if (word.Contains("(") && word.Contains("|") && word.Contains(")"))
            {
                string[] arrA = word.Split('(');
                string[] arrB = arrA[1].Split(')');
                string[] arrC = arrB[0].Split('|');
                string[] queryArr = { arrA[0] + " " + arrB[1] + arrC[0], arrA[0] + " " + arrB[1] + arrC[1] };
                return queryArr;
            }
            else if (word.Contains("|"))
            {
                string[] queryArr = word.Split('|');
                return queryArr;
            }
            else
            {
                string[] queryArr = { word };
                return queryArr;
            }

        }

        private string[] removeSpaceItem(string[] arr)
        {
            int cun = 0;
            foreach (string w in arr)
            {
                if (w == " " | w == "" | w == "  ")
                {
                    cun++;
                }
            }
            string[] newArr = new string[arr.Length - cun];
            cun = 0;
            foreach (string w in arr)
            {
                if (w != " " & w != "" & w != "  ")
                {
                    newArr[cun] = w;
                    cun++;
                }

            }
            return newArr;
        }

        private string RemoveStopWords(string word, string[] stopWords)
        {
            foreach (string stopWord in stopWords)
            {
                if (word.Contains(stopWord))
                {
                    word = word.Replace(stopWord, string.Empty);
                }
            }

            return word;
        }

        private void BindComboxBox(List<List<string>> phraseList)
        {
            searchQueryComboBox.Items.Clear();
            foreach(List<string> wordList in phraseList)
            {
                string searchQuery = string.Empty;
                foreach (string word in wordList)
                {
                    string singleWord = word.Split(';')[0].Split(':')[0];
                    searchQuery += singleWord + " ";
                }
                searchQueryComboBox.Items.Add(searchQuery);
            }
            searchQueryComboBox.SelectedIndex = 0;
        }

        private string GetFileName(string filePath)
        {
            string fileName = filePath.Substring(filePath.LastIndexOf('\\') + 1);
            return fileName;
        }

        private string GetAndRemoveWordToBeRemoved(ref string query)
        {
            string resultWord = string.Empty;
            string[] words = query.Split(' ');
            foreach(string singleWord in words)
            {
                if (singleWord.Contains('/'))
                {
                    resultWord = singleWord.Substring(singleWord.IndexOf('/') + 1);
                    query = query.Replace(singleWord, string.Empty);
                }
            }
            

            return resultWord;
        }

        private List<string> GetAndRemoveExactWordList(ref string query)
        {
            List<string> resultWordList = new List<string>();
            int count = query.Count(x => x == '"');
            if(count == 2)
            {
                int firstIndex = query.IndexOf('"');
                int lastIndex = query.LastIndexOf('"');
                int length = lastIndex - firstIndex;
                string resultWord = query.Substring(firstIndex + 1, length - 1);
                query = query.Replace('"', ' ');
                resultWordList = resultWord.Split(' ').ToList();
            }
            return resultWordList;
        }

        private List<Dictionary<double, double>> calculateVectorSpace(List<List<string>> TF_IDF_List)
        {

            // To store all the queries and its cosine values for every document
            // e.g. Phrase(string) Dict(documentId, cosineValue)
            //      Apple Orange        1        , 0.6
            //                          8        , 0.45
            //      Apple stawberry     2        , 0.674
            //                          201      , 0.1256
            List<Dictionary<double, double>> cosineValueDictForPhrase = new List<Dictionary<double, double>>();
            foreach (List<string> phrase in TF_IDF_List)
            {
                HashSet<double> documentIdHS = new HashSet<double>();
                Dictionary<string, double> query_TF_IDF_Dict = new Dictionary<string, double>();
                Dictionary<string, Dictionary<double, double>> queryDocument_TF_IDF_dict = new Dictionary<string, Dictionary<double, double>>();

                // Loop through all the words in one phrase
                foreach (string queryAndDocString in phrase)
                {
                    string[] splitString = queryAndDocString.Split(';');
                    string[] queryAndValue = splitString[0].Split(':');
                    string queryTerm = queryAndValue[0];
                    string queryTF_IDF = queryAndValue[1];
                    double queryTF_IDF_double = Convert.ToDouble(queryTF_IDF);
                    // Store all queries and its value inside one dict
                    query_TF_IDF_Dict.Add(queryTerm, queryTF_IDF_double);

                    Dictionary<double, double> document_TF_IDF_dict = new Dictionary<double, double>();
                    for (int i = 1; i < splitString.Length - 1; i++)
                    {
                        string[] documentAndValue = splitString[i].Split(':');
                        double documentId = Convert.ToDouble(documentAndValue[0]);
                        double document_TF_IDF_double = Convert.ToDouble(documentAndValue[1]);
                        // Store all document Id inside hashset to remove duplicates
                        documentIdHS.Add(documentId);
                        // Store all document ID and its value inside dict
                        document_TF_IDF_dict.Add(documentId, document_TF_IDF_double);
                    }
                    // Store the query word along with the respective documentid and its values
                    queryDocument_TF_IDF_dict.Add(queryTerm, document_TF_IDF_dict);

                }

                double divider = 0;
                double dividee = 0;
                double dividerQuery = 0;
                double dividerDocument = 0;
                Dictionary<double, double> cosineForDocumentsDict = new Dictionary<double, double>();
                // Loop through all documents
                foreach (double documentId in documentIdHS)
                {
                    // Loop through all queries 
                    foreach (KeyValuePair<string, double> queryKVP in query_TF_IDF_Dict)
                    {
                        string query = queryKVP.Key;
                        double queryIDF = queryKVP.Value;
                        double documentIDF = 0;
                        // Get all the documents that contains the query
                        Dictionary<double, double> documentDict = queryDocument_TF_IDF_dict[query];
                        // Check if the word is found inside document
                        if (documentDict.ContainsKey(documentId))
                        {
                            documentIDF = documentDict[documentId];
                        }

                        dividee += queryIDF * documentIDF;
                        dividerQuery += Math.Pow(queryIDF, 2);
                        dividerDocument += Math.Pow(documentIDF, 2);
                    }
                    divider = Math.Sqrt(dividerQuery) * Math.Sqrt(dividerDocument);
                    double cosineValue = Math.Round(dividee / divider, 6); // up to 6 decimal places
                    cosineForDocumentsDict.Add(documentId, cosineValue);
                }
                cosineValueDictForPhrase.Add(cosineForDocumentsDict);
            }

            return cosineValueDictForPhrase;
        }

        private void SortAndBindData(List<Dictionary<double,double>> cosineValueDictForPhrase,int comboBoxId, string wordToBeRemoved, List<string> exactWordList)
        {
            DataTable resultTable = new DataTable();
            resultTable.Columns.Add("No.");
            resultTable.Columns.Add("File Name");
            resultTable.Columns.Add("File Location");
            int count = 1;
            string dbName = Path.GetDirectoryName(Application.ExecutablePath) + @"\searchEngine.db";
            var sqlConnection = new SQLiteConnection("DataSource=" + dbName);
            sqlConnection.Open();

            var phraseDict = cosineValueDictForPhrase[comboBoxId];
            var sortedKeyValue = phraseDict.OrderByDescending(x => x.Value);
            foreach (KeyValuePair<double, double> resultPair in sortedKeyValue)
            {
                string filePath = string.Empty;
                string sql = "select documentName from documents where documentId = " + resultPair.Key;
                SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    filePath = reader.GetString(0);
                }
                string fileName = GetFileName(filePath);
                resultTable.Rows.Add(count, fileName, filePath);
                count++;
            }

            resultGV.DataSource = resultTable;
            resultGV.Columns[2].Width = 600;
           
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
            SQLiteConnection sqlConnection;
            try
            {
                SQLiteConnection.CreateFile(databaseName);
                sqlConnection = new SQLiteConnection("DataSource=" + databaseName);
                // Create new database 
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
            }
            catch(Exception ex)
            {

            }
            sqlConnection = new SQLiteConnection("DataSource=" + databaseName);
            sqlConnection.Open();

            return sqlConnection;
        }

        private void InsertOrUpdateReverseIndex(SQLiteConnection sqlConnection, Dictionary<string, string> wordDict)
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

        #endregion

    }
}
