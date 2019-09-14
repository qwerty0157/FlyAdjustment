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
        public InitialTerm.InitialParameter Param;
        private static double Forward;
        private static double Volatility;
        private static InitialTerm.CallPutType Type;
        private static InitialTerm.DeltaType DeltaType;
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
                        param, volatility, forward, 0.25, Type, deltaType);
                case InitialTerm.VolatilityType.MS25Put:
                    return CalcInvCdf(
                        param, volatility, forward, -0.25, Type, deltaType);
                case InitialTerm.VolatilityType.MS10Call:
                    return CalcInvCdf(
                        param, volatility, forward, 0.1, Type, deltaType);
                case InitialTerm.VolatilityType.MS10Put:
                    return CalcInvCdf(
                        param, volatility, forward, -0.1, Type, deltaType);
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
            Volatility = volatility;
            Forward = forward;
            DeltaType = deltaType;
            Number = delta;

            var strikeCalculator = new StrikrCalcultor();
            return UseBisection.Calc(
                strikeCalculator.DeltaFunctionOfStrike,
                0.5 * param.spotRate,
                2.0 * param.spotRate);
        }

        private double DeltaFunctionOfStrike(double strike)
        {
            return DeltaCalculator.CalcDeltaValue(
                Param,
                Forward,
                strike,
                Volatility,
                Type,
                DeltaType) - (int)Type * Number;
        }
    }
}
