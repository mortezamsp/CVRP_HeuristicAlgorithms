using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class GeneticSearcher : EvolutionaryAlgorithm
    {
        private int PopulationSize;
        public GeneticSearcher(ProblemInformations p)
        {
            pinfs = p;
            PopulationSize = p.DEMENTION;
        }
        public void SearchGA()
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
            //  erading variables values
            //
            int _populationSize = SettingsReader.ReadValueOfInteger("GA_FirstPopulationSize");
            int NumberOfCrossovers = SettingsReader.ReadValueOfInteger("GA_NumberOf_CrossOvers");
            int NumberOfOffSprings = SettingsReader.ReadValueOfInteger("GA_OffSprings");
            int NumberOfMutations = SettingsReader.ReadValueOfInteger("GA_NumberOf_Mutations");
            #endregion

            //
            //  first population generation
            //
            Random r = new Random();
            GenerateInitialPopulation(_populationSize, ref r);

            //
            //  start looping
            //
            #region looping
            int[] POPU_RanksIndex = new int[PopulationSize];
            bool looping = true;
            while (looping)
            {
                try
                {
                    //
                    //  create new population throw crossover
                    //
                    Answer[] offsprings = new Answer[NumberOfCrossovers * 2];
                    double[] Ranks = new double[NumberOfCrossovers * 2];
                    for (int i = 0; i < NumberOfCrossovers; i++)
                    {
                        int c1 = r.Next(0, PopulationSize);
                        int c2 = r.Next(0, PopulationSize);
                        Answer[] tmp = CrossOver.method2(new Answer[] { Population[c1], Population[c2] });      /*heart is hear*/

                        offsprings[i * 2] = tmp[0];
                        offsprings[i * 2 + 1] = tmp[1];

                        Ranks[i * 2] = QualityOfAnswer(ref tmp[0]);
                        Ranks[i * 2 + 1] = QualityOfAnswer(ref tmp[1]);
                    }
                    int[] OFSP_RanksIndex = SortByQuality(ref Ranks, "inc");
                    evals += NumberOfCrossovers * 2;


                    //
                    //  sort population by quality
                    //
                    double[] populationRanks = new double[PopulationSize];
                    for (int i = 0; i < PopulationSize; i++)
                        populationRanks[i] = QualityOfAnswer(ref Population[i]);
                    POPU_RanksIndex = SortByQuality(ref populationRanks, "dec");
                    evals += PopulationSize;


                    //
                    //  replace OffSprings by population
                    //
                    for (int i = 0; i < NumberOfOffSprings; i++)
                        Population[POPU_RanksIndex[i]] = new Answer(ref offsprings[OFSP_RanksIndex[i]]);


                    //
                    //  mutation on one randomly answer
                    //
                    for (int i = 0; i < NumberOfMutations; i++)
                    {
                        int MIndex = r.Next(0, PopulationSize);
                        Population[MIndex] = new Answer(Mutation.method1(Population[MIndex]));
                    }

                }
                catch { }

                //
                //  checking termination condition
                //
                #region termination condition
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

            BestAnswer = new Answer(ref Population[POPU_RanksIndex[PopulationSize - 1]]);
        }

        protected override Answer GenerateFirstAnswerRandomally(ref Random r)
        {
            int failed = 0;
        _INITIALIZE_:
            Answer answer = new Answer(pinfs.TRUKCSNUM, pinfs.CAPACITY, pinfs.DEMENTION, pinfs.cities);
            answer.logging = false;

            //add first city
            for (int i = 0; i < pinfs.TRUKCSNUM; i++)
                answer.AddCity(i, pinfs.cities[0]);

            //add another cities randomally
            for (int i = 1; //ignore first cities
                i < pinfs.DEMENTION; i++)
            {
            _PRODUCE_RAND_:
                int trucknum = r.Next(pinfs.TRUKCSNUM);

                if (answer.GetTotalDemandOfTruck(trucknum) + pinfs.cities[i].demand <= pinfs.CAPACITY)
                {
                    answer.AddCity(trucknum, pinfs.cities[i]);
                    failed = 0;
                }
                else
                {
                    failed++;
                    if (failed >= pinfs.TRUKCSNUM)
                    {
                        failed = 0;
                        goto _INITIALIZE_;
                    }
                    goto _PRODUCE_RAND_;
                }

            }

            //check correctness of answer
            for (int i = 0; i < pinfs.TRUKCSNUM; i++)
            {
                if (answer.GetTotalDemandOfTruck(i) == 0)
                    goto _INITIALIZE_;
                if (answer.GetTotalDemandOfTruck(i) > pinfs.CAPACITY)
                    goto _INITIALIZE_;
            }

            //answer.logging = true;
            return answer;
        }
    }
}
