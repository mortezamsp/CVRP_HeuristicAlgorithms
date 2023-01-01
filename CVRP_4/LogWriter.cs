using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CVRP_4
{
    static class LogWriter
    {
        public static void ClearLog(string logfilename)
        {
            if (logfilename.EndsWith(".txt") && File.Exists(logfilename))
            {
                StreamWriter sw = new StreamWriter(logfilename, false);
                sw.Close();
            }
        }
        public static void Write(string logfilename, string message)
        {
            StreamWriter sw = new StreamWriter(logfilename, true);
            sw.Write(message);
            sw.Close();
        }
        public static void WriteLine(string logfilename, string message)
        {
            StreamWriter sw = new StreamWriter(logfilename, true);
            sw.WriteLine(message);
            sw.Close();
        }
    }
}
