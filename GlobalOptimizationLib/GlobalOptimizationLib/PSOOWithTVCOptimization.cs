using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace GlobalOptimizationLib
{
    public class PSOOWithTVCOptimization
    {
        //Particle Swarm Optimization Method for Constrained Optimization Problems
        //written by Konstantinos E. Parsopoulos and Michael N. Vrahtis
        //https://www.cs.cinvestav.mx/~constraint/papers/eisci.pdf
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int maximumiteration { get; set; }
        public int numofswarms { get; set; }
        public double inertiaweightmax { get; set; }
        public double inertiaweightmin { get; set; }
        public double chi { get; set; }
        public Func<double[], double> objectfun { get; set; }
        public double tolerance { get; set; }
        public double Vmax { get; set; }
        public double[] Optimize()
        {
            // Calculate delta for interiaweight
            var detalweight = (inertiaweightmax - inertiaweightmin) / maximumiteration;
            //Generate initial guess
            var globalbest = new double[lowerbound.Length];
            var localswarm = new Dictionary<int, double[]>();
            var localbest = new Dictionary<int, double[]>();
            var Velocity = new Dictionary<int, double[]>();
            var minerror = 9999999999999.999;

            var u10 = 0.2;
            var u20 = 0.94;
            for (int i = 0; i < numofswarms; i++)
            {
               
                var temp = new double[lowerbound.Length];
                var tempV = new double[lowerbound.Length];
                for (int j = 0; j < temp.Length; j++)
                {
                    u10= 4 * u10 * (1 - u10);
                    u20 = 4 * u20 * (1 - u20);
                    temp[j] = (upperbound[j] - lowerbound[j]) * u10 + lowerbound[j];
                    tempV[j] = 2 * Vmax * u20 - Vmax;
                }
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
            var y10 = 0.35;
            var y20 = 0.63;
            var oldglobalerror = minerror;
            var r1 = new double[upperbound.Length];
            var r2 = new double[upperbound.Length];
            for (int i = 0; i < maximumiteration; i++)
            {
                var tempweight = inertiaweightmin + detalweight * i;
                var c1 = 2 * Math.Sin(1 - (i / maximumiteration) * Math.PI / 2) + 0.5;
                var c2 = 2 * Math.Cos(1 - (i / maximumiteration) * Math.PI / 2) + 0.5;
                for (int j = 0; j < numofswarms; j++)
                {
                    var tempx = localswarm[j].Clone() as double[];
                    var tempV = Velocity[j].Clone() as double[];
                    var templocalbest = localbest[j].Clone() as double[];
                    (y10, r1) = GenerateR(y10, tempx.Length);
                    (y20, r2) = GenerateR(y20, tempV.Length);
                    var item1 = ArrayMultiplyConstant(tempV, tempweight);
                    var item2 = ArrayMultiplyConstant(r1, c1);
                    item2 = ArrayMultiplyArray(item2, ArrayMinus(templocalbest, tempx));
                    var item3 = ArrayMultiplyConstant(r2, c2);
                    item3 = ArrayMultiplyArray(item3, ArrayMinus(globalbest, tempx));
                    item1 = ArrayPlus(item1, item2);
                    item1 = ArrayPlus(item1, item3);
                    var newV = ArrayMultiplyConstant(item1, chi);
                    newV = ConstrainV(newV, Vmax);
                    var newX = ArrayPlus(tempx, newV);
                    newX = ConstrainX(newX);
                    localswarm[j] = newX.Clone() as double[];
                    Velocity[j] = newV.Clone() as double[];
                    var newlocalbest = swaplocalbest(tempx, newX);
                    localbest[j] = newlocalbest.Clone() as double[];
                    var localerror = objectfun(localbest[j]);
                    if (localerror < minerror)
                    {
                        globalbest = localbest[j].Clone() as double[];
                        minerror = localerror;
                    }
                }
                if (Math.Abs(oldglobalerror - minerror) < tolerance && i > 50)
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
        public (double,double[]) GenerateR(double y0,int length)
        { 
            
            var result = new double[length];
            for (int i = 0; i < length; i++)
            {
                y0 = 4 * y0 * (1 - y0);
                result[i] = y0;
            }
            return (y0,result);
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
