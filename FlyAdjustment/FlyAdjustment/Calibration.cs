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
        private static InitialTerm.InitialParameter param;
        private static double forward;
        public static double K25Call { get; private set; }
        public static double KATM;
        public static double K25Put { get; private set; }
        public static double VolSS25;
        public static string InterpolationName;
        private static Vector<double> VolParameter;
        private static List<double> strikeList;

        public static Vector<double> Solve(
            string interpolationName,
            InitialTerm.InitialParameter param,
            double forward, 
            double vtarget,
            List<double> strikeList)
        {
            InterpolationName = interpolationName;
            param = param;
            strikeList = strikeList;
            K25Call = strikeList[3];
            KATM = strikeList[2];
            K25Put = strikeList[1];
            VolSS25 = param.volMS25;
            var tol = 1.0E-5;
            var error = 1.0;
            var LowerVector = Vector<double>.Build.DenseOfArray(new double[] { 0.00001, 0.00001, -0.99999 });
            var UpperVector = Vector<double>.Build.DenseOfArray(new double[] { 10, 10, 0.99999 });
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
            var res1 = Math.Pow((VolCalculator(KATM, volParameter)) - param.volATM, 2.0);
            var res2 = Math.Pow((VolCalculator(K25Call, volParameter)- VolCalculator(K25Put, volParameter)) - param.volRR25, 2.0);
            var res3 = Math.Pow((0.5 * VolCalculator(K25Call, volParameter) + 0.5 * VolCalculator(K25Put, volParameter)
                                 - VolCalculator(KATM, volParameter)) - param.volMS25, 2.0);
            return res1 + res2 + res3;
        }

        private static double VolCalculator(double strike, Vector<double> vec)
        {
            switch(InterpolationName)
            {
                case "PolynomialInDelta":
                    return CalibrationFunc.PolynomialInDelta.CalcVol(forward, strike, param.tau, param);
                case "SABR":
                    return CalibrationFunc.SABR.CalcVol(forward, strike, param.tau, param);
                default:
                    return 0;
            }
        }
        private static double CalcCallStrike(double strike)
        {
            return DeltaCalculator.CalcDeltaValue(
                param,
                forward,
                strike,
                VolCalculator(strike, VolParameter),
                InitialTerm.CallPutType.call,
                param.deltaType) - 0.25;
        }
        private static double CalcPutStrike(double strike)
        {
            return DeltaCalculator.CalcDeltaValue(
                param,
                forward,
                strike,
                VolCalculator(strike, VolParameter),
                InitialTerm.CallPutType.put,
                param.deltaType) + 0.25;
        }
        private static double CalcValueTrial(Vector<double> vec)
        {
            var K25CallMS = strikeList[3];
            var K25PutMS = strikeList[1];
            var parameter = Vector<double>.Build.DenseOfArray(new double[] { vec[0], vec[1], vec[2]});

            var vol25Call = VolCalculator(K25CallMS, VolParameter);
            var vol25Put = VolCalculator(K25PutMS, VolParameter);

            var d1Call = DFuncCalculator.CalcDValue(
                param,
                vol25Call,
                param.tau,
                forward,
                K25CallMS);
            var d2Call = d1Call - vol25Call * Math.Sqrt(param.tau);
            var d1Put = DFuncCalculator.CalcDValue(
                param,
                vol25Put,
                param.tau,
                forward,
                K25PutMS);
            var d2Put = d1Put - vol25Put * Math.Sqrt(param.tau);

            return PriceCalculator.CalcOptionPrice(
                param,
                forward,
                K25CallMS,
                d1Call,
                d2Call,
                InitialTerm.CallPutType.call)
               + PriceCalculator.CalcOptionPrice(
                param,
                forward,
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
