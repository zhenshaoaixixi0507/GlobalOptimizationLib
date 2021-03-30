﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace MarketDataFittingTool.OptimizationAlgorithmLib
{
    public class FruitFlyAlgorithm
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
                var rnd1 = new MersenneTwister(i + 1, true);
                var rnd2 = new MersenneTwister(i + 2, true);
                var tempx = new double[lowerbound.Length];
                var tempy = new double[lowerbound.Length];
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    tempx[j] = rnd1.NextDouble()*10;
                    tempy[j] = rnd2.NextDouble()*10;
                }
                X.Add(i, tempx.Clone() as double[]);
                Y.Add(i, tempy.Clone() as double[]);
                var tempD = CalculateD(tempx, tempy);
                var tempSol = CaculateSolution(tempD);
                //var newerror = objectfun(tempSol);
                var newerror = 0.0;

                if (CheckParameters(tempSol) == false)
                {
                    newerror = 999999999999999999.99;
                }
                else
                {
                    newerror = objectfun(tempSol);
                }
                if (newerror < best)
                {
                    best = newerror;
                    smellbest = tempSol;
                    newX = tempx.Clone() as double[];
                    newY = tempy.Clone() as double[];
                }
            }

            //Main loop
            var bestsmell = best;
            var oldbest = best;
            for (int i = 0; i < maximumiteration; i++)
            {
                
                for (int j = 0; j < numofflies; j++)
                {
                    var rnd1 = new MersenneTwister(i + j + 1, true);
                    var rnd2 = new MersenneTwister(i + j + 2, true);
                    var xdistance = GenerateDistance(rnd1);
                    var ydistance = GenerateDistance(rnd2);
                    X[j] = ArrayPlus(newX, xdistance).Clone() as double[];
                    Y[j] = ArrayPlus(newY, ydistance).Clone() as double[];
                    var tempD = CalculateD(X[j], Y[j]);
                    var tempSol = CaculateSolution(tempD);
                    var newerror = 0.0;

                    if (CheckParameters(tempSol) == false)
                    {
                        newerror = 999999999999999999.99;
                    }
                    else
                    {
                        newerror = objectfun(tempSol);
                    }
                    //var newerror = objectfun(tempSol);
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
                if (Math.Abs(oldbest - best) <= tolerance && i>5000)
                {
                    break;
                }
                if (Math.Abs(oldbest - best) > tolerance)
                {
                    oldbest = best;
                }
                Console.WriteLine("Error: " + Convert.ToString(best));
            }
            var finalsolution = CaculateSolution(CalculateD(newX, newY));
            return finalsolution;
        }
        public bool CheckParameters(double[] p)
        {
            var result = true;
            for (int i = 0; i < p.Length; i++)
            {
                if (p[i] > upperbound[i])
                {
                    result = false;
                    break;
                }
                if (p[i] < lowerbound[i])
                {
                    result = false;
                    break;
                }
            }
            return result;
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
                result[i] =rnd.NextDouble()*2-1;
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
