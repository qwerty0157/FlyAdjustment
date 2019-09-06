using System;

namespace FlyAdjustment
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            CalcEURUSD();
        }

        static void CalcEURUSD()
        {
            var rateOfForeign = 0.03446;
            var rateOfDomestic = 0.0294;
            var spotRate = 1.3465;
            var tau = 1.0;
            var volAtm = 0.1825;
            var vol25MS = 0.0095;
            var vol25RR = -0.006;
            var forward = CalcForward.CalcForwardRate(
                spotRate,
                tau,
                rateOfForeign,
                rateOfDomestic);
            var parameter = new Parameter(
                rateOfForeign,
                rateOfDomestic,
                spotRate,
                tau,
                volAtm,
                vol25MS,
                vol25RR);
            var delta = 0.25;

            Calibration.FlyAdjustmentByPolynomial(delta, parameter);
        }
    }
}
