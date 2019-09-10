using System;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;

namespace FlyAdjustment
{
    public class DeltaCalculator
    {
        private static Func<double, double> CDF = d => Normal.CDF(0, 1, d);

        static public double CalcDeltaValue(
            InitialTerm.InitialParameter param,
            double forward,
            double strike,
            double volatility,
            InitialTerm.CallPutType type,
            InitialTerm.DeltaType deltaType)
        {
            var d1 = DFuncCalculator.CalcDValue(param, volatility, param.tau, forward, strike);
            var d2 = d1 - volatility * Math.Sqrt(param.tau);


            switch (deltaType)
            {
                case InitialTerm.DeltaType.PipsSpotDelta:
                    return (int)type
                        * Math.Exp(-param.rateOfForeign * param.tau) * CDF((int)type * d1);
                case InitialTerm.DeltaType.PercentageSpotDelta:
                    return (int)type
                        * Math.Exp(-param.rateOfDomestic * param.tau) * strike
                        / param.spotRate * CDF((int)type * d2);
                default:
                    return 0.0;
            }
        }
    }
}
