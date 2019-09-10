using System;
using System.Collections.Generic;
using System.Linq;

namespace FlyAdjustment
{
    static public class InitialSetting
    {
        public static List<double> StrikeSetting(InitialTerm.InitialParameter param)
        {
            var strikeList = new List<double>();
            var forward = ForwardCalculator.CalcForwardRate(param);
            foreach(InitialTerm.VolatilityType volatilityType
                   in Enum.GetValues(typeof(InitialTerm.VolatilityType)))
            {
                var volatility = JudgeMaterial.JudgeOfVolatility.Volatility(param, volatilityType);
                strikeList.Add(StrikrCalcultor.CalcStrike(
                    param,
                    forward,
                    volatility,
                    param.deltaType,
                    volatilityType));
            }
            return strikeList;
        }

        public static List<double> PriceSetting(InitialTerm.InitialParameter param)
        {
            var priceList = new List<double>();
            var forward = ForwardCalculator.CalcForwardRate(param);
            foreach(InitialTerm.VolatilityType volatilityType
                    in Enum.GetValues(typeof(InitialTerm.VolatilityType)))
            {
                var type = JudgeMaterial.JudgeOfCallPut.CallOrPut(volatilityType);
                var volatility = JudgeMaterial.JudgeOfVolatility.Volatility(param, volatilityType);
                var strike = StrikrCalcultor.CalcStrike(param, forward, volatility, param.deltaType, volatilityType);
                var d1 = DFuncCalculator.CalcDValue(param, volatility, param.tau, forward, strike);
                var d2 = d1 - volatility * Math.Sqrt(param.tau);
                priceList.Add(FlyAdjustment.PriceCalculator.CalcOptionPrice(param, forward, strike, d1, d2, type));                                 
            }
            return priceList;
        }
    }
}
