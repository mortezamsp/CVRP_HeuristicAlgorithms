using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    static class Mutation
    {
            public static Answer method1(Answer Answer)
        {
            Random r = new Random();
            double p = r.Next(10);
            if (p >= 6)
            {
                Answer[] ans = new Answer[3];
                int index=0;
                double val=10000000;
                for (int i = 0; i < 3; i++)
                {
                    ans[i] = Insertion_Deletion(Answer);
                    double v = ans[i].GetTotalDistance();
                    if (v < val)
                    {
                        val = v;
                        index = i;
                    }
                }
                return ans[index];
            }
            else if (p >= 4)
            {
                Answer[] ans = new Answer[3];
                int index = 0;
                double val = 10000000;
                for (int i = 0; i < 3; i++)
                {
                    ans[i] = Replace_Randomly(Answer);
                    double v = ans[i].GetTotalDistance();
                    if (v < val)
                    {
                        val = v;
                        index = i;
                    }
                }
                return ans[index];
            }
            else if (p >= 0)
            {
                Answer[] ans = new Answer[3];
                int index = 0;
                double val = 10000000;
                for (int i = 0; i < 3; i++)
                {
                    ans[i] = Replace_InsideTruck(Answer);
                    double v = ans[i].GetTotalDistance();
                    if (v < val)
                    {
                        val = v;
                        index = i;
                    }
                }
                return ans[index];
            }
            return Answer;
        }


            public static Answer Insertion_Deletion(Answer CurrentAnswer)
            {
                Random r = new Random();

                //
                // select 2 truckks randomally
                //
                int tr1 = 0, tr2 = 0;
                int fail = 0;
                do
                {
                    tr1 = r.Next(CurrentAnswer.TrucskNum);
                    tr2 = r.Next(CurrentAnswer.TrucskNum);
                    fail++;
                    if (fail > 10)
                        return CurrentAnswer;
                }
                while (tr1 == tr2 || CurrentAnswer.NumberOfCitiesInTruck[tr1] < 3);

                //
                // seleect the best city to remove
                //
                for (int j = 1; j < CurrentAnswer.NumberOfCitiesInTruck[tr1]; j++)
                {
                    City c = new City(CurrentAnswer.GetCity(tr1, j));
                    if (CurrentAnswer.GetTotalDemandOfTruck(tr2) + c.demand < CurrentAnswer.Capacity)
                    {
                        //
                        // select best city in destination to place
                        //
                        double _min_val = 100000000;
                        int _min_indx = CurrentAnswer.NumberOfCitiesInTruck[tr2];
                        for (int k = 0; k < CurrentAnswer.NumberOfCitiesInTruck[tr2]; k++)
                        {
                            Answer _tmp = new Answer(ref CurrentAnswer);
                            _tmp.InsertCityAfter(tr2, CurrentAnswer.cities[CurrentAnswer.Routes[tr1, j] - 1], CurrentAnswer.cities[CurrentAnswer.Routes[tr2, k] - 1]);
                            double _tmpcost = _tmp.GetTotalDistanceOfTruck(tr2);
                            if (_tmpcost < _min_val)
                            {
                                _min_val = _tmpcost;
                                _min_indx = k;
                            }
                        }

                        Answer NewAnswer = new Answer(ref CurrentAnswer);
                        City b = NewAnswer.cities[NewAnswer.Routes[tr2, _min_indx] - 1];
                        NewAnswer.RemoveCity(tr1, ref c);
                        NewAnswer.InsertCityAfter(tr2, c, b);

                        //change if improved
                        if (NewAnswer.GetTotalDistance() < CurrentAnswer.GetTotalDistance())
                        {
                            if (NewAnswer.logging)
                                LogWriter.WriteLine("log.txt", "\nmoved\t\t[" + tr1.ToString() + ":" + c.number.ToString() + "]->[" + tr2.ToString() + "]");

                            return NewAnswer;
                        }
                    }
                }

                return CurrentAnswer;
            }
            public static Answer Replace_Randomly(Answer CurrentAnswer)
            {
                Answer NewAnswer = new Answer(ref CurrentAnswer);
                Random r = new Random();

                int tr1 = r.Next(0, CurrentAnswer.TrucskNum);
                int ct1 = r.Next(0, CurrentAnswer.NumberOfCitiesInTruck[tr1]);
                int tr2 = r.Next(0, CurrentAnswer.TrucskNum);
                int ct2 = r.Next(0, CurrentAnswer.NumberOfCitiesInTruck[tr2]);
                while (NewAnswer.ReplaceCities(tr1, CurrentAnswer.GetCity(tr1, ct1), tr2, CurrentAnswer.GetCity(tr2, ct2)) < 0)
                {
                    tr1 = r.Next(0, CurrentAnswer.TrucskNum);
                    ct1 = r.Next(0, CurrentAnswer.NumberOfCitiesInTruck[tr1]);
                    tr2 = r.Next(0, CurrentAnswer.TrucskNum);
                    ct2 = r.Next(0, CurrentAnswer.NumberOfCitiesInTruck[tr2]);
                }

                return NewAnswer;
            }
            public static Answer Replace_InsideTruck(Answer CurrentAnswer)
            {
                Random r = new Random();
                int tr1 = r.Next(0, CurrentAnswer.TrucskNum);
                int ct1 = r.Next(0, CurrentAnswer.NumberOfCitiesInTruck[tr1]);

                double minv = 10000;
                int index = -1;
                City c = CurrentAnswer.GetCity(tr1, ct1);
                for (int i = 1; i < CurrentAnswer.NumberOfCitiesInTruck[tr1]; i++)
                {
                    Answer ans = new Answer(ref CurrentAnswer);
                    City c2 = CurrentAnswer.GetCity(tr1, i);
                    ans.RemoveCity(tr1, ref c);
                    ans.InsertCityAfter(tr1, c, c2);
                    double cost = ans.GetTotalDistance();
                    if (cost < minv)
                    {
                        minv = cost;
                        index = i;
                    }
                }
                if (index != -1)
                {
                    Answer ans = new Answer(ref CurrentAnswer);
                    City b = CurrentAnswer.GetCity(tr1, index);
                    ans.RemoveCity(tr1, ct1);
                    ans.InsertCityAfter(tr1, c, b);
                    return ans;
                }

                return CurrentAnswer;
            }
            public static Answer Insert_Cycle(Answer CurrentAnswer)
            {
                Random r = new Random();
                int tr1 = 0;
                do { tr1 = r.Next(CurrentAnswer.TrucskNum); } while (CurrentAnswer.NumberOfCitiesInTruck[tr1] >= CurrentAnswer.Capacity - 1);

                double mincost = 100000;
                int bestct1index = 0;
                int bestct2index = 0;
                double TruckCost = CurrentAnswer.GetTotalDistanceOfTruck(tr1);
                for (int i = 2; i < CurrentAnswer.NumberOfCitiesInTruck[tr1] - 1; i++)
                {
                    double oldcost = CurrentAnswer.GetDistance(tr1, i - 1, tr1, i) + CurrentAnswer.GetDistance(tr1, i, tr1, i + 1);
                    double cost1 = CurrentAnswer.GetDistance(tr1, i - 1, tr1, 0) + CurrentAnswer.GetDistance(tr1, 0, tr1, i + 1);
                    for (int j = 0; j < CurrentAnswer.NumberOfCitiesInTruck[tr1] - 1; j++)
                    {
                        if (i == j || i == j + 1)
                            continue;
                        double newcost = cost1 + CurrentAnswer.GetDistance(tr1, j, tr1, i) + CurrentAnswer.GetDistance(tr1, i, tr1, j + 1);
                        if (newcost < oldcost)
                        {
                            if (newcost < mincost)
                            {
                                mincost = newcost;
                                bestct1index = i;
                                bestct2index = j;
                            }
                        }
                    }
                }

                // do best cycle-insertion
                if (mincost != 100000)
                {
                    Answer newanswer = new Answer(ref CurrentAnswer);

                    City c1 = newanswer.GetCity(tr1, bestct1index);
                    City c2 = newanswer.GetCity(tr1, bestct2index);
                    newanswer.InsertCityAfter(tr1, newanswer.GetCity(tr1, 0), newanswer.GetCity(tr1, bestct1index - 1));
                    newanswer.InsertCityAfter(tr1, c1, c2);
                    newanswer.RemoveCity(tr1, bestct1index + 2);

                    return newanswer;
                }

                return CurrentAnswer;
            }
    }
}
