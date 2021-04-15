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
        public int initialguesssize { get; set; }
        public double tolerance { get; set; }
        public Func<double[], double> objectfun { get; set; }

        public double[] Optimize()
        {
            var Positions = new Dictionary<int, double[]>();
            var Leader_score = 999999999999999.99;
            var Leader_pos = new double[lowerbound.Length];
            //Initialize
            for (int i = 0; i < initialguesssize; i++)
            {
                var rnd = new MersenneTwister(i + 1, true);
                var temp = new double[lowerbound.Length];
                for (int j = 0; j < lowerbound.Length; j++)
                {
                    temp[j] = rnd.NextDouble() * (upperbound[j] - lowerbound[j]) + lowerbound[j];
                }
                Positions.Add(i, temp.Clone() as double[]);
                var templeaderscore = objectfun(temp);
                if (templeaderscore < Leader_score)
                {
                    Leader_score = templeaderscore;
                    Leader_pos = temp.Clone() as double[];
                }
            }

            var oldLeader_score = Leader_score;
            var a = 0.0;
            var a2 = 0.0;
            //Main loop
            for (int i = 0; i < maximumiteration; i++)
            {
                a = 2 - i * ((2) / maximumiteration);
                a2 = -1 + i * ((-1) / maximumiteration);
                var rnd = new MersenneTwister(i + 1, true);
                var rnd2 = new MersenneTwister(i + 2, true);
                var rnd3 = new MersenneTwister(i + 3, true);
                var rnd4 = new MersenneTwister(i + 4, true);
                for (int j = 0; j < numofagents; j++)
                {
                    var r1 = rnd.NextDouble();
                    var r2 = rnd2.NextDouble();
                    var A = 2 * a * r1 - a;
                    var C = 2 * r2;
                    var b = 1;
                    var l = (a2 - 1) * rnd3.NextDouble() + 1;
                    var p = rnd4.NextDouble();
                    for (int k = 0; k < lowerbound.Length; k++)
                    {
                        if (p < 0.5)
                        {
                            if (Math.Abs(A) >= 1)
                            {
                                var rnd5 = new MersenneTwister(i + j + k + 1, true);
                                var rand_leader_index = (int)Math.Floor((numofagents-1)* rnd5.NextDouble() + 0);
                                var X_rand = Positions[rand_leader_index];
                                var D_X_rand = Math.Abs(C * X_rand[k] - Positions[j][k]);
                                Positions[j][k] = X_rand[k] - A * D_X_rand;
                            }
                            if (Math.Abs(A) < 1)
                            {
                                var D_Leader = Math.Abs(C * Leader_pos[k] - Positions[j][k]);
                                Positions[j][k] = Leader_pos[k] - A * D_Leader;
                            }

                        }
                        if (p >= 0.5)
                        {
                            var distance2Leader = Math.Abs(Leader_pos[k] - Positions[j][k]);
                            Positions[j][k] = distance2Leader * Math.Exp(b * l) * Math.Cos(l * 2 * Math.PI) + Leader_pos[k];
                        }
                    }
                    Positions[j] = ConstrainPara(Positions[j].Clone() as double[]);
                    var fitness = objectfun(Positions[j]);
                    if (fitness < Leader_score)
                    {
                        Leader_score = fitness;
                        Leader_pos = Positions[j].Clone() as double[];
                    }
                    if (Math.Abs(oldLeader_score - Leader_score) < tolerance && i >=100)
                    {
                        break;
                    }
                    else
                    {
                        oldLeader_score = Leader_score;
                        Console.WriteLine("Iteration Time: " + Convert.ToString(i) + " Objective Function Value: " + Convert.ToString(Leader_score));
                    }
                   
                }
            }

            return Leader_pos;
        }

        public double[] ConstrainPara(double[]X)
        {
            for (int i = 0; i < X.Length; i++)
            {
                if (X[i] < lowerbound[i])
                {
                    X[i] = lowerbound[i];
                }
                if (X[i] > upperbound[i])
                {
                    X[i] = upperbound[i];
                }
            }
            return X;
        }

    }
}
