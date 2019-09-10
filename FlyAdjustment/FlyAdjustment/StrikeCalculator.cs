using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
using MathNet.Numerics;
namespace FlyAdjustment
{
    public class StrikrCalcultor
    {
        private static double Number;
        public InitialTerm.InitialParameter param;
        private static double forward;
        private static double volatility;
        private static InitialTerm.CallPutType type;
        private static InitialTerm.DeltaType deltaType;
        private static Func<double, double> InvCDF = d => Normal.InvCDF(0, 1, d);

        static public double CalcStrike(
            InitialTerm.InitialParameter param,
            double forward,
            double volatility,
            InitialTerm.DeltaType deltaType,
            InitialTerm.VolatilityType volatilityType)
        {
            switch(volatilityType)
            {
                case InitialTerm.VolatilityType.ATM:
                    return CalcAtmStrike(param, forward, volatility, deltaType);
                case InitialTerm.VolatilityType.MS25Call:
                    return CalcInvCdf(
                        param, volatility, forward, 0.25, type, deltaType);
                case InitialTerm.VolatilityType.MS25Put:
                    return CalcInvCdf(
                        param, volatility, forward, -0.25, type, deltaType);
                case InitialTerm.VolatilityType.MS10Call:
                    return CalcInvCdf(
                        param, volatility, forward, 0.1, type, deltaType);
                case InitialTerm.VolatilityType.MS10Put:
                    return CalcInvCdf(
                        param, volatility, forward, -0.1, type, deltaType);
                default:
                    return 0;
            }
        }

        private static double CalcAtmStrike(
            InitialTerm.InitialParameter param,
            double forward,
            double volatility,
            InitialTerm.DeltaType deltaType)
        {
            double strike = 0.0;
            if(deltaType == InitialTerm.DeltaType.PipsSpotDelta
               || deltaType == InitialTerm.DeltaType.PercentageForwardDelta)
            {
                strike = forward * Math.Exp(volatility * volatility * param.tau / 2.0);
            }
            else
            {
                strike = forward * Math.Exp(-volatility * volatility * param.tau / 2.0);
            }
            return strike;
        }

        public static double CalcInvCdf(
            InitialTerm.InitialParameter param,
            double volatility,
            double forward,
            double delta,
            InitialTerm.CallPutType type,
            InitialTerm.DeltaType deltaType)
        {
            param = param;
            volatility = volatility;
            forward = forward;
            deltaType = deltaType;
            Number = delta;

            return UseBisection.Calc(
                DeltaFunctionOfStrike,
                0.5 * param.spotRate,
                2.0 * param.spotRate);
        }

        private static double DeltaFunctionOfStrike(double strike)
        {
            return DeltaCalculator.CalcDeltaValue(
                param,
                forward,
                strike,
                volatility,
                type,
                deltaType) - (int)type * Number;
        }
    }
}
