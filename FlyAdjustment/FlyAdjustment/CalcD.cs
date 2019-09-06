using System;
namespace FlyAdjustment
{
    public class CalcD
    {
        static public double CalcDValue(
            double forward,
            double strike,
            double vol,
            double tau,
            bool callFlag)
        {
            return (Math.Log(forward / strike)
                     + Convert.ToInt32(callFlag) * 0.5 * Math.Pow(vol, 2.0) * tau)
                     / (vol * Math.Sqrt(tau));
        }
    }
}
