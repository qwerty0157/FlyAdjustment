using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
namespace FlyAdjustment
{
    public class PolynomialInDelta
    {
        static public double PolynomialInDeltaFunction(
            double strike,
            double forward,
            double tau,
            double c0,
            double c1,
            double c2)
        {
            var x = Math.Log(forward / strike);
            var delta0 = Math.Exp(c0);
            var delta = CalcDelta(x, delta0, tau);

            return Math.Exp(function(x, delta, c0, c1, c2));
        }

        static private double CalcDelta(
            double x,
            double delta0,
            double tau)
        {
            return Normal.CDF(0, 1, x / (delta0 * Math.Sqrt(tau)));
        }

        static private double function(
            double x,
            double delta,
            double c0,
            double c1,
            double c2)
        {
            return c0 + c1 * delta + c2 * Math.Pow(delta, 2.0);
        }
    }
}
