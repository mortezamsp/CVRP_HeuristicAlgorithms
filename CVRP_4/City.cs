using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class City
    {
        public int number;
        public int x;
        public int y;
        public int demand;

        public City(int number_, int x_, int y_, int demand_)
        {
            number = number_;
            x = x_;
            y = y_;
            demand = demand_;
        }
        public City(int number_, int x_, int y_)
        {
            number = number_;
            x = x_;
            y = y_;
        }
        public City(ref City city)
        {
            number = city.number;
            x = city.x;
            y = city.y;
            demand = city.demand;
        }
        public City(City city)
        {
            number = city.number;
            x = city.x;
            y = city.y;
            demand = city.demand;
        }

        public bool IsEqualWith(ref City city1)
        {
            if (number == city1.number && x == city1.x && y == city1.y && demand == city1.demand)
                return true;
            return false;
        }
    }
}
