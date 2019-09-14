using System;
namespace FlyAdjustment.JudgeMaterial
{
    public class JudgeOfCallPut
    {
        static public InitialTerm.CallPutType CallOrPut(InitialTerm.VolatilityType volatilityType)
        {
            var type = InitialTerm.CallPutType.put;
            if (volatilityType == InitialTerm.VolatilityType.MS10Put
                || volatilityType == InitialTerm.VolatilityType.MS25Put)
            {
                type = InitialTerm.CallPutType.put;
            }
            else
                type = InitialTerm.CallPutType.call;

            return type;
        }
    }
}
