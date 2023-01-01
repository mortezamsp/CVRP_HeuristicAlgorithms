using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class AntColonySearcher : EvolutionaryAlgorithm
    {
        #region algorithm parameters
        private int NumberOfAnts;
        private double[,] DistanceMatrix;
        private double[,] etha;
        private double[,] tau;
        private double tau0;
        double alpha;
        double beta;
        double rho;
        #endregion

        public AntColonySearcher(ProblemInformations p)
        {
            pinfs = p;
            DistanceMatrix = new double[p.DEMENTION, p.DEMENTION];
            etha = new double[p.DEMENTION, p.DEMENTION];
            tau = new double[p.DEMENTION, p.DEMENTION];
        }

        public void ACSearch()
        {
            #region reading settings
            //
            //  reading information about termination condition
            //
            string TerminationConditionType = SettingsReader.ReadValueOfString("TerminationCondition");
            int evals = 0;
            int MaxEvals = SettingsReader.ReadValueOfInteger("MaxEvals_Constraint");
            int Minute = DateTime.Now.Minute;
            int Second = DateTime.Now.Second;
            int MaxSeconds = SettingsReader.ReadValueOfInteger("Time_Secconds_Constraint");
            int t = 0;
            int MaxT = SettingsReader.ReadValueOfInteger("MAXIteration_Constraint");

            //
            //  reading algorithm parameters
            //
            NumberOfAnts = SettingsReader.ReadValueOfInteger("AC_NumberOfAnts");
            tau0 = SettingsReader.ReadValueOfDouble("AC_tau0");
            rho = SettingsReader.ReadValueOfDouble("AC_rho");
            alpha = SettingsReader.ReadValueOfDouble("AC_alpha");
            beta = SettingsReader.ReadValueOfDouble("AC_beta");
            #endregion

            #region initing values
            for (int i = 0; i < pinfs.DEMENTION; i++)
            {
                for (int j = 0; j < pinfs.DEMENTION; j++)
                {
                    double dist = new Answer(ref pinfs).GetDistance(i, j);
                    DistanceMatrix[i, j] = DistanceMatrix[j, i] = dist;
                    etha[i, j] = etha[j, i] = 1 / dist;
                    tau[i, j] = 1;
                }
            }
            #endregion

            #region searching
            Random r = new Random();
            bool looping = true;
            while(looping)
            {
                GenerateInitialPopulation(NumberOfAnts, ref r);

                #region complete each ant's solution
                bool AddingCityIsFinished = false;
                int maxit = pinfs.DEMENTION * 10, it = 0;
                while(! AddingCityIsFinished && it <= maxit)
                {
                    //  for each ant, select the city 'i' with probability
                    for (int j = 0; j < NumberOfAnts; j++)
                    {
                        if (Population[j].TrucksAreFull())
                            continue;

                        //  foreach TRUCK of ANT-solution 'j'
                        for (int k = 0; k < pinfs.TRUKCSNUM; k++)
                        {
                            if (Population[j].TrucksAreFull())
                                continue;

                            //  'p[i]' contains attractness of a city 'i'
                            double[] p = new double[pinfs.DEMENTION];

                            double psum = 0;
                            for (int a = 0; a < pinfs.DEMENTION; a++)
                            {
                                //  if city 'i' is not added
                                if (Population[j].IndexOf(a + 1)[0] == -1)
                                {
                                    //  each 'population[j]' is one ANT; and contains a solution.
                                    int ii = Population[j].Routes[k, Population[j].NumberOfCitiesInTruck[k] - 1] - 1;
                                    p[a] = Math.Pow(tau[ii, a], alpha) * Math.Pow(etha[ii, a], beta);
                                    psum += p[a];
                                }
                            }

                            //  select the city with best probability
                            for (int a = 0; a < pinfs.DEMENTION; a++) p[a] = p[a] / psum;
                            double randnumber = r.NextDouble();
                            int index = -1;
                            for (int a = 0; a < pinfs.DEMENTION; a++)
                                if (p[a] >= randnumber)
                                    index = a;
                            if (index == -1)    //then select the max city
                            {
                                double maxvalue = 0;
                                for (int a = 0; a < pinfs.DEMENTION; a++)
                                    if (p[a] > maxvalue)
                                    {
                                        index = a;
                                        maxvalue = p[a];
                                    }
                            }
                            
                            //  add best city to TRUCK's solution
                            if (index > -1)
                                Population[j].AddCity(k, index + 1);
                        }

                    }

                    //  checking termination condition
                    AddingCityIsFinished = true;
                    for (int i2 = 0; i2 < NumberOfAnts; i2++)
                    {
                        if (!Population[i2].TrucksAreFull())
                        {
                            AddingCityIsFinished = false;
                            break;
                        }
                    }
                    it++;
                }
                for (int i = 0; i < NumberOfAnts; i++)
                {
                    if (!Population[i].TrucksAreFull())
                    {
                        continue;
                    }
                }
                #endregion

                #region pheromon
                double Cost;
                double minCost = 1000000;
                int minCostIndex = 0;
                for (int i = 0; i < NumberOfAnts; i++)
                {
                    if (!Population[i].TrucksAreFull())
                        continue;

                    Cost = Population[i].GetTotalDistance();
                    if (Cost < minCost)
                    {
                        minCost = Cost;
                        minCostIndex = i;
                    }

                    for (int k = 0; k < pinfs.TRUKCSNUM; k++)
                    {
                        for (int l = 0; l < Population[i].NumberOfCitiesInTruck[k]; l++)
                        {
                            int ii = Population[i].Routes[k, l] - 1;
                            int jj = (l == Population[i].NumberOfCitiesInTruck[k] - 1) ?
                                Population[i].Routes[k, 0] - 1 : Population[i].Routes[k, l + 1] - 1;

                            tau[ii, jj] = tau[jj, ii] = tau[ii, jj] + (1 / Cost);
                        }
                    }
                }

                for (int i = 0; i < pinfs.DEMENTION; i++)
                {
                    for (int j = 0; j < pinfs.DEMENTION; j++)
                    {
                        tau[i, j] = (1 - rho) * tau[i, j];
                    }
                }
                #endregion

                #region assign best answer

                if (BestAnswer == null)
                    BestAnswer = new Answer(ref Population[minCostIndex]);
                else if (minCost < BestAnswer.GetTotalDistance())
                    BestAnswer = new Answer(ref Population[minCostIndex]);

                #endregion

                #region cheking termination condition
                t++;
                switch (TerminationConditionType)
                {
                    case "evals":
                        if (evals >= MaxEvals)
                            looping = false;
                        break;
                    case "time":
                        if ((DateTime.Now.Minute - Minute - 1) * 60 + (60 - Second) + DateTime.Now.Second >= MaxSeconds)
                            looping = false;
                        break;
                    case "iteration":
                        if (t >= MaxT)
                            looping = false;
                        break;
                }
                #endregion
            }
            #endregion
        }

        protected override Answer GenerateFirstAnswerRandomally(ref Random r)
        {
            //
            // add a city to each TRUCK of an ANT
            //
            Answer ans = new Answer(ref pinfs);
            ans.logging = false;
            for (int j = 0; j < pinfs.TRUKCSNUM; j++)
            {
                //int citynumber = 0;
                //do { citynumber = r.Next(0, pinfs.DEMENTION); }
                //while (ans.IndexOf(citynumber + 1)[0] > 0);
                //ans.AddCity(j, citynumber + 1);
                ans.AddCity(j, 1);
            }
            return ans;
        }
    }
}
