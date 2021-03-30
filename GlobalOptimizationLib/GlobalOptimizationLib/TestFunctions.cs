using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalOptimizationLib
{
    public class TestFunctions
    {
        public double sumsqure(double[]x)
        {
            var result = 0.0;
            for (int i = 0;i < x.Length;i++)
            {
                result = x[i] * x[i] + result-5;
            }

            return result;
        }
    }
}
