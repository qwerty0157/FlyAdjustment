using System;

namespace FlyAdjustment
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CalcUSDJPY();
        }

        static void CalcUSDJPY()
        {
            var spot = 106.925;
            var rateOfDomestic = -0.00085034363976;
            var rateOfForeign = 0.02216698493974;
            var tau = 1.0;
            var volAtm = 0.071;
            var vol25MS = 0.002;
            var vol10MS = 0.0105;
            var vol25RR = -0.021;
            var vol10RR = -0.042;
            var deltaType = InitialTerm.DeltaType.PercentageSpotDelta;

            var param = new InitialTerm.InitialParameter(
                spot,
                rateOfDomestic,
                rateOfForeign,
                tau,
                volAtm,
                vol25MS,
                vol10MS,
                vol25RR,
                vol10RR,
                deltaType
            );
            var forward = ForwardCalculator.CalcForwardRate(param);
            var strikeList = InitialSetting.StrikeSetting(param);
            var interpolationName = "PolynomialInDelta";
            //var interpolationName = "SABR";

            var vTarget = 4.5;

            var Ans = Calibration.Solve(
                interpolationName,
                param,
                forward,
                vTarget,
                strikeList);

            Console.WriteLine(Ans);
        }
    }
}
