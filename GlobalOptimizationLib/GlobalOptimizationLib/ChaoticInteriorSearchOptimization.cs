using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;

namespace GlobalOptimizationLib
{
    public class ChaoticInteriorSearchOptimization
    {
        
        //https://www.sciencedirect.com/science/article/abs/pii/S0019057814000597
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int maximumiteration { get; set; }
        public int locationsize { get; set; }
        public double tolerance { get; set; }
        public Func<double[], double> objectfun { get; set; }
        public int sizeofinitialguess { get; set; }
        public double alphamin { get; set; }
        public double alphamax { get; set; }
        public double[] Optimize()
        {
            
            //Initialize 
            var best = new double();
            var globalbest = new double[lowerbound.Length];
            best = 9999999999999999.99;
            var C00 = 0.19;
            for (int i = 0; i < sizeofinitialguess; i++)
            {
                //var rnd = new MersenneTwister(i + 3, true);
                var tempx = new double[lowerbound.Length];
                C00 = 4 * C00 * (1 - C00);
                for (int j = 0; j < lowerbound.Length; j++)
                {
                   
                    tempx[j] = C00 * (upperbound[j] - lowerbound[j]) + lowerbound[j];
                    //tempx[j] = rnd.NextDouble() * (upperbound[j] - lowerbound[j]) + lowerbound[j];
                }
                var newerror = objectfun(tempx);

                if (newerror < best)
                {
                    best = newerror;
                    globalbest = tempx.Clone() as double[];

                }
            }
           
            var component=globalbest.Clone() as double[];
            var mirror=globalbest.Clone() as double[];
            var oldbest = best;
            //Iteration starts
            var C10 = 0.31;
            var C20 = 0.13;
            var C30 = 0.87;
            for(int i=0;i<maximumiteration;i++)
            {
                var alpha = alphamax +(alphamax - alphamin) / maximumiteration*i;
                for(int j=0;j<locationsize;j++)
                {
                   globalbest =Generatenewglobal(i+j,globalbest).Clone() as double[];
                    C10 = 4 * C10 * (1 - C10);
                    if (C10<=alpha)
                    {
                        C30 = 4 * C30 * (1 - C30);
                        (var tempmirror, var tempcomponent) = GeneratenMirrorAndComponent(C30, component, globalbest);
                        mirror = tempmirror.Clone() as double[];
                        var newcomponent = tempcomponent.Clone() as double[];
                        var errorofnewcompo = objectfun(newcomponent);
                        var errorofoldcompo = objectfun(component);
                        if (errorofnewcompo < errorofoldcompo)
                        {
                            component = newcomponent.Clone() as double[];
                        }
                        if (errorofnewcompo < best)
                        {
                            best = errorofnewcompo;
                        }
                    }
                    if (C10> alpha)
                    {
                        C20 = 4 * C20 * (1 - C20);
                        var newcomponent = GenerateComponent(C20,component).Clone() as double[];
                        var errorofnewcompo = objectfun(newcomponent);
                        var errorofoldcompo = objectfun(component);
                        if (errorofnewcompo < errorofoldcompo)
                        {
                            component = newcomponent.Clone() as double[];
                        }
                        if (errorofnewcompo < best)
                        {
                            best = errorofnewcompo;
                        }
                    }
                }

                if (Math.Abs(oldbest - best) < tolerance && i > Math.Floor((double)maximumiteration /2))
                //if (Math.Abs(oldbest - best) < tolerance && i > 1000)
                {
                    break;
                }
                if (best < oldbest)
                {
                    oldbest = best;
                }

                Console.WriteLine("Error: " + Convert.ToString(best));
            }

            return component;
        }


        public double[] GenerateComponent(double r2,double[]oldcomponent)
        {
            var result = new double[oldcomponent.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = (upperbound[i] - lowerbound[i]) * r2 + lowerbound[i];
            }

            return result;
        }
        public (double[],double[]) GeneratenMirrorAndComponent(double r3,double[]oldcomponent, double[]newglobal)
        {
            var newmirror=new double[oldcomponent.Length];
            var newcomponent= new double[oldcomponent.Length];
            for(int i=0;i<oldcomponent.Length;i++)
            {
                newmirror[i]=r3*oldcomponent[i]+(1-r3)*newglobal[i];
                newcomponent[i]=2*newmirror[i]-oldcomponent[i];
            }
            newmirror = ContrainVector(newmirror);//.Clone() as double[];
            newcomponent = ContrainVector(newcomponent);
            return (newmirror,newcomponent);
        }
        public double[] Generatenewglobal(int seed, double[]oldglobal)
        {
            var result= new double[oldglobal.Length];
           for(int i=0;i<oldglobal.Length;i++)
           {
               var rn=Normal.Sample(new MersenneTwister(seed+10,true), 0.0, 1.0); 
               result[i]=rn*0.01*(upperbound[i]-lowerbound[i]);
           }
           return result;
            
        }
        public double[] GenerateComposition(double r2)
        {
            var result= new double[lowerbound.Length];
            for(int i=0;i<lowerbound.Length;i++)
            {
                result[i]=lowerbound[i]+(upperbound[i]-lowerbound[i])*r2;
            }
            return result;
        }
        public double[] ContrainVector(double[] x)
        {
           
            for (int i = 0; i < x.Length; i++)
            {
                if (x[i] < lowerbound[i])
                {
                    x[i] = lowerbound[i];
                }
                if (x[i] > upperbound[i])
                {
                    x[i] = upperbound[i];
                }
            }
            return x;
        }
       
    }
}
