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
        public double initialacceptrate { get; set; }
        public double alpha { get; set; }

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
            var x0 = initialguess.Clone() as double[];
            var x = initialguess.Clone() as double[];
            var fx0 = minerror;
            var fx = minerror;
            var oldminerror=0.0;
            for (int m = 0; m < maximumiteration; m++)
            {
                T = m / maximumiteration;
               
                mu=Math.Pow(10,T*100);
                for (int k = 0; k < numberofneighbours; k++)
                {
                    var dx = getdx(mu, x, upperbound, lowerbound, k);
                    var x1 = arrayplus(x, dx);
                    x1 = constrainx(x1, upperbound, lowerbound);
                    var fx1 = objectfun(x1);
                    var df = fx1 - fx;
                    if (validateX(df, T, fx, k))
                    {
                        x = x1.Clone() as double[];
                        fx = fx1;
                    }

                    if (fx1 < minerror)
                    {
                        x0 = x1;
                        fx0 = fx1;
                    }
                }
                if (m > 1 && Math.Abs(minerror - oldminerror) < tolerence)
                {
                    break;
                }
                else
                {
                    oldminerror = minerror;
                    minerror = fx0;
                }

                Console.WriteLine("Error: " + Convert.ToString(minerror));
            }
            return x0;

        }

        public bool validateX(double df, double T, double oldf,int k)
        {
            var result = false;
            if (df<0)
            {
                var rnd = new MersenneTwister(k + 1);
                var p = rnd.NextDouble();
                var p1 = Math.Exp(-T * df / (Math.Abs(oldf) + 0.000000000001) / tolerence);
                if (p < p1)
                {
                    result = true;
                }
            }

            return result;
        }
        public double[] constrainx(double[] x, double[] u, double[] l)
        {
            var newx = new double[x.Length];
            for (int i = 0; i < newx.Length; i++)
            {
                if (x[i] < l[i])
                {
                    newx[i] = l[i];
                }
                if (x[i] > u[i])
                {
                    newx[i] = u[i];
                }
                if (x[i] >= l[i] && x[i] <= u[i])
                {
                    newx[i] = x[i];
                }
            }
            return newx;
        }
        public double[] arrayplus(double[]x,double[]y)
        {
            var result = new double[x.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = x[i] + y[i];
            }
            return result;
        }

        public double[] getdx(double mu,double[] x, double[] u, double[] l, int k)
        {
            var result = new double[x.Length];
            var rnd = new MersenneTwister(k + 1);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = 2 * rnd.NextDouble() - 1;
            }

            result = mu_inv(result, mu);
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = result[i] * (u[i] - l[i]);
            }
            return result;
        }
        
        
        public double[] mu_inv(double[]y,double mu)
        {
            var result = new double[y.Length];
            var sign = 0.000;
            for (int i = 0; i < y.Length; i++)
            {
                sign = y[i] >= 0 ? 1 : 0;
                result[i] = ((Math.Pow((1 + mu), Math.Abs(y[i])) - 1))/mu * sign;
            }

            return result;
        }
    }
}
