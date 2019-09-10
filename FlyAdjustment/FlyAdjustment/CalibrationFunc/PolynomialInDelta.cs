using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.Distributions;
namespace FlyAdjustment.CalibrationFunc
{
    public class PolynomialInDelta
    {
        public PolynomialInDelta()
        {
        }
        static Func<double, double> CDF = d => Normal.CDF(0,1,d);
        public static double CalcVol(
            double forward,
            double strike,
            double tau,
            Vector<double> param)
        {
            double delta0 = Math.Exp(param[0]);
            return Math.Exp(PolynomialFunc(tau, delta0, Math.Log(forward / strike), param));
        }

        static double PolynomialFunc(
            double tau,
            double delta0,
            double x,
            Vector<double> param)
        {
            double polynomialFunc = 0.0;
            polynomialFunc = param[0] + param[1] * Math.Pow(deltaFunc(tau, delta0, x), 1.0)
                                      + param[2] * Math.Pow(deltaFunc(tau, delta0, x), 2.0);
            return polynomialFunc;
        }

        static double deltaFunc(
            double tau,
            double delta0,
            double x)
        {
            return CDF(x / (delta0 * Math.Sqrt(tau)));
        }
    }
}
