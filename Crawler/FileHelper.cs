using System;
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
        public Dictionary<string,string> ParseDocuments(string[] filePaths, SQLiteConnection sqlConnection, Dictionary<string, string> wordDict)
        {
            string sql = string.Empty;
            SQLiteCommand command;
            foreach (string filePath in filePaths)
            {
                bool isDocumentIdInserted = false;
                long documentId = 0;

                sql = "insert into documents (documentName) values ('" + filePath + "')";
                command = new SQLiteCommand(sql, sqlConnection);
                command.ExecuteNonQuery();

                sql = "select documentId from documents where documentName = '" + filePath + "'";
                command = new SQLiteCommand(sql, sqlConnection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    documentId = Convert.ToInt64(reader["documentId"]);
                }

                string[] stringArray = File.ReadAllText(filePath).Split(' ');
                for (int i = 0; i < stringArray.Length; i++)
                {
                    // NEED TO DECIDE WHETHER WE WILL INCLUDE STOP WORDS SUCH AS PREPOSITION OR NOT
                    string word = stringArray[i];
                    if (wordDict.ContainsKey(word))
                    {
                        if (isDocumentIdInserted)
                        {
                            wordDict[word] = ";" + documentId + ":" + i;
                            isDocumentIdInserted = true;
                        }
                        else
                        {
                            wordDict[word] = "," + i;
                        }

                    }
                    else
                    {
                        wordDict.Add(word, documentId + ":" + i.ToString());
                    }
                }
            }

            return wordDict;
        }
    }
}
