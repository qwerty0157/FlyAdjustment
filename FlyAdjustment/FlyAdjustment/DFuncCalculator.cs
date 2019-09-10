using System;
namespace FlyAdjustment
{
    public class DFuncCalculator
    {
        static public double CalcDValue(
            InitialTerm.InitialParameter param,
            double vol,
            double tau,
            double forward,
            double strike)
        {
            return (Math.Log(forward / strike)
                     * 0.5 * Math.Pow(vol, 2.0) * tau)
                     / (vol * Math.Sqrt(tau));
        }
    }
}
