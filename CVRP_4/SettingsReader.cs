using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CVRP_4
{
    static class SettingsReader
    {
        /// <summary>
        /// read value of integer
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static int ReadValueOf(string key)
        {
            StreamReader sr = new StreamReader("settings.txt");

            int ret = 0;
            while (true)
            {
                string line = sr.ReadLine();

                if (line != null && line.StartsWith("//"))
                    continue;

                if (line == null)
                    break;

                if (line == key)
                {
                    while (line.StartsWith("//"))
                        line = sr.ReadLine();
                    ret = int.Parse(sr.ReadLine());
                }
            }

            sr.Close();

            return ret;
        }
        public static int ReadValueOfInteger(string key)
        {
            return ReadValueOf(key);
        }

        /// <summary>
        /// read value of double
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static double ReadValueOfDouble(string key)
        {
            StreamReader sr = new StreamReader("settings.txt");

            double ret = 0;
            while (true)
            {
                string line = sr.ReadLine();

                if (line != null && line.StartsWith("//"))
                    continue;

                if (line == null)
                    break;

                if (line == key)
                {
                    while (line.StartsWith("//"))
                        line = sr.ReadLine();
                    ret = double.Parse(sr.ReadLine());
                }
            }

            sr.Close();

            return ret;
        }

        /// <summary>
        /// read value of string
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string ReadValueOfString(string key)
        {
            StreamReader sr = new StreamReader("settings.txt");

            string ret = null;
            while (true)
            {
                string line = sr.ReadLine();

                if (line != null && line.StartsWith("//"))
                    continue;

                if (line == null)
                    break;

                if (line == key)
                {
                    ret = sr.ReadLine();
                }
            }

            sr.Close();

            return ret;
        }

        /// <summary>
        /// read value of boolean
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool ReadValueOfBool(string key)
        {
            StreamReader sr = new StreamReader("settings.txt");

            bool ret = false;
            while (true)
            {
                string line = sr.ReadLine();

                if (line != null && line.StartsWith("//"))
                    continue;

                if (line == null)
                    break;

                if (line == key)
                {
                    ret = bool.Parse(sr.ReadLine());
                }
            }

            sr.Close();

            return ret;
        }
    }
}
