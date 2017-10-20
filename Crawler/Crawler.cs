using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
            string url = @"D:\Test\";
            string[] filePaths = Directory.GetFiles(url, "*.txt");
            foreach(string filePath in filePaths)
            {
                using (FileStream stream  = File.Open(filePath, FileMode.Open))
                {
                    
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
