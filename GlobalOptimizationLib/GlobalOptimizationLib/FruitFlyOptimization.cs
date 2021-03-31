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
            //Calculate Lambda values
            var lambdamax=0.0;
            var lambdamin=0.00001;
            var u=-9999999999.99;
            var l=9999999999.99;
            for(int i=0;i<lowerbound.Length;i++)
            {
                if(upperbound[i]>u)
                {
                    u=upperbound[i];
                }
                if(lowerbound[i]<l)
                {
                    l=lowerbound[i];
                }
            }
            lambdamax=(u-l)/2;
            //Initialize the original position
            var X = new Dictionary<int, double[]>();
            var Y = new Dictionary<int, double[]>();
            var best = new double();
            var xstar = new double[upperbound.Length];
            var location= new double[upperbound.Length]; 
            best = 9999999999999999.99;
            for (int i = 0; i < numofflies; i++)
            {
                var rnd1 = new MersenneTwister(i + 1, true);
                var tempx = new double[lowerbound.Length];              
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    tempx[j] = rnd1.NextDouble()*(upperbound[j]-lowerbound[j])+lowerbound[j];      
                }

                var newerror = objectfun(tempx);

                if (newerror < best)
                {
                    best = newerror;
                    location = tempx.Clone() as double[];
                    xstar=location.Clone() as double[];
                }
                X.Add(i,tempx.Clone() as double[]);
                Y.Add(i,tempx.Clone() as double[]);
            }

            //Main loop
            var bestsmell1 = best;
            var bestsmell2=best;
            var oldbest = best;
            var templambda=0.0;
            for (int i = 0; i < maximumiteration; i++)
            {
                templambda=lambdamax*Math.Exp(Math.Log(lambdamin/lambdamax)*i/maximumiteration);
                for (int j = 0; j < numofflies; j++)
                {
                    var d= GenerateInteger(i+j+1,lowerbound.Length);
                    var rnd1=new MersenneTwister(i+j+1,true);
                    var rnd2 = new MersenneTwister(i + j + 2, true);
                    var r1=rnd1.NextDouble();
                    var r2 = rnd2.NextDouble();
                    X[j][d]=Math.Max(Math.Min(location[d]+templambda*r1,upperbound[d]),lowerbound[d]);
                    Y[j][d]=Math.Max(Math.Min(location[d]-templambda*r2,upperbound[d]),lowerbound[d]);
                    bestsmell1=objectfun(X[j]);
                    bestsmell2=objectfun(Y[j]);
                    if(bestsmell1<best)
                    {
                        location=X[j].Clone() as double[];
                        xstar=location.Clone() as double[];
                        best=bestsmell1;
                    }
                    if(bestsmell2<best)
                    {
                        location=Y[j].Clone() as double[];
                        xstar=location.Clone() as double[];
                        best=bestsmell2;
                    }
                   
                }


                if (Math.Abs(oldbest - best) <= tolerance && i > Math.Floor((double)maximumiteration / 3*2)) 
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
