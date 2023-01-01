using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class ProblemInformations
    {
        public int DEMENTION;
        public int CAPACITY;
        public int TRUKCSNUM;
        public List<City> cities;

        /// <summary>
        /// containing all informatins of problem
        /// </summary>
        /// <param name="d_">dementions</param>
        /// <param name="c_">capacity</param>
        /// <param name="t_">number of trukctors</param>
        /// <param name="l_">list of cities</param>
        public ProblemInformations(int d_, int c_, int t_, List<City> l_)
        {
            DEMENTION = d_;
            CAPACITY = c_;
            TRUKCSNUM = t_;
            cities = l_;
        }
    }
}
