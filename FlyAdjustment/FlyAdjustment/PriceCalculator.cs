using System;
using MathNet.Numerics.Distributions;
namespace FlyAdjustment
{
    public class PriceCalculator
    {
        private Func<double, double> CDF = d => Normal.CDF(0, 1, d);
        static public double CalcOptionPrice(
            InitialTerm.InitialParameter param,
            double forward,
            double strike,
            double d1,
            double d2,
            InitialTerm.CallPutType type)
        {
            return (int)type * forward * Math.Exp(-param.rateOfForeign * param.tau)
                        * Normal.CDF(0, 1, (int)type * d1)
                        - strike * Normal.CDF(0, 1, (int)type * d2);
        }
    }
}
