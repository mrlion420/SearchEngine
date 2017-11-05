using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SearchEngine
{
    public class FileHelper
    {
        
        public Dictionary<string, string> ParseDocuments(List<string> filePaths, SQLiteConnection sqlConnection, Dictionary<string, string> wordDict,Form1 form)
        {
            string[] stopWords = new string[] { ",", ".", ";", ":", "'", "\"", "\\", "/", "|", "_", "-", "(", ")" , "\r\n" , "\r" , "\n" , "\t" };
            string sql = string.Empty;
            SQLiteCommand command;
            
            foreach (string filePath in filePaths)
            {
                
                if (!isIndexedInDatabase(filePath, sqlConnection))
                {
                    long documentId = 0;
                    HashSet<string> isDocumentIdInsertedForWord = new HashSet<string>();

                    string[] stringArray = File.ReadAllText(filePath).Replace(Environment.NewLine, " ").Split(' ');

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
                    int index = filePath.LastIndexOf('\\');
                    int secondIndex = filePath.LastIndexOf('.');
                    string fileName = filePath.Substring(index + 1, secondIndex - index - 1);
                    string[] fileNameArray = fileName.Split(' ');
                    List<string> holder = stringArray.ToList();
                    foreach (string fileNameWord in fileNameArray)
                    {
                        holder.Add(fileNameWord);
                    }
                    
                    stringArray = holder.ToArray();
                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        string resultString = string.Empty;
                        string word = stringArray[i].ToLower();
                        word = RemoveStopWords(word, stopWords);
                        word = StripHTML(word);
                        word = RemoveUnicode(word);
                        
                        if (!ContainsUnicodeCharacter(word) && !string.IsNullOrWhiteSpace(word))
                        {
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
                
                form.UpdateProgressBar();
                
            }

            return wordDict;
        }

        public string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
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

        private string RemoveUnicode(string input)
        {
            return System.Text.Encoding.ASCII.GetString(System.Text.Encoding.ASCII.GetBytes(input));
        }

        public bool ContainsUnicodeCharacter(string input)
        {
            const int MaxAnsiCode = 255;

            return input.Any(c => c > MaxAnsiCode);
        }

        public string RemoveStopWords(string word, string[] stopWords)
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
    }
}
