using System;
using System.Collections.Generic;
namespace FlyAdjustment
{
    public class UseBisection
    {
        public static double Calc(Func<double, double> func, double lower, double upper)
        {
            var i = 0;
            var middle = 0.0;
            var eps = 1.0E-8;

            while(!(Math.Abs(upper-lower)<eps))
            {
                i++;
                middle = (upper + lower) / 2.0;
                if(func(middle) * func(lower) > 0)
                {
                    lower = middle;
                }
                else
                {
                    upper = middle;
                }
            }
            return middle;
        }
    }
}
