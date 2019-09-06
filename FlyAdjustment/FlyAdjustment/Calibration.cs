using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
namespace FlyAdjustment
{
    public class Calibration
    {
        static public double FlyAdjustmentByPolynomial(double delta, Parameter parameter)
        {
            var rateOfDomestic = parameter.getRateOfForeign();
            var rateOfForeign = parameter.getRateOfDomestic();
            var spotRate = parameter.getSpotRate();
            var tau = parameter.getTau();
            var volAtm = parameter.getVolAtm();
            var vol25MS = parameter.getVol25MS();
            var vol25RR = parameter.getVol25RR();

            var forward = CalcForward.CalcForwardRate(
                spotRate,
                tau,
                rateOfForeign,
                rateOfDomestic);

            var strikeAtm = CalcStrike.CalcStrikeValue(
                forward,
                volAtm,
                tau);

            //For EURUSD
            strikeAtm = 1.3620;
            var strike25MSCall = 1.5449;
            var strike25MSPut = 1.2050;

            var vTarget = CalcPrice.CalcOptionPrice
                                   (true,
                                   spotRate,
                                   forward,
                                   rateOfForeign,
                                   rateOfDomestic,
                                   strike25MSCall,
                                   volAtm + vol25MS,
                                   tau
                                   )
                        + CalcPrice.CalcOptionPrice
                                   (false,
                                   spotRate,
                                   forward,
                                   rateOfForeign,
                                   rateOfDomestic,
                                   strike25MSPut,
                                   volAtm + vol25MS,
                                   tau
                                   );
            var tol = 1.0E-7;
            var adjust = 1.0E-5;
            var vol25SS = vol25MS;

            Vector<double> vec = Vector<double>.Build.DenseOfArray(new double[3] { 0.0, 0.0, 0.0 });

            Func<double, double, double, double> PolynomialInDeltaAtm =
            (c0, c1, c2) =>
            {
                return PolynomialInDelta.PolynomialInDeltaFunction(
                    strikeAtm,
                    forward,
                    tau,
                    c0,
                    c1,
                    c2);
            };

            Func<double, double, double, double> PolynomialInDelta25CallMS =
            (c0, c1, c2) =>
            {
                return PolynomialInDelta.PolynomialInDeltaFunction(
                    strike25MSCall,
                    forward,
                    tau,
                    c0,
                    c1,
                    c2);
            };

            Func<double, double, double, double> PolynomialInDelta25PutMS =
            (c0, c1, c2) =>
            {
                return PolynomialInDelta.PolynomialInDeltaFunction(
                    strike25MSPut,
                    forward,
                    tau,
                    c0,
                    c1,
                    c2);
            };

            var res1 = Math.Pow(PolynomialInDeltaAtm(vec[0], vec[1], vec[2]) - volAtm, 2.0);
            var res2 = Math.Pow(PolynomialInDelta25CallMS(vec[0], vec[1], vec[2])
                                - PolynomialInDelta25PutMS(vec[0], vec[1], vec[2]) - vol25RR, 2.0);
            var res3 = Math.Pow(0.5 * PolynomialInDelta25CallMS(vec[0], vec[1], vec[2])
                                + 0.5 *  PolynomialInDelta25PutMS(vec[0], vec[1], vec[2])
                                - PolynomialInDeltaAtm(vec[0], vec[1], vec[2]) - vol25MS, 2.0);

            Vector<double> param = Vector<double>.Build.DenseOfArray(new double[3] { res1, res2, res3 });
            var test = FindMinimum.OfFunction((arg) => vec;);

            return 0;
        }
    }
}
