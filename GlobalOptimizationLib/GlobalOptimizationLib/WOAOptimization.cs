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
        public int numofflies { get; set; }
        public double tolerance { get; set; }
        public Func<double[], double> objectfun { get; set; }
    }
}
