using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SearchEngine
{
    public class Logger
    {
        public string filePath { get; set; }
        public Logger(string filePath)
        {
            this.filePath = filePath;
        }
        public void write(string message)
        {
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
            {
                file.WriteLine(message);
            }
        }
    }
}
