using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class Program
    {
        static void Main(string[] args)
        {
            _READ_INPUT_:
            Console.ForegroundColor = ConsoleColor.Magenta;

            //read filename forme console
            //and load data into pinfs
            ProblemInformations pinfs = Parser.ParsInputs(args);

            _RUN_:
            int runs = SettingsReader.ReadValueOf("MaxRuns");
            double[] a = new double[runs];
            
            for (int i = 0; i < runs; i++)
            {
                LogWriter.ClearLog("log.txt");
                AntColonySearcher AC = new AntColonySearcher(pinfs);
                AC.ACSearch();
                a[i] = AC.QualityOfAnswer();
                //GeneticSearcher gs = new GeneticSearcher(pinfs);
                //gs.SearchGA();
                //a[i] = gs.QualityOfAnswer();
                Console.WriteLine("answer[" + i.ToString() + "] quality = " + a[i].ToString() + " ,");
            }
            Console.WriteLine("\n\nAVERAGE after " + runs.ToString() + " run = " + (a.Sum() / runs).ToString() + " .");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\n\n\tContinue? press 'y'\n\trun with another methods? press 'a' . . .");
            Console.ForegroundColor = ConsoleColor.Green;
            char key = Console.ReadKey().KeyChar;
            Console.WriteLine();
            Console.ResetColor();
            if (key == 'y')
                goto _READ_INPUT_;
            if (key == 'a')
                goto _RUN_;

        }
    }
}
