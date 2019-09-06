using System;
using MathNet.Numerics.Distributions;
namespace FlyAdjustment
{
    public class CalcPrice
    {
        static public double CalcOptionPrice(
            bool callFlag,
            double spot,
            double forward,
            double rateOfForeign,
            double rateOfDomestic,
            double strike,
            double vol,
            double tau)
        {
            var d1 = CalcD.CalcDValue(
                forward,
                strike,
                vol,
                tau,
                callFlag);
            
            var d2 = CalcD.CalcDValue(
                forward,
                strike,
                vol,
                tau,
                !callFlag);

            return Convert.ToInt32(callFlag) * spot * Math.Exp(-rateOfForeign * tau)
                          * Normal.CDF(0, 1, Convert.ToInt32(callFlag) * d1)
                          - Convert.ToInt32(callFlag) * strike * Math.Exp(-rateOfDomestic * tau)
                          * Normal.CDF(0, 1, Convert.ToInt32(callFlag) * d2);
        }
    }
}
