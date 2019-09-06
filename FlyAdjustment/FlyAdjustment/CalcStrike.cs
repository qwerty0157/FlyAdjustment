using System;
namespace FlyAdjustment
{
    public class CalcStrike
    {
        static public double CalcStrikeValue(
            //DeltaType deltaType,
            double forward,
            double vol,
            double tau)
        {
            return forward * Math.Exp(0.5 * Math.Pow(vol, 2.0) * tau);
        }
    }
}
