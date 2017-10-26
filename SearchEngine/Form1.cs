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
            List<List<string>> TF_IDF_List = new List<List<string>>();
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
            foreach(List<string> phrase in TF_IDF_List)
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
                    for(int i = 1;i < splitString.Length; i++)
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
                foreach(double documentId in documentIdHS)
                {
                    // Loop through all queries 
                    foreach(KeyValuePair<string,double> queryKVP in query_TF_IDF_Dict)
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

        public double CalculateCosineSimilarity(double query_TF_IDF, double document_TF_IDF)
        {
            double result = 0;
            // Calculate by given theory
            // assign the final result to result
            return result;
        }

    }
}
