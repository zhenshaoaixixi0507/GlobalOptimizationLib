using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalOptimizationLib
{
    class Program
    {
        static void Main(string[] args)
        {
            var test = new TestFunctions();
            var lowerbound = new double[50];
            var upperbound = new double[50];
            for (int i = 0; i < lowerbound.Length; i++)
            {
                lowerbound[i] = -0.99;
                upperbound[i] = 0.99;
            }

            //Test SimulatedAnnealingOptimization
            var SA = new SimulatedAnnealingOptimization();
            SA.tolerance = 0.00000000000001;
            SA.objectfun = test.sumsqure;
            SA.lowerbound = lowerbound;
            SA.upperbound = upperbound;
            SA.numberofneighbours = 500;
            SA.maximumiteration = 500;
            SA.initialguesnum = 1000;
            var optimizedx = SA.Optimize();

            ////TestPSOOptimization
            //var PSO = new PSOOptimization();
            //PSO.tolerance = 0.000001;
            //PSO.objectfun = test.sumsqure;
            //PSO.lowerbound = lowerbound;
            //PSO.upperbound = upperbound;
            //PSO.maximumiteration = 500;
            //PSO.numofswarms = 500;
            //PSO.inertiaweightmax = 1.2;
            //PSO.inertiaweightmin = 0.1;
            //PSO.chi = 0.73;
            //PSO.c1 = 2;
            //PSO.c2 = 2;
            //PSO.Vmax = 4;
            //var optimizedx = PSO.Optimize();

            //TestFruitFlyOptimization
            //var FFO = new FruitFlyOptimization();
            //FFO.tolerance = 0.000001;
            //FFO.objectfun = test.sumsqure;
            //FFO.lowerbound = lowerbound;
            //FFO.upperbound = upperbound;
            //FFO.maximumiteration = 50000;
            //FFO.numofflies = 5;
            //var optimizedx = FFO.Optimize();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }
}
