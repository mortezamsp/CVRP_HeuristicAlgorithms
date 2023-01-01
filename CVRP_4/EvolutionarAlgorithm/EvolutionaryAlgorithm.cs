using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class EvolutionaryAlgorithm
    {
        protected ProblemInformations pinfs;

        #region constructor
        public EvolutionaryAlgorithm()
        {
        }
        public EvolutionaryAlgorithm(ProblemInformations p)
        {
            pinfs = p;
        }
        #endregion

        #region best answer
        protected Answer BestAnswer;
        public double QualityOfAnswer(ref Answer answer)
        {
            return answer.GetTotalDistance();
        }
        public double QualityOfAnswer()
        {
            return BestAnswer.GetTotalDistance();
        }
        #endregion

        #region population
        protected Answer[] Population;
        protected int PopulationSize;
        protected virtual Answer GenerateFirstAnswerRandomally(ref Random r)
        {
            return null;
        }
        public void GenerateInitialPopulation(int populationsize, ref Random r)
        {
            PopulationSize = populationsize;

            Population = new Answer[PopulationSize];
            for (int i = 0; i < populationsize; i++)
                Population[i] = GenerateFirstAnswerRandomally(ref r);
        }
        #endregion

        #region tools
        protected int[] SortByQuality(ref double[] array, string type)
        {
            int[] index = new int[array.GetUpperBound(0) + 1];
            for (int i = 0; i < array.GetUpperBound(0) + 1; i++)
                index[i] = i;

            if (type == "dec")
            {
                bool swaped;
                do
                {
                    swaped = false;
                    for (int i = 1; i < array.GetUpperBound(0) + 1; i++)
                        if (array[index[i - 1]] > array[index[i]])
                        {
                            int tmp = index[i - 1];
                            index[i - 1] = index[i];
                            index[i] = tmp;
                            swaped = true;
                        }
                }
                while (swaped);
            }
            else if (type == "inc")
            {
                bool swaped;
                do
                {
                    swaped = false;
                    for (int i = 0; i < array.GetUpperBound(0); i++)
                        if (array[index[i]] < array[index[i + 1]])
                        {
                            int tmp = index[i + 1];
                            index[i + 1] = index[i];
                            index[i] = tmp;
                            swaped = true;
                        }
                }
                while (swaped);
            }

            return index;
        }
        #endregion
    }
}
