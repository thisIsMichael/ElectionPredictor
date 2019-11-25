using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ElectionPredictor
{
    public class OutputController : IDisposable
    {
        private StreamWriter file;

        public OutputController(string filepath)
        {
            file = File.CreateText(filepath);
        }

        public void Dispose()
        {
            file.Dispose();
            file = null;
        }

        public void WriteLine(string value = "")
        {
            file.WriteLine(value);
            file.Flush();
            Console.WriteLine(value);
        }

        
    }
}
