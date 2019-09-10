using System;
namespace FlyAdjustment
{
    public class ForwardCalculator
    {
        static public double CalcForwardRate(InitialTerm.InitialParameter param)
        {
            return param.spotRate * Math.Exp((param.rateOfDomestic - param.rateOfForeign) * param.tau);
        }
    }
}
