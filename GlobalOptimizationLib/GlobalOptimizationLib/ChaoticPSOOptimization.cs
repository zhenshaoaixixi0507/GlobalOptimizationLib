using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GlobalOptimizationLib
{
    public class ChaoticPSOOptimization
    {
        //Chaotic particle swarm optimization for data clustering
        //Li-Yeh Chuang a, Chih-Jen Hsiao b, Cheng-Hong Yang
        //https://doi.org/10.1016/j.eswa.2011.05.027
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int numofswarms { get; set; }
        public int maximumiteration { get; set; }
        public double inertiaweightmax { get; set; }
        public double inertiaweightmin { get; set; }
        public double c1 { get; set; }
        public double c2 { get; set; }
        public Func<double[], double> objectfun { get; set; }
        public double tolerance { get; set; }

        public double[] Optimize()
        {
            
            var Vmax = upperbound.Max();
            // Calculate delta for interiaweight
            var detalweight = (inertiaweightmax - inertiaweightmin) / maximumiteration;
            //Generate initial guess
            var globalbest = new double[lowerbound.Length];
            var localswarm = new Dictionary<int, double[]>();
            var localbest = new Dictionary<int, double[]>();
            var Velocity = new Dictionary<int, double[]>();
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
                localswarm.Add(i, temp.Clone() as double[]);
                localbest.Add(i, temp.Clone() as double[]);
                Velocity.Add(i, tempV.Clone() as double[]);
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
            C10 = 0.37;//Randomly selected
            C20 = 0.73;//Randomly selected
           
            var oldglobalerror = minerror;
            var localerror = new double[numofswarms];
            
            for (int i = 0; i < maximumiteration; i++)
            {
                var tempweight = inertiaweightmin + detalweight * i;
                var vector1 = GenerateR(C10);
                var vector2 = GenerateR(C20);
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
                    localswarm[j] = newX.Clone() as double[];
                    Velocity[j] = newV.Clone() as double[];
                    var newlocalbest = swaplocalbest(tempx, newX);
                    localbest[j] = newlocalbest.Clone() as double[];
                    localerror[j] = objectfun(localbest[j]);

                });
                var minIndex = Array.IndexOf(localerror, localerror.Min());
                if (localerror[minIndex] < minerror)
                {
                    globalbest = localbest[minIndex].Clone() as double[];
                    minerror = localerror[minIndex];
                }
                if (Math.Abs(oldglobalerror - minerror) < tolerance && i > 300)
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
        public double[]GenerateR(double c0)
        {
            var results = new double[lowerbound.Length*numofswarms];
            for (int i = 0; i < results.Length; i++)
            {
                results[i] = c0;
                c0= 4 * c0 * (1 - c0);
            }
            return results;
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
