using System;
namespace FlyAdjustment.JudgeMaterial
{
    public class JudgeOfVolatility
    {
        public static double Volatility(InitialTerm.InitialParameter param, InitialTerm.VolatilityType volatilityType)
        {
            double volatility = 0.0;
            if (volatility == InitialTerm.VolatilityType.ATM)
            {
                volatility = param.volATM;
            }
            else if (volatility == InitialTerm.VolatilityType.MS25Call
                   || volatility == InitialTerm.VolatilityType.MS25Put)
            {
                volatility = param.volATM + param.volMS25;
            }
            else if (volatility == InitialTerm.VolatilityType.MS10Call
                   || volatility == InitialTerm.VolatilityType.MS10Put)
            {
                volatility = param.volATM + param.volMS10;
            }
            return volatility;
        }
    }
}
