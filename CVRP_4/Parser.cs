using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CVRP_4
{
    static class Parser
    {
        public static ProblemInformations ParsInputs(string[] args)
        {
            string filename;
            GETINPUT:
            if (args.Length == 0)
            {
                Console.Write("enter filename : \t");
                Console.ForegroundColor = ConsoleColor.Cyan;
                filename = Console.ReadLine();
                Console.ResetColor();
                Console.Write("\n");
            }
            else
                filename = args[0];
            if (!filename.EndsWith(".vrp"))
                filename = filename + ".vrp";
            if (!File.Exists(filename))
            {
                Console.WriteLine("ERRORE!\n\tan error accured with filename.\n\tfile does not exists.\n\ttry again.\n\n");
                args = new string[] { };
                goto GETINPUT;
            }


            StreamReader sr = new StreamReader(filename);

            sr.ReadLine();
            string line = sr.ReadLine();
            string tmp = line.Substring(line.IndexOf("No of trucks: ") + 14);
            int numtruks = int.Parse(tmp.Substring(0, tmp.IndexOf(',')));
            sr.ReadLine();
            line = sr.ReadLine();
            int dementions = int.Parse(line.Substring(line.IndexOf(": ") + 2));
            sr.ReadLine();
            line = sr.ReadLine();
            int capacity = int.Parse(line.Substring(line.IndexOf(": ") + 2));
            sr.ReadLine();

            List<City> cities = new List<City>();
            for (int i = 0; i < dementions; i++)
            {
                line = sr.ReadLine();
                string[] nums = line.Split(new char[] { ' ' });
                int index = 0;
                while (nums[index] == "")
                    index++;
                cities.Add(new City(int.Parse(nums[index]), int.Parse(nums[index + 1]), int.Parse(nums[index + 2])));
            }

            sr.ReadLine();
            for (int i = 0; i < dementions; i++)
            {
                line = sr.ReadLine();
                string[] nums = line.Split(new char[] { ' ' });
                int index = 0;
                while (nums[index] == "")
                    index++;
                cities[int.Parse(nums[index]) - 1].demand = int.Parse(nums[index + 1]);
            }

            ProblemInformations pinfs = new ProblemInformations(dementions, capacity, numtruks, cities);

            sr.Close();
            return pinfs;
        }
    }
}
