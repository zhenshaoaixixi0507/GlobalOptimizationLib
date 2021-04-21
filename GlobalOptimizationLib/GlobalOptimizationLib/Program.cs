using System;
using System.Collections.Generic;
using System.Diagnostics;
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
                lowerbound[i] = -29.99;
                upperbound[i] = 29.99;
            }

            //TestPSOOptimization
            //var PSO = new PSOOptimization();
            //PSO.tolerance = 0.00000000001;
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
            //var stopwatch = new Stopwatch();
            //stopwatch.Reset();
            //stopwatch.Start();
            //var optimizedx = PSO.Optimize();
            //stopwatch.Stop();
            //Console.WriteLine($"\nTime taken {stopwatch.ElapsedMilliseconds}ms");

            //TestFruitFlyOptimization
            //var FFO = new FruitFlyOptimization();
            //FFO.tolerance = 0.000000001;
            //FFO.objectfun = test.sumsqure;
            //FFO.lowerbound = lowerbound;
            //FFO.upperbound = upperbound;
            //FFO.maximumiteration = 20000;
            //FFO.numofflies =10;
            //var optimizedx = FFO.Optimize();

            //TestInteriorSearchOptimization
            //var ISO = new InteriorSearchOptimization();
            //ISO.sizeofinitialguess = 1000;
            //ISO.tolerance = 0.000000001;
            //ISO.objectfun = test.sumsqure;
            //ISO.lowerbound = lowerbound;
            //ISO.upperbound = upperbound;
            //ISO.maximumiteration = 10000;
            //ISO.locationsize = 10;
            //ISO.alphamin = 0.1;
            //ISO.alphamax = 0.3;
            //var optimizedx = ISO.Optimize();

            ////TestWOAOptimization
            //var WOA = new WOAOptimization();
            //WOA.lowerbound = lowerbound;
            //WOA.upperbound = upperbound;
            //WOA.tolerance = 0.00000001;
            //WOA.initialguesssize = 1000;
            //WOA.numofagents =50;
            //WOA.maximumiteration = 500;
            //WOA.objectfun = test.sumsqure;
            //var optimizedx = WOA.Optimize();

            ////TestChaoticPSOOptimization
            //var ChaoticPSO = new ChaoticPSOOptimization();
            //ChaoticPSO.tolerance = 0.00000000001;
            //ChaoticPSO.objectfun = test.sumsqure;
            //ChaoticPSO.lowerbound = lowerbound;
            //ChaoticPSO.upperbound = upperbound;
            //ChaoticPSO.inertiaweightmax = 2;
            //ChaoticPSO.inertiaweightmin = 0.1;
            //ChaoticPSO.c1 = 2;
            //ChaoticPSO.c2 = 2;
            //var stopwatch = new Stopwatch();
            //stopwatch.Reset();
            //stopwatch.Start();
            //var optimizedx = ChaoticPSO.Optimize();
            //stopwatch.Stop();
            //Console.WriteLine($"\nTime taken {stopwatch.ElapsedMilliseconds}ms");

            ////TestChaoticInteriorSearchOptimization
            //var ChaoticISO = new InteriorSearchOptimization();
            //ChaoticISO.sizeofinitialguess = 1000;
            //ChaoticISO.tolerance = 0.000000001;
            //ChaoticISO.objectfun = test.sumsqure;
            //ChaoticISO.lowerbound = lowerbound;
            //ChaoticISO.upperbound = upperbound;
            //ChaoticISO.maximumiteration = 500;
            //ChaoticISO.locationsize = 10;
            //ChaoticISO.alphamin = 0.1;
            //ChaoticISO.alphamax = 0.3;
            //var stopwatch = new Stopwatch();
            //stopwatch.Reset();
            //stopwatch.Start();
            //var optimizedx = ChaoticISO.Optimize();
            //stopwatch.Stop();

            //TestChaoticFruitFlyOptimization
            var ChaoticFFO = new ChaoticFruitFlyOptimization();
            ChaoticFFO.tolerance = 0.000000001;
            ChaoticFFO.objectfun = test.sumsqure;
            ChaoticFFO.lowerbound = lowerbound;
            ChaoticFFO.upperbound = upperbound;
            ChaoticFFO.maximumiteration = 5000;
            ChaoticFFO.sizeofinitialguess =1000;
            ChaoticFFO.numofflies = 1000;
            var stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            var optimizedx = ChaoticFFO.Optimize();
            stopwatch.Stop();
            Console.WriteLine($"\nTime taken {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }
    }
}
