using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CVRP_4
{
    class Answer
    {
        public int TrucskNum;
        public int Capacity;
        public int Dementions;

        public bool logging;

        #region Constructors
        public Answer(int trucksnum, int capacity, int dementions, List<City>cities_)
        {
            TrucskNum = trucksnum;
            Capacity = capacity;
            Dementions = dementions;

            NumberOfCitiesInTruck = new int[trucksnum];
            Routes = new int[trucksnum, capacity];
            DistCach = new double[dementions, dementions];

            for (int i = 0; i < TrucskNum; i++) NumberOfCitiesInTruck[i] = 0;
            for (int i = 0; i < TrucskNum; i++) for (int j = 0; j < Capacity; j++) Routes[i, j] = -1;
            for (int i = 0; i < Dementions; i++) for (int j = 0; j < Dementions; j++) DistCach[i, j] = 0;

            NumberOfMissedes = 0;
            NumberOfCathedes = 0;

            cities = new List<City>();
            for (int i = 0; i < dementions; i++) cities.Add(cities_[i]);

            logging = true;
        }
        public Answer(ref Answer answer)
        {
            TrucskNum = answer.TrucskNum;
            Capacity = answer.Capacity;
            Dementions = answer.Dementions;

            NumberOfCitiesInTruck = new int[TrucskNum];
            Routes = new int[TrucskNum, Capacity];
            DistCach = new double[Dementions, Dementions];

            for (int i = 0; i < TrucskNum; i++) NumberOfCitiesInTruck[i] = answer.NumberOfCitiesInTruck[i];
            for (int i = 0; i < TrucskNum; i++) for (int j = 0; j < NumberOfCitiesInTruck[i]; j++) Routes[i, j] = answer.Routes[i, j];
            for (int i = 0; i < Dementions; i++) for (int j = 0; j < Dementions; j++) DistCach[i, j] = answer.DistCach[i, j];

            NumberOfMissedes = answer.NumberOfMissedes;
            NumberOfCathedes = answer.NumberOfCathedes;

            cities = new List<City>();
            for (int i = 0; i < Dementions; i++) cities.Add(answer.cities[i]);

            logging = answer.logging;
        }
        public Answer(Answer answer)
        {
            TrucskNum = answer.TrucskNum;
            Capacity = answer.Capacity;
            Dementions = answer.Dementions;

            NumberOfCitiesInTruck = new int[TrucskNum];
            Routes = new int[TrucskNum, Capacity];
            DistCach = new double[Dementions, Dementions];

            for (int i = 0; i < TrucskNum; i++) NumberOfCitiesInTruck[i] = answer.NumberOfCitiesInTruck[i];
            for (int i = 0; i < TrucskNum; i++) for (int j = 0; j < NumberOfCitiesInTruck[i]; j++) Routes[i, j] = Routes[i, j];
            for (int i = 0; i < Dementions; i++) for (int j = 0; j < Dementions; j++) DistCach[i, j] = answer.DistCach[i, j];

            cities = new List<City>();
            for (int i = 0; i < Dementions; i++) cities.Add(answer.cities[i]);

            NumberOfMissedes = answer.NumberOfMissedes;
            NumberOfCathedes = answer.NumberOfCathedes;
        }
        //for computational porpuses only!
        public Answer(ref ProblemInformations pinfs)
        {
            TrucskNum = pinfs.TRUKCSNUM;
            Capacity = pinfs.CAPACITY;
            Dementions = pinfs.DEMENTION;

            NumberOfCitiesInTruck = new int[pinfs.TRUKCSNUM];
            Routes = new int[pinfs.TRUKCSNUM, pinfs.CAPACITY];
            DistCach = new double[pinfs.DEMENTION, pinfs.DEMENTION];

            for (int i = 0; i < TrucskNum; i++) NumberOfCitiesInTruck[i] = 0;
            for (int i = 0; i < TrucskNum; i++) for (int j = 0; j < Capacity; j++) Routes[i, j] = -1;
            for (int i = 0; i < Dementions; i++) for (int j = 0; j < Dementions; j++) DistCach[i, j] = 0;

            NumberOfMissedes = 0;
            NumberOfCathedes = 0;

            cities = new List<City>();
            for (int i = 0; i < pinfs.DEMENTION; i++) cities.Add(pinfs.cities[i]);

            logging = true;
        }
        #endregion

        #region City
        public List<City> cities;
        public int[,] Routes;
        public int[] NumberOfCitiesInTruck;


        public City GetCity(int Trucknum, int CityIndex)
        {
            if (CityIndex > NumberOfCitiesInTruck[Trucknum])
                throw new Exception("City Index Was Wrong!");

            City c = cities[Routes[Trucknum, CityIndex] - 1];
            if (c == null)
                throw new Exception("City Index Was Wrong!");

            return c;
        }
        public City GetCity(int citynumber)
        {
            return cities[citynumber - 1];
        }
        public int SetCity(int Trucknum, int CityIndex, City city)
        {
            //if (IndexOf(Trucknum, ref city) != -1)
            //    return -1;
            Routes[Trucknum, CityIndex] = city.number;
            cities[city.number - 1] = city;

            return 0;
        }/// dangerous to use !

        public int AddCity(int TruckNum, ref City city)
        {
            if(logging) LogWriter.WriteLine("log.txt", "\nadding\t\t[" + TruckNum.ToString() + ":" + NumberOfCitiesInTruck[TruckNum].ToString() + "]");

            if (GetTotalDemandOfTruck(TruckNum) + city.demand > Capacity)
                return -1;

            if (IndexOf(TruckNum, ref city) > -1)
                return -1;

            Routes[TruckNum, NumberOfCitiesInTruck[TruckNum]] = city.number;
            NumberOfCitiesInTruck[TruckNum]++;

            if (logging) PrintTrucks();

            return 1;
        }
        public int AddCity(int TruckNum, City city)
        {
            if (logging) LogWriter.WriteLine("log.txt", "\nadding\t\t[" + TruckNum.ToString() + ":" + NumberOfCitiesInTruck[TruckNum].ToString() + "]");

            if (GetTotalDemandOfTruck(TruckNum) + city.demand > Capacity)
                return -1;

            if (IndexOf(TruckNum, ref city) > -1)
                return -1;

            Routes[TruckNum, NumberOfCitiesInTruck[TruckNum]] = city.number;
            NumberOfCitiesInTruck[TruckNum]++;

            if (logging) PrintTrucks();

            return 1;
        }
        public int AddCity(int TruckNum, int CityNumber)
        {
            if (logging) LogWriter.WriteLine("log.txt", "\nadding\t\t[" + TruckNum.ToString() + ":" + NumberOfCitiesInTruck[TruckNum].ToString() + "]");

            City c = cities[CityNumber - 1];
            if (GetTotalDemandOfTruck(TruckNum) + c.demand > Capacity)
                return -1;

            if (IndexOf(TruckNum, ref c) > -1)
                return -1;

            Routes[TruckNum, NumberOfCitiesInTruck[TruckNum]] = CityNumber;
            NumberOfCitiesInTruck[TruckNum]++;

            if (logging) PrintTrucks();

            return 1;
        }
        public int InsertCityAfter(int TruckNum, City c, City AfterThis)
        {
            //check
            //...
            //
            if (GetTotalDemandOfTruck(TruckNum) + c.demand > Capacity)
                return -1;

            if (IndexOf(TruckNum, ref c) > -1)
                return -1;

            int cindex = IndexOf(TruckNum, ref AfterThis);
            if (cindex != NumberOfCitiesInTruck[TruckNum])
                for (int i = NumberOfCitiesInTruck[TruckNum]; i > cindex + 1; i--)
                    Routes[TruckNum, i] = Routes[TruckNum, i - 1];
            Routes[TruckNum, cindex + 1] = c.number;
            NumberOfCitiesInTruck[TruckNum]++;

            return 1;
        }

        public int RemoveCity(int TruckNum, ref City city)
        {
            int index = IndexOf(TruckNum, ref city);
            return RemoveCity(TruckNum, index);
        }
        public int RemoveCity(int TruckNum, int CityIndex)
        {
            if (CityIndex < 0)
                return 0;

            if (logging) LogWriter.WriteLine("log.txt", "\nremoveing\t\t[" + TruckNum.ToString() + ":" + CityIndex.ToString() + "]");

            for (int i = CityIndex; i < NumberOfCitiesInTruck[TruckNum] - 1; i++)
                Routes[TruckNum, i] = Routes[TruckNum, i + 1];
            NumberOfCitiesInTruck[TruckNum]--;

            if (logging) PrintTrucks();

            return 1;
        }
        public int RemoveCity(int citynumber)
        {
            int[] idx = IndexOf(citynumber);
            if (idx[0] == -1)
                return -1;
            return RemoveCity(idx[0], idx[1]);
        }
        public int RemoveLastCity(int TruckNum)
        {
            if (logging) LogWriter.WriteLine("log.txt", "\nremoveing\t\t[" + TruckNum.ToString() + ":" + NumberOfCitiesInTruck[TruckNum].ToString() + "]");

            NumberOfCitiesInTruck[TruckNum]--;

            if (logging) PrintTrucks();

            return 1;
        }
        public int RemoveCitiesAfter(int trucknum, int CityIndex)
        {
            if (CityIndex >= NumberOfCitiesInTruck[trucknum])
                throw new Exception("INDEX IS INCORRECT!");

            NumberOfCitiesInTruck[trucknum] = CityIndex + 1;
            return 1;
        }

        /// 
        /// توابع ماست مالی کننده
        public int SetCityNull(int citynumber)
        {
            int[] idx = IndexOf(citynumber);
            if (idx[0] == -1)
                return -1;

            Routes[idx[0], idx[1]] = -1;
            return 0;
        }
        public int SetCityNull(int trucknum,int cityindex)
        {
            Routes[trucknum, cityindex] = -1;
            return 0;
        }
        public int FillNullCityBy(City c)
        {
            for (int i = 0; i < TrucskNum; i++)
                for (int j = 0; j < NumberOfCitiesInTruck[i]; j++)
                    if (Routes[i, j] == -1)
                    {
                        Routes[i, j] = c.number;
                        return 0;
                    }
            return -1;
        }
        #endregion

        #region distance
        public double[,] DistCach;
        public int NumberOfMissedes;
        public int NumberOfCathedes;

        public double GetDistance(int TruckNum1, ref City City1, int TruckNum2, ref City City2)
        {
            int idx1 = City1.number - 1;
            int idx2 = City2.number - 1;

            if (DistCach[idx1, idx2] == 0)
            {
                NumberOfMissedes++;
                double dist = Math.Sqrt(Math.Pow(City1.x - City2.x, 2) + Math.Pow(City1.y - City2.y, 2));
                DistCach[idx1, idx2] = dist;
                DistCach[idx2, idx1] = dist;
                return dist;
            }

            NumberOfCathedes++;
            return DistCach[idx1,idx2];
        }
        public double GetDistance(int TruckNum1, int IndexOfCity1, int TruckNum2, int IndexOfCity2)
        {
            int numberOfCity1 = Routes[TruckNum1, IndexOfCity1] - 1;
            int numberOfCity2 = Routes[TruckNum2, IndexOfCity2] - 1;
            if (numberOfCity1 < 0 || numberOfCity2 < 0) return 0;
            if (DistCach[numberOfCity1, numberOfCity2] == 0)
            {
                NumberOfMissedes++;
                City City1 = cities[numberOfCity1];
                City City2 = cities[numberOfCity2];
                double dist = Math.Sqrt(Math.Pow(City1.x - City2.x, 2) + Math.Pow(City1.y - City2.y, 2));
                DistCach[numberOfCity1, numberOfCity2] = dist;
                DistCach[numberOfCity2, numberOfCity1] = dist;
                return dist;
            }

            NumberOfCathedes++;
            return DistCach[numberOfCity1, numberOfCity2];
        }
        public double GetDistance(int indexofCity1, int indexofCity2)
        {
            City City1 = cities[indexofCity1];
            City City2 = cities[indexofCity2];
            return  Math.Sqrt(Math.Pow(City1.x - City2.x, 2) + Math.Pow(City1.y - City2.y, 2));
        }
        public double GetTotalDistanceOfTruck(int Trucknum)
        {
            double tot = 0;
            for (int i = 1; i < NumberOfCitiesInTruck[Trucknum]; i++)
                tot += GetDistance(Trucknum, i, Trucknum, i - 1);
            tot += GetDistance(Trucknum, 0, Trucknum, NumberOfCitiesInTruck[Trucknum] - 1);

            return tot;
        }
        public double GetTotalDistance()
        {
            double tot = 0;
            for (int i = 0; i < TrucskNum; i++)
                tot += GetTotalDistanceOfTruck(i);
            
            return tot;
        }
        #endregion

        #region demand
        //private int[] TotalDemands;
        public int GetTotalDemandOfTruck(int Trucknum)
        {
            int tot=0;
            for (int i = 0; i < NumberOfCitiesInTruck[Trucknum]; i++)
                tot += cities[Routes[Trucknum, i] - 1].demand;
            return tot;
        }
        public int GetTotalDemand()
        {
            int tot = 0;
            for (int i = 0; i < TrucskNum; i++)
                tot += GetTotalDemandOfTruck(i);
            return tot;
        }
        public bool TrucksAreFull()
        {
            int tot = 0;
            for (int i = 0; i < TrucskNum; i++)
                tot += NumberOfCitiesInTruck[i];
            return tot >= Dementions ? true : false;
        }
        #endregion

        #region operations
        public int ReplaceCities(int trucknum1, City city1, int trucknum2, City city2)
        {
            //if (trucknum1 == trucknum2)
            //    return -1;
            if (IndexOf(trucknum1, ref city1) < 0 || IndexOf(trucknum2, ref city2) < 0)
                return -1;
            //first cities could not be replaced
            if (city1.number == 1 || city2.number == 1)
                return -1;
            //replace if capacities  are enough
            if (GetTotalDemandOfTruck(trucknum1) - city1.demand + city2.demand > Capacity
                ||
                GetTotalDemandOfTruck(trucknum2) - city2.demand + city1.demand > Capacity)
                return -1;

            int indexofcity1 = IndexOf(trucknum1, ref city1);
            int indexofcity2 = IndexOf(trucknum2, ref city2);

            if (logging)
                LogWriter.WriteLine("log.txt", "\nreplaceing\t\t[" + trucknum1.ToString() + ":" + indexofcity1.ToString() + "]<->[" +
                    trucknum2.ToString() + ":" + indexofcity2.ToString() + "]");

            int tmp = city1.number;
            Routes[trucknum1, indexofcity1] = city2.number;
            Routes[trucknum2, indexofcity2] = tmp;


            if(logging)
                PrintTrucks();

            return 1;
        }
        public int Validation()
        {
            int t = 0;
            for (int i = 0; i < TrucskNum; i++) t += NumberOfCitiesInTruck[i];
            if (t - TrucskNum + 1 > Dementions)
                return -1;

            t = 0;
            for (int i = 0; i < TrucskNum; i++)
            {
                for (int j = 0; j < NumberOfCitiesInTruck[i]; j++)
                    t += cities[Routes[i, j] - 1].demand;
                if (t > Capacity)
                    return -1;
            }
            return 0;
        }
        #endregion

        #region search

        public int IndexOf(int TruckNum, ref City city)
        {
            for (int i = 0; i < NumberOfCitiesInTruck[TruckNum]; i++)
                if (Routes[TruckNum, i] == city.number)
                    return i;
            return -1;
        }
        public int[] IndexOf(int citynumber)
        {
            for (int i = 0; i < TrucskNum; i++)
                for (int j = 0; j < NumberOfCitiesInTruck[i]; j++)
                    if (Routes[i, j] == citynumber)
                        return new int[] { i, j };
            return new int[] { -1, -1 };
        }
        public int IndexOfMaxTruck()
        {
            double maxv = 0;
            int indx = -1;
            for (int i = 0; i < TrucskNum; i++)
            {
                double v = GetTotalDistanceOfTruck(i);
                if (v > maxv)
                {
                    maxv = v;
                    indx = i;
                }
            }
            return indx;
        }
        public int IndexOfMinTruck()
        {
            double minv = 100000;
            int indx = -1;
            for (int i = 0; i < TrucskNum; i++)
            {
                double v = GetTotalDistanceOfTruck(i);
                if (v < minv)
                {
                    minv = v;
                    indx = i;
                }
            }
            return indx;
        }
        public int IndexOfMaxDemandTruck()
        {
            double maxv = 0;
            int indx = -1;
            for (int i = 0; i < TrucskNum; i++)
            {
                double v = GetTotalDemandOfTruck(i);
                if (v > maxv)
                {
                    maxv = v;
                    indx = i;
                }
            }
            return indx;
        }
        public int IndexOfMinDemandTruck()
        {
            double minv = 100000;
            int indx = -1;
            for (int i = 0; i < TrucskNum; i++)
            {
                double v = GetTotalDemandOfTruck(i);
                if (v < minv)
                {
                    minv = v;
                    indx = i;
                }
            }
            return indx;
        }

        #endregion

        #region print
        public void PrintTruck(int trucknum)
        {
            if (trucknum > TrucskNum)
                throw new Exception("WRONG TRUCK NUMBER!");

            LogWriter.Write("log.txt", "\n\tTruck" + trucknum.ToString() + ":\t\t");
            for(int i=0;i<Dementions;i++)
                try { LogWriter.Write("log.txt", Routes[trucknum, i].ToString() + ","); }
                catch { LogWriter.Write("log.txt", "-,"); }
        }
        public void PrintTrucks()
        {
            for (int i = 0; i < TrucskNum; i++)
                PrintTruck(i);
        }
        #endregion

    }
}
