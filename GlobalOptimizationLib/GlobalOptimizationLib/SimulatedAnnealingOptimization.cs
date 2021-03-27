using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GlobalOptimizationLib
{
    public class SimulatedAnnealingOptimization
    {
        public int initialguesnum { get; set; }
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public double tolerence { get; set; }
        public int maximumiteration { get; set; }
        public int numberofneighbours { get; set; }
        public Func<double[], double> objectfun{get;set;}

        public double[] Optimize()
        {
            //Generate initial guess
            var initialguess = new double[lowerbound.Length];
            
            var minerror = 9999999999999.999;
            for (int i = 0; i < initialguesnum; i++)
            {
                var rnd = new MersenneTwister(i+1, true);
                var temp = new double[lowerbound.Length];
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = (upperbound[j] - lowerbound[j]) * rnd.NextDouble() + lowerbound[j];
                }
                if (i == 1)
                {
                    minerror = objectfun(temp);
                }
                if (i > 1)
                {
                    var error = objectfun(temp);
                    if (error < minerror)
                    {
                        minerror = error;
                        initialguess = temp.Clone() as double[];
                    }
                }
            }

            //Algorithm starts
            var T = 0.0000;
            var mu = 0.0000;
            for (int m = 0; m < maximumiteration; m++)
            {
                T = m / maximumiteration;
                mu=Math.Pow(10,T*100);
            }

        }

        public double[] mu_inv(double[]y,double mu)
        {
            var result = new double[y.Length];
            var sign = 0.000;
            for (int i = 0; i < y.Length; i++)
            {
                sign = y[i] >= 0 ? 1 : 0;
                result[i] = (Math.Pow((1 + mu), Math.Abs(y[i])) - 1) * sign;
            }

            return result;
        }
    }
}
