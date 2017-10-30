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
            

        }

        public void calculateVectorSpace(List<List<string>> TF_IDF_List, string wordToBeRemoved, List<string> exactWordList)
        {
            
            List<string> firstPhraseTestData = new List<string>();
            firstPhraseTestData.Add("apple:0.5;1:1.20329;8:0.45318;54:1.0125");
            firstPhraseTestData.Add("orange:0.5;1:0.12347;201:0.64792;356:1.72453;60:1.203;55:1.022");
            TF_IDF_List.Add(firstPhraseTestData);

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
                    for (int i = 1; i < splitString.Length; i++)
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
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string searchKeyWord = textBox1.Text;
            List<List<string>> scoreList = findKeyword(searchKeyWord);
            string li = "";
            foreach (List<string> scl in scoreList)
            {
                foreach (string a in scl)
                {
                    li = li + a + "***";
                }
                li = li + "---";
            }
            textBox2.Text = li;

            // Calculate Vector space 
            // Pass the 3 variables to the above method
        }

        public List<List<string>> findKeyword(string word)
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
        public double getQueryTfVal(string word, string[] words)
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
        public string[] getOrQuery(string word)
        {
            if (word.Contains("(") && word.Contains("|") && word.Contains(")"))
            {
                string[] arrA = word.Split('(');
                string[] arrB = arrA[1].Split(')');
                string[] arrC = arrB[0].Split('|');
                string[] queryArr = { arrA[0] + " " + arrB[1] + arrC[0], arrA[0] + " " + arrB[1] + arrC[1] };
                return queryArr;
            }
            else
            {
                string[] queryArr = { word };
                return queryArr;
            }

        }
        public string[] removeSpaceItem(string[] arr)
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
    }
}
