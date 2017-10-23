﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public class FileHelper
    {
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

        public string RemoveStopWords(string word, string[] stopWords)
        {
            foreach (string stopWord in stopWords)
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
