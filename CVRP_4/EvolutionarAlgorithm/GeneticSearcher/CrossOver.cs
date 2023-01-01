using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    static class CrossOver
    {

        //very greedy try
        public static Answer[] method1(Answer[] Answers)
        {
            Random r = new Random();

            //
            // select trucks to replace
            // in this step, a truck with max distance in answer[0] will be replaced with a rtuck with min distance in ans[1],
            // replacement is done for cities for 'index > point'
            //
            int mindx0 = Answers[0].IndexOfMinTruck();
            int maxdx0 = Answers[0].IndexOfMaxTruck();
            int mindx1 = Answers[1].IndexOfMinTruck();
            int maxdx1 = Answers[1].IndexOfMaxTruck();

            // middle point
            int point = (Answers[0].NumberOfCitiesInTruck[mindx0] + Answers[0].NumberOfCitiesInTruck[mindx1] +
                Answers[1].NumberOfCitiesInTruck[mindx0] + Answers[1].NumberOfCitiesInTruck[mindx1]) / 8;

            //backup
            /*Answer[] TmpAnswer = new Answer[Answers.GetUpperBound(0)];
            for(int i=0;i<Answers.GetUpperBound(0);i++)
                TmpAnswer[i]=new Answer(ref Answers[i*/

            int cnt = 0;
            //replacing mindx0,maxdx1
            List<City> cities00 = new List<City>();
            for (int i = Answers[0].NumberOfCitiesInTruck[mindx0] - 1; i >= point; i--) cities00.Add(Answers[0].GetCity(mindx0, i));
            Answers[0].RemoveCitiesAfter(mindx0, point);
            List<City> cities11 = new List<City>();
            for (int i = Answers[1].NumberOfCitiesInTruck[maxdx1] - 1; i >= point; i--) cities11.Add(Answers[1].GetCity(maxdx1, i));
            Answers[1].RemoveCitiesAfter(maxdx1, point);
            for (int i = 0; i < Math.Min(cities00.Count, cities11.Count); i++)
            {
                if (Answers[0].AddCity(mindx0, cities11[i]) < 0)
                {
                    //put back cities
                    Answers[0].AddCity(mindx0, cities00[i]);
                    Answers[1].AddCity(maxdx1, cities11[i]);
                    continue;
                }
                else if (Answers[1].AddCity(maxdx1, cities00[i]) < 0)
                {
                    //cancel last insertion
                    Answers[0].RemoveLastCity(mindx0);
                    //put back cities
                    Answers[0].AddCity(mindx0, cities00[i]);
                    Answers[1].AddCity(maxdx1, cities11[i]);
                }
                cnt++;
            }
            //replacing maxdx0,mindx1
            List<City> cities01 = new List<City>();
            for (int i = Answers[0].NumberOfCitiesInTruck[maxdx0] - 1; i >= point; i--) cities01.Add(Answers[0].GetCity(maxdx0, i));
            Answers[0].RemoveCitiesAfter(maxdx0, point);
            List<City> cities10 = new List<City>();
            for (int i = Answers[1].NumberOfCitiesInTruck[mindx1] - 1; i >= point; i--) cities10.Add(Answers[1].GetCity(mindx1, i));
            Answers[1].RemoveCitiesAfter(mindx1, point);
            for (int i = 0; i < Math.Min(cities01.Count, cities10.Count); i++)
            {
                if (Answers[0].AddCity(maxdx0, cities10[i]) < 0)
                {
                    //put back cities
                    Answers[0].AddCity(maxdx0, cities01[i]);
                    Answers[1].AddCity(mindx1, cities10[i]);
                    continue;
                }
                else if (Answers[1].AddCity(mindx1, cities01[i]) < 0)
                {
                    //cancel last insertion
                    Answers[0].RemoveLastCity(maxdx0);
                    //put back cities
                    Answers[0].AddCity(maxdx0, cities01[i]);
                    Answers[1].AddCity(mindx1, cities10[i]);
                }
                cnt++;
            }

            return Answers;
        }

        //neads 200 iterations
        public static Answer[] method2(Answer[] Answers)
        {
            int mindx0 = Answers[0].IndexOfMinTruck();
            int maxdx0 = Answers[0].IndexOfMaxTruck();
            int mindx1 = Answers[1].IndexOfMinTruck();
            int maxdx1 = Answers[1].IndexOfMaxTruck();

            // middle point
            int point = (Answers[0].NumberOfCitiesInTruck[mindx0] + Answers[0].NumberOfCitiesInTruck[mindx1] +
                Answers[1].NumberOfCitiesInTruck[maxdx0] + Answers[1].NumberOfCitiesInTruck[maxdx1]) / 6;
            //int point = 1;

            //maxdx0 = Answers[0].IndexOfMaxTruck();
            //maxdx1 = Answers[1].IndexOfMaxTruck();

            int[] cities00 = new int[Answers[0].NumberOfCitiesInTruck[maxdx0]];
            int[] cities11 = new int[Answers[1].NumberOfCitiesInTruck[maxdx1]];
            int offset = Math.Min(Answers[0].NumberOfCitiesInTruck[maxdx0] - 1, Answers[1].NumberOfCitiesInTruck[maxdx1] - 1);
            for (int i = offset; i >= point; i--)
            {
                cities00[i] = Answers[0].GetCity(maxdx0, i).number;

                cities11[i] = Answers[1].GetCity(maxdx1, i).number;

                try
                {
                    Answers[1].SetCityNull(Answers[0].GetCity(maxdx0, i).number);
                    Answers[0].SetCityNull(Answers[1].GetCity(maxdx1, i).number);
                }
                catch { }
            }
            for (int i = offset; i >= point; i--)
            {
                Answers[1].SetCity(maxdx1, i, Answers[0].GetCity(cities00[i]));
                Answers[0].SetCity(maxdx0, i, Answers[1].GetCity(cities11[i]));
            }
            for (int i = offset; i >= point; i--)
            {
                if (Answers[0].IndexOf(cities00[i])[0] == -1)
                    Answers[0].FillNullCityBy(Answers[0].GetCity(cities00[i]));
                if (Answers[1].IndexOf(cities11[i])[0] == -1)
                    Answers[1].FillNullCityBy(Answers[1].GetCity(cities11[i]));
            }


            return Answers;
        }

        //neads 10,000 iterations
        public static Answer[] method3(Answer[] Answers)
        {
            #region move frome answer 0 to answer 1
            int mintruck = Answers[0].IndexOfMinTruck();
            while (Answers[0].NumberOfCitiesInTruck[mintruck] < 3) mintruck = (mintruck + 1) % Answers[0].TrucskNum;

            double[] dist = new double[Answers[0].NumberOfCitiesInTruck[mintruck]];
            for (int i = 1; i < Answers[0].NumberOfCitiesInTruck[mintruck] - 1; i++)
                dist[i] = Answers[0].GetDistance(mintruck, i, mintruck, i - 1) + Answers[0].GetDistance(mintruck, i, mintruck, i + 1);

            int idx = 0; double v = 100000;
            for (int i = 1; i < Answers[0].NumberOfCitiesInTruck[mintruck] - 1; i++) if (dist[i] < v) { v = dist[i]; idx = i; }

            List<int> cities1 = new List<int>();
            cities1.Add(Answers[0].GetCity(mintruck, idx - 1).number);
            cities1.Add(Answers[0].GetCity(mintruck, idx).number);
            cities1.Add(Answers[0].GetCity(mintruck, idx + 1).number);
            Answers[1].RemoveCity(cities1[0]);
            Answers[1].RemoveCity(cities1[1]);
            Answers[1].RemoveCity(cities1[2]);

            int mintruck2 = Answers[1].IndexOfMinDemandTruck();
            Answers[1].AddCity(mintruck2, cities1[0]);
            Answers[1].AddCity(mintruck2, cities1[1]);
            Answers[1].AddCity(mintruck2, cities1[2]);
            #endregion

            return Answers;
        }
    }
}
