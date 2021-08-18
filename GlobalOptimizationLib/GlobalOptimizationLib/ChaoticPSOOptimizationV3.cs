using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GlobalOptimizationLib
{
    public class ChaoticPSOOptimizationV3
    {
        //Shape optimization of structures with cutouts by an efficient approach based on XIGA
        //and chaotic particle swarm optimization

        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int numofswarms { get; set; }
        public int maximumiteration { get; set; }
        
        public Func<double[], double> objectfun { get; set; }
        public double tolerance { get; set; }

        public double[] Optimize()
        {
            
            var Vmax = upperbound.Max();
            // Calculate delta for interiaweight
            var inertiaweightmax = 0.9;
            var inertiaweightmin = 0.4;
            var c1 = 2;
            var c2 = 2;
            var detalweight = (inertiaweightmax - inertiaweightmin) / maximumiteration;
            //Generate initial guess
            var globalbest = new double[lowerbound.Length];
            var localswarm = new double[numofswarms][];
            var localbest = new double[numofswarms][];
            var Velocity = new double[numofswarms][];
            var minerror = 9999999999999.999;

            var C10 = 0.21;//Randomly selected
            var C20 = 0.63;//Randomly selected
            for (int i = 0; i < numofswarms; i++)
            {

                C10 = 4 * C10 * (1 - C10);
                C20 = 4 * C20 * (1 - C20);
                var temp = new double[lowerbound.Length];
                var tempV = new double[lowerbound.Length];
                Parallel.For(0, temp.Length, j =>
                {
                    temp[j] = (upperbound[j] - lowerbound[j]) * C20 + lowerbound[j];
                    tempV[j] = 2 * Vmax * C10 - Vmax;
                });
                localswarm[i] = temp;
                localbest[i] = temp;
                Velocity[i] = tempV;

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
                        globalbest = temp.Clone() as double[];
                    }

                }
            }

            //Iteration starts
            var u0 = 1.00;
            var y0 = 1.00;
            var oldglobalerror = minerror;
            var localerror = new double[numofswarms];
            
            for (int i = 0; i < maximumiteration; i++)
            {
                var tempweight = inertiaweightmin + detalweight * i;
                var vector1 = new double[lowerbound.Length * numofswarms];
                var vector2 = new double[lowerbound.Length * numofswarms];
                (u0,y0,vector1) = GenerateR(u0,y0);
                (u0, y0, vector2) = GenerateR(u0,y0);
                var tempsolutionR = new double[lowerbound.Length];
                var tempvR = new double[lowerbound.Length];
                Parallel.For(0, numofswarms, j =>
                {

                    var tempx = localswarm[j].Clone() as double[];
                    var tempV = Velocity[j].Clone() as double[];
                    var templocalbest = localbest[j].Clone() as double[];
                    var item1 = ArrayMultiplyConstant(tempV, tempweight);
                    Array.Copy(vector1, lowerbound.Length * j,tempsolutionR, 0,lowerbound.Length);
                    Array.Copy(vector2, lowerbound.Length * j,tempvR, 0,lowerbound.Length);
                    var item2 = ArrayMultiplyArray(ArrayMinus(templocalbest, tempx), ArrayMultiplyConstant(tempsolutionR, c1));
                    var item3 = ArrayMultiplyArray(ArrayMinus(globalbest, tempx), ArrayMultiplyConstant(tempvR, c2));
                    item1 = ArrayPlus(item1, item2);
                    item1 = ArrayPlus(item1, item3);
                    var newV = ArrayMultiplyConstant(item1, 1);
                    newV = ConstrainV(newV, Vmax);
                    var newX = ArrayPlus(tempx, newV);
                    newX = ConstrainX(newX);
                    localswarm[j] = newX;
                    Velocity[j] = newV;
                    var newlocalbest = swaplocalbest(tempx, newX);
                    localbest[j] = newlocalbest;
                    localerror[j] = objectfun(localbest[j]);

                });
                var minIndex = Array.IndexOf(localerror, localerror.Min());
                if (localerror[minIndex] < minerror)
                {
                    globalbest = localbest[minIndex].Clone() as double[];
                    minerror = localerror[minIndex];
                }
                if (Math.Abs(oldglobalerror - minerror) < tolerance && i > 500)
                {
                    break;
                }
                else
                {
                    oldglobalerror = minerror;
                    Console.WriteLine("Iteration Time: "+Convert.ToString(i)+" Objective Function Value: " + Convert.ToString(minerror));
                }
                
            }

            return globalbest;
        }

        public (double,double,double[]) GenerateR(double u0,double y0)
        {
            var results = new double[lowerbound.Length*numofswarms];
            for (int i = 0; i < results.Length; i++)
            {
                y0 = Math.Cos(2 * Math.PI * u0) + y0 * Math.Exp(-3);
                u0 = (u0 + 400 + 12 * y0) % 1.0;
                results[i] = Math.Min(Math.Max(u0, 0), 1);
            }
            return (u0,y0, results);
        }
        public double[] ConstrainX(double[] x)
        {
            var result = new double[x.Length];
            for (int i = 0; i < result.Length; i++)
            { 
                result[i]= Math.Min(Math.Max(x[i], lowerbound[i]), upperbound[i]);
            }
            return result;
        }
        public double[] ConstrainV(double[] v, double Vmax)
        {
            var result = new double[v.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Math.Min(Math.Max(v[i], -Vmax), Vmax);
            }
            return result;
        }
        public double[] swaplocalbest(double[] oldx, double[] newx)
        {
            var result = new double[oldx.Length];
            var olderror = objectfun(oldx);
            var newerror = objectfun(newx);
            if (newerror < olderror)
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = newx[i];
                }
            }
            else
            {
                for (int i = 0; i < result.Length; i++)
                {
                    result[i] = oldx[i];
                }
            }
            return result;
        }
        public double[] ArrayMinus(double[]x,double[]y)
        {
            var result = new double[x.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = x[i] - y[i];
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
        public double[] ArrayMultiplyConstant(double[] x, double c)
        {
            var result = new double[x.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = x[i] * c;
            }
            return result;
        }
        public double[] ArrayMultiplyArray(double[] x, double[] y)
        {
            var result = new double[x.Length];
            for (int i = 0; i < x.Length; i++)
            {
                result[i] = x[i] * y[i];
            }
            return result;
        }
    }
}
