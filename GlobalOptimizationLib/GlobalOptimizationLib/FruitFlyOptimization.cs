using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GlobalOptimizationLib
{
    public class FruitFlyOptimization
    {
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int maximumiteration { get; set; }
        public int numofflies { get; set; }
        public double tolerance { get; set; }
        public Func<double[], double> objectfun { get; set; }
        public double[] Optimize()
        {
            //Initialize the original position
            var X = new Dictionary<int, double[]>();
            var Y = new Dictionary<int, double[]>();
            var smellbest = new double[upperbound.Length];
            var best = new double();
            var newX = new double[upperbound.Length];
            var newY = new double[upperbound.Length];
            best = 9999999999999999.99;
            for (int i = 0; i < numofflies; i++)
            { 
                var rnd1= new MersenneTwister(i + 1, true);
                var rnd2 = new MersenneTwister(i + 2, true);
                var tempx = new double[lowerbound.Length];
                var tempy = new double[lowerbound.Length];
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    tempx[j] = rnd1.NextDouble()*(upperbound[j]-lowerbound[j])+lowerbound[j];
                    tempy[j] = rnd2.NextDouble()*(upperbound[j] - lowerbound[j]) + lowerbound[j];
                }
                X.Add(i, tempx.Clone() as double[]);
                Y.Add(i, tempy.Clone() as double[]);
                var tempD = CalculateD(tempx, tempy);
                var tempSol = CaculateSolution(tempD);
                var newerror = objectfun(tempSol);
                if (newerror < best)
                {
                    best = newerror;
                    smellbest = tempSol;
                    newX = tempx.Clone() as double[];
                    newY = tempy.Clone() as double[];
                }
            }

            //Main loop
            var bestsmell = 999999999999.99;

            for (int i = 0; i < maximumiteration; i++)
            {
                for (int j = 0; j < numofflies; j++)
                {
                    var rnd1 = new MersenneTwister(i+j + 1, true);
                    var rnd2 = new MersenneTwister(i+j + 2, true);
                    var xdistance = GenerateDistance(rnd1);
                    var ydistance = GenerateDistance(rnd2);
                    X[j] = ArrayPlus(newX, xdistance);
                    Y[j] = ArrayPlus(newY, ydistance);
                    var tempD = CalculateD(X[j], Y[j]);
                    var tempSol = CaculateSolution(tempD);
                    var newerror = objectfun(tempSol);
                    if (newerror < bestsmell)
                    {
                        bestsmell = newerror;
                        smellbest = tempSol;
                        newX = X[j];
                        newY = Y[j];
                    }
                }
                if (bestsmell < best)
                {
                    best = bestsmell;
                }
                Console.WriteLine("Error: " + Convert.ToString(best));
            }
            var finalsolution = CaculateSolution(CalculateD(newX, newY));
            return finalsolution;
        }
        public double[] ArrayPlus(double[] x, double[] y)
        {
            var result = new double[x.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = x[i] + y[i];
            }
            return result;
        }
        public double[] GenerateDistance(MersenneTwister rnd)
        {
            var result = new double[lowerbound.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = rnd.NextDouble() * (upperbound[i] - lowerbound[i]) + lowerbound[i];
            }
            return result;
        }

        public double[] CalculateD(double[] x, double[] y)
        {
            var result = new double[x.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Sqrt(x[i] * x[i] + y[i] * y[i]);
            }
            return result;
        }
        public double[] CaculateSolution(double[] D)
        {
            var result = new double[D.Length];
            for (int i = 0; i < D.Length; i++)
            {
                result[i] = 1 / (D[i] + 0.0000000000001);
            }
            return result;
        }
    }
}
