using MathNet.Numerics.Random;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalOptimizationLib
{
    public class WOAOptimization
    {
        //Implement Chaotic whale optimization algorithm developed by Gaganpreet Kaur, Sankalap Arora
        public double[] lowerbound { get; set; }
        public double[] upperbound { get; set; }
        public int maximumiteration { get; set; }
        public int numofagents { get; set; }
        public double tolerance { get; set; }
        public Func<double[], double> objectfun { get; set; }

        public double[] Optimize()
        {
            var Positions = new Dictionary<int, double[]>();
            var Leader_score = 999999999999999.99;
            var Leader_pos = new double[lowerbound.Length];
            //Initialize
            for (int i = 0; i < numofagents; i++)
            {
                var rnd = new MersenneTwister(i + 1, true);
                var temp = new double[lowerbound.Length];
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    temp[j] = rnd.NextDouble() * (upperbound[j] - lowerbound[j]) + lowerbound[j];
                }
                Positions.Add(i, temp.Clone() as double[]);
                if (objectfun(temp) < Leader_score)
                {
                    Leader_pos = temp.Clone() as double[];
                }
            }

            //Main loop
            for (int i = 0; i < maximumiteration; i++)
            { 
            
            
            }
        }

    }
}
