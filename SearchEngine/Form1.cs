using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
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
            Logger log = new Logger(Directory.GetCurrentDirectory() + @"\log.txt");
            try
            {
                string url = Directory.GetCurrentDirectory();
                string[] filePaths = Directory.GetFiles(url, "*.txt");
                Dictionary<string, string> wordDict = new Dictionary<string, string>();

                string dbName = Directory.GetCurrentDirectory() + @"\test.db";
                if (!File.Exists(dbName))
                {
                    log.write("create db");
                    SQLiteConnection.CreateFile(dbName);
                }

                SQLiteConnection sqlConnection = new SQLiteConnection("DataSource=" + dbName);
                sqlConnection.Open();
                string sql = @"create table documents(
            documentId integer not null primary key autoincrement,
            documentName varchar not null
            )";

                SQLiteCommand command = new SQLiteCommand(sql, sqlConnection);
                command.ExecuteNonQuery();


                foreach (string filePath in filePaths)
                {
                    sql = "insert into documents (documentName) values (" + filePath + ")";
                    command = new SQLiteCommand(sql, sqlConnection);
                    command.ExecuteNonQuery();

                    string[] stringArray = File.ReadAllText(filePath).Split(' ');
                    for (int i = 0; i < stringArray.Length; i++)
                    {
                        string word = stringArray[i];
                        if (wordDict.ContainsKey(word))
                        {
                            wordDict[word] = "," + i;
                        }
                        else
                        {
                            wordDict.Add(word, i.ToString());
                        }
                    }
                }
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                log.write(ex.ToString());
            }

        }
    }
}
