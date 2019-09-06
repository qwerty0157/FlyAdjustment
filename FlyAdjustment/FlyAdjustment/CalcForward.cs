using System;
namespace FlyAdjustment
{
    public class CalcForward
    {
        static public double CalcForwardRate(
            double spotRate,
            double tau, 
            double rateOfForeign, 
            double rateOfDomestic)
        {
            return spotRate * Math.Exp((rateOfDomestic - rateOfForeign) * tau);
        }
    }
}
