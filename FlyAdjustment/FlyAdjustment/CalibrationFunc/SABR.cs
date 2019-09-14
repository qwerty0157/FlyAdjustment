using System;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.LinearAlgebra;
namespace FlyAdjustment.CalibrationFunc
{
    public class SABR
    {
        public SABR()
        {
        }

        public static double CalcVol(
            double forward,
            double strike,
            double tau,
            Vector<double> param)
        {
            var alpha = param[0];
            var nu = param[1];
            var rho = param[2];
            var beta = 1.0;

            return alpha
                / (Math.Pow(forward * strike, (1 - beta) / 2.0))
                * (1 + Math.Pow(1 - beta, 2.0) / 24)
                * Math.Pow(Math.Log(forward / strike), 2.0)
                + (Math.Pow(1 - beta, 4.0) / 1920)
                * Math.Pow(Math.Log(forward * strike), 4.0)
                * chiFunction(forward, strike, alpha, nu, rho, beta)
                      * Function(forward, strike, tau, alpha, nu, rho, beta);
        }

        private static double chiFunction(
            double forward,
            double strike,
            double alpha,
            double nu,
            double rho,
            double beta)
        {
            var zFunc = nu / alpha
                * Math.Pow(forward * strike, (1 - beta) / 2.0)
                      * Math.Log(forward / strike);
            var chi = Math.Log(
                (Math.Sqrt(1 - 2.0 * rho * zFunc + Math.Pow(zFunc, 2.0))
                + zFunc - rho) / (1 - rho));
            return chi;
        }

        private static double Function(
            double forward,
            double strike,
            double tau,
            double alpha,
            double nu,
            double rho,
            double beta)
        {
            return 1 + (Math.Pow((1 - beta), 2.0)) / 24 * Math.Pow(alpha, 2.0)
                         / Math.Pow(forward * strike, 1 - beta)
                + 1 / 4 * rho * beta * nu * alpha
                / Math.Pow((forward * strike), (1 - beta) / 2
                           + ((2 - 3 * Math.Pow(rho, 2.0)) / 24 * Math.Pow(nu, 2.0))) * tau;
        }
    }
}
