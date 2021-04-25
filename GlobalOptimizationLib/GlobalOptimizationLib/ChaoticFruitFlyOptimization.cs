using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GlobalOptimizationLib
{
    public class ChaoticFruitFlyOptimization
    {
        //An improved fruit fly optimization algorithm for continuous function optimization problems
        //written by Quan-KePanabHong-YanSangbJun-HuaDuanbLiangGaoc
        //https://www.sciencedirect.com/science/article/abs/pii/S0950705114000781
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int maximumiteration { get; set; }
        public int numofflies { get; set; }
        public double tolerance { get; set; }
        public Func<double[], double> objectfun { get; set; }
        public int sizeofinitialguess { get; set; }
        public double[] Optimize()
        {
            //Calculate Lambda values
            //var lambdamax=10;
            var lambdamin = 0.0000001;
            var u = -9999999999.99;
            var l = 9999999999.99;
            for (int i = 0; i < lowerbound.Length; i++)
            {
                if (upperbound[i] > u)
                {
                    u = upperbound[i];
                }
                if (lowerbound[i] < l)
                {
                    l = lowerbound[i];
                }
            }
            var lambdamax = (u - l) / 2;
            //Initialize the original position
            var X = new Dictionary<int, double[]>();
            var best = new double();
            var xstar = new double[upperbound.Length];
            var location= new double[upperbound.Length]; 
            best = 9999999999999999.99;
            var C00 = 0.19;

            for (int i = 0; i < sizeofinitialguess; i++)
            {
                C00 = 4 * C00 * (1 - C00);

                var tempx = new double[lowerbound.Length];
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    tempx[j] = C00 * (upperbound[j] - lowerbound[j]) + lowerbound[j];
                }
                
                var newerror = objectfun(tempx);

                if (newerror < best)
                {
                    best = newerror;
                    location = tempx.Clone() as double[];
                    xstar = location.Clone() as double[];
                }
            }
            for(int i=0;i<numofflies; i++)
            {
                X.Add(i, xstar.Clone() as double[]);  
            };

            //Main loop
            var bestsmell = new double[numofflies];
            var oldbest = best;
            var templambda=0.0;
            var u0 = 1.00;
            var y0 = 1.00;
            var C10Array = new double[maximumiteration,numofflies];
            var C20Array = new double[maximumiteration, numofflies];
            for (int i = 0; i < maximumiteration; i++)
            {
                for (int j = 0; j < numofflies; j++)
                {
                    y0 = Math.Cos(2 * Math.PI * u0) + y0 * Math.Exp(-3);
                    u0 = (u0 + 400 + 12 * y0) % 1.0;
                    C10Array[i, j] = Math.Min(Math.Max(u0, 0), 1);
                    y0 = Math.Cos(2 * Math.PI * u0) + y0 * Math.Exp(-3);
                    u0 = (u0 + 400 + 12 * y0) % 1.0;
                    C20Array[i, j] = Math.Min(Math.Max(u0, 0), 1);
                }
            }

            for (int i = 0; i < maximumiteration; i++)
            {
                templambda =lambdamax*Math.Exp(Math.Log(lambdamin/lambdamax)*i/maximumiteration);
                Parallel.For(0,numofflies, j=>
                {
                    var d = (int)Math.Floor(C20Array[i,j]*lowerbound.Length);

                    X[j][d] = Math.Max(Math.Min(location[d] + templambda * (C10Array[i, j]*2-1), upperbound[d]), lowerbound[d]);

                    bestsmell[j] = objectfun(X[j]);
                   
                });
                var minIndex = Array.IndexOf(bestsmell, bestsmell.Min());
                if (bestsmell[minIndex] < best)
                {
                    best = bestsmell[minIndex];
                    location = X[minIndex].Clone() as double[];
                    xstar = location.Clone() as double[];

                }

                if (Math.Abs(oldbest - best) <= tolerance && i > Math.Floor((double)maximumiteration /3*2)) 
                {
                    break;
                }
                else
                {
                    oldbest=best;
                }
               
                Console.WriteLine("Error: " + Convert.ToString(best));
            }
            
            return xstar;
        }
       
    }
}
