using System;
namespace FlyAdjustment.JudgeMaterial
{
    public class JudgeOfVolatility
    {
        public static double Volatility(InitialTerm.InitialParameter param, InitialTerm.VolatilityType volatilityType)
        {
            double volatility = 0.0;
            if (volatilityType == InitialTerm.VolatilityType.ATM)
            {
                volatility = param.volATM;
            }
            else if (volatilityType == InitialTerm.VolatilityType.MS25Call
                     || volatilityType == InitialTerm.VolatilityType.MS25Put)
            {
                volatility = param.volATM + param.volMS25;
            }
            else if (volatilityType == InitialTerm.VolatilityType.MS10Call
                     || volatilityType == InitialTerm.VolatilityType.MS10Put)
            {
                volatility = param.volATM + param.volMS10;
            }
            return volatility;
        }
    }
}
