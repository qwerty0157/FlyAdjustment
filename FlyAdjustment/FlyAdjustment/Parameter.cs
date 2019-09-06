using System;
namespace FlyAdjustment
{
    public class Parameter
    {
        double rateOfForeign;
        double rateOfDomestic;
        double spotRate;
        double tau;
        double volAtm;
        double vol25MS;
        double vol25RR;
        Parameter parameter;

        public Parameter(Parameter parameter)
        {
            this.parameter = parameter;
        }

        public Parameter(
            double rateOfForeign,
            double rateOfDomestic,
            double spotRate,
            double tau,
            double volAtm,
            double vol25MS,
            double vol25RR)
        {
            this.rateOfDomestic = rateOfDomestic;
            this.rateOfForeign = rateOfForeign;
            this.spotRate = spotRate;
            this.tau = tau;
            this.volAtm = volAtm;
            this.vol25MS = vol25MS;
            this.vol25RR = vol25RR;
        }

        Parameter GetParameter()
        {
            return this;
        }

        public double getRateOfForeign()
        {
            return this.rateOfForeign;
        }
        public double getRateOfDomestic()
        {
            return this.rateOfDomestic;
        }
        public double getSpotRate()
        {
            return this.spotRate;
        }
        public double getTau()
        {
            return this.tau;
        }
        public double getVolAtm()
        {
            return this.volAtm;
        }
        public double getVol25MS()
        {
            return this.vol25MS;
        }
        public double getVol25RR()
        {
            return this.vol25RR;
        }
    }
}
