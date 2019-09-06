using System;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace FlyAdjustment
{
    public class CalcDelta
    {
        static public double CalcDeltaValue(
            //DeltaType deltaType,
            bool callFlag,
            double rateOfForeign,
            double rateOfDomestic,
            double forward,
            double strike,
            double vol,
            double tau
            )
        {
            // For EURUSD
            var d1 = CalcD.CalcDValue(forward, strike, vol, tau, true);
            return Convert.ToInt32(callFlag) * Math.Exp(-rateOfForeign * tau)
                          * Normal.CDF(0, 1, Convert.ToInt32(callFlag) * d1);    
        }

        enum DeltaType
        {
            PipsSpotDelta,
            PercentageSpotDelta,
            PipsForwardDelta,
            PercentageForwardDelta
        }
    }
}
