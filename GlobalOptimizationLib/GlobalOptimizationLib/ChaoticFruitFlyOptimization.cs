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
            var lambdamin = 0.00001;
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
            var C10 = 0.45;
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
            for (int i = 0; i < numofflies; i++)
            {
                C10 = 4 * C10 * (1 - C10) ;
                var tempx = new double[lowerbound.Length];              
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    tempx[j] = C10 * (upperbound[j]-lowerbound[j])+lowerbound[j];      
                }
                X.Add(i,tempx.Clone() as double[]);
            }

            //Main loop
            var bestsmell = best;
            var oldbest = best;
            var templambda=0.0;
            var C30 = 0.68;
            for (int i = 0; i < maximumiteration; i++)
            {
                templambda=lambdamax*Math.Exp(Math.Log(lambdamin/lambdamax)*i/maximumiteration);
                for (int j = 0; j < numofflies; j++)
                {
                    var d = GenerateInteger(i + j + 1, lowerbound.Length);
                    C30 = 4 * C30 * (1 - C30);
                    var r1 = C30 * 2 - 1;

                    X[j][d] = Math.Max(Math.Min(location[d] + templambda * r1, upperbound[d]), lowerbound[d]);

                    bestsmell = objectfun(X[j]);
                    if (bestsmell<best)
                    {
                        best = bestsmell;
                        location = X[j].Clone() as double[];
                        xstar =location.Clone() as double[];
                        
                    }
                   
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

        public int GenerateInteger(int seed,int numofpara)
        {
            var rnd = new MersenneTwister(seed, true);
            return rnd.Next(0,numofpara);
        }
       
    }
}
