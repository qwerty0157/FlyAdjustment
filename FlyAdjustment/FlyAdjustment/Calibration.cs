using System;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Optimization;
using MathNet.Numerics.RootFinding;
namespace FlyAdjustment
{
    public class Calibration
    {
        private static Func<double, double> CDF = d => Normal.CDF(0, 1, d);
        private static InitialTerm.InitialParameter Param;
        private static double Forward;
        public static double K25Call { get; private set; }
        public static double KATM;
        public static double K25Put { get; private set; }
        public static double VolSS25;
        public static string InterpolationName;
        private static Vector<double> VolParameter;
        private static List<double> StrikeList;

        public static Vector<double> Solve(
            string interpolationName,
            InitialTerm.InitialParameter param,
            double forward, 
            double vtarget,
            List<double> strikeList)
        {
            InterpolationName = interpolationName;
            Param = param;
            StrikeList = strikeList;
            K25Call = strikeList[3];
            KATM = strikeList[2];
            K25Put = strikeList[1];
            VolSS25 = param.volMS25;
            var tol = 1.0E-5;
            var error = 1.0;
            var LowerVector = Vector<double>.Build.DenseOfArray(new double[] { 0.00001, 0.00001, -0.99999 });
            var UpperVector = Vector<double>.Build.DenseOfArray(new double[] { 100, 100, 0.99999 });
            VolParameter = Vector<double>.Build.DenseOfArray(new double[] { 0.1, 0.1, -0.1 });

            while(Math.Abs(error) > tol)
            {
                if(InterpolationName == "SABR")
                {
                    VolParameter = FindMinimum.OfFunctionConstrained(
                        FuncOfVolCondtion,
                        LowerVector,
                        UpperVector,
                        VolParameter);
                    //VolParameter = FindMinimum.OfFunction(FuncOfVolCondtion, VolParameter);
                }
                else
                {
                    VolParameter = FindMinimum.OfFunction(FuncOfVolCondtion, VolParameter);
                }
                CalcStrikeList(VolParameter);
                var vTrial = CalcValueTrial(VolParameter);
                error = vTrial - vtarget;

                if(error > 0)
                {
                    VolSS25 -= tol / 100;
                }
                else
                {
                    VolSS25 += tol / 100;
                }
            }
            var volRR25 = VolCalculator(K25Call, VolParameter) - VolCalculator(K25Put, VolParameter);
            var Ans = Vector<double>.Build.DenseOfArray(new double[]
            {
                VolParameter[0],
                VolParameter[1],
                VolParameter[2],
                K25Call,
                K25Put,
                VolSS25,
                volRR25
            });
            return Ans;
        }

        private static double FuncOfVolCondtion(Vector<double> vec)
        {
            var volParameter = Vector<double>.Build.DenseOfArray(new double[] { vec[0], vec[2], vec[2] });
            var res1 = Math.Pow((VolCalculator(KATM, volParameter)) - Param.volATM, 2.0);
            var res2 = Math.Pow((VolCalculator(K25Call, volParameter)- VolCalculator(K25Put, volParameter)) - Param.volRR25, 2.0);
            var res3 = Math.Pow((0.5 * VolCalculator(K25Call, volParameter) + 0.5 * VolCalculator(K25Put, volParameter)
                                 - VolCalculator(KATM, volParameter)) - Param.volMS25, 2.0);
            return res1 + res2 + res3;
        }

        private static double VolCalculator(double strike, Vector<double> vec)
        {
            switch(InterpolationName)
            {
                case "PolynomialInDelta":
                    return CalibrationFunc.PolynomialInDelta.CalcVol(Forward, strike, Param.tau, vec);
                case "SABR":
                    return CalibrationFunc.SABR.CalcVol(Forward, strike, Param.tau, vec);
                default:
                    return 0;
            }
        }
        private static double CalcCallStrike(double strike)
        {
            return DeltaCalculator.CalcDeltaValue(
                Param,
                Forward,
                strike,
                VolCalculator(strike, VolParameter),
                InitialTerm.CallPutType.call,
                Param.deltaType) - 0.25;
        }
        private static double CalcPutStrike(double strike)
        {
            return DeltaCalculator.CalcDeltaValue(
                Param,
                Forward,
                strike,
                VolCalculator(strike, VolParameter),
                InitialTerm.CallPutType.put,
                Param.deltaType) + 0.25;
        }
        private static double CalcValueTrial(Vector<double> vec)
        {
            var K25CallMS = StrikeList[3];
            var K25PutMS = StrikeList[1];
            var parameter = Vector<double>.Build.DenseOfArray(new double[] { vec[0], vec[1], vec[2]});

            var vol25Call = VolCalculator(K25CallMS, VolParameter);
            var vol25Put = VolCalculator(K25PutMS, VolParameter);

            var d1Call = DFuncCalculator.CalcDValue(
                Param,
                vol25Call,
                Param.tau,
                Forward,
                K25CallMS);
            var d2Call = d1Call - vol25Call * Math.Sqrt(Param.tau);
            var d1Put = DFuncCalculator.CalcDValue(
                Param,
                vol25Put,
                Param.tau,
                Forward,
                K25PutMS);
            var d2Put = d1Put - vol25Put * Math.Sqrt(Param.tau);

            return PriceCalculator.CalcOptionPrice(
                Param,
                Forward,
                K25CallMS,
                d1Call,
                d2Call,
                InitialTerm.CallPutType.call)
               + PriceCalculator.CalcOptionPrice(
                Param,
                Forward,
                K25PutMS,
                d1Put,
                d2Put,
                InitialTerm.CallPutType.put);
        }

        private static void CalcStrikeList(Vector<double> volParameter)
        {
            K25Call = UseBisection.Calc(CalcCallStrike, KATM, 2.0 * KATM);
            K25Put = UseBisection.Calc(CalcPutStrike, 0.5 * KATM, KATM);
        }

    }
}
