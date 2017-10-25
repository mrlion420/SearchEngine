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
            List<string> TF_IDF_List = new List<string>();
            TF_IDF_List.Add("apple:0.5;1:1.20329;8:0.45318;54:1.0125");
            TF_IDF_List.Add("orange:0.5;2:0.12347;201:0.64792;356:1.72453;60:1.203;55:1.022");

            // To store all the queries and its cosine values for every document
            // e.g. Query(string) Dict(cosineValue, documentID)
            //      Apple         0.6,  1
            //                    0.45, 8
            //      Orange        0.5,  2
            //                    0.35, 201
            Dictionary<string, Dictionary<double, double>> cosineValueDictForQuery = new Dictionary<string, Dictionary<double, double>>();
            foreach(string queryAndDocString in TF_IDF_List)
            {
                string[] splitString = queryAndDocString.Split(';');
                string[] queryAndValue = splitString[0].Split(':');
                string queryTerm = queryAndValue[0];
                string queryTF_IDF = queryAndValue[1];
                double queryTF_IDF_double = Convert.ToDouble(queryTF_IDF);

                // To store the document ID and its cosine value together
                // E.g. document ID 1 has 0.6 cosine value
                Dictionary<double, double> documentCosineDict = new Dictionary<double, double>();

                for(int i = 1;i < splitString.Length; i++)
                {
                    string[] documentIdAndValue = splitString[i].Split(':');
                    string documentId = documentIdAndValue[0];
                    string document_TF_IDF = documentIdAndValue[1];
                    // Conversion
                    double documentId_double = Convert.ToDouble(documentId);
                    double document_TF_IDF_double = Convert.ToDouble(document_TF_IDF);
                    // Calculate
                    double cosineValue = CalculateCosineSimilarity(queryTF_IDF_double, document_TF_IDF_double);
                    // Store the current document ID and its value into dict
                    documentCosineDict.Add(documentId_double, cosineValue);
                }
                // When all the document ID has been stored, store them into the query where the document belongs
                cosineValueDictForQuery.Add(queryTerm, documentCosineDict);
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
