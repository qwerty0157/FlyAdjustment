using System;
namespace FlyAdjustment.InitialTerm
{
    public class InitialParameter
    {
        public double spotRate { get; private set; }
        public double rateOfDomestic { get; private set; }
        public double rateOfForeign { get; private set; }
        public double tau { get; private set; }
        public double volATM { get; private set; }
        public double volMS25 { get; private set; }
        public double volMS10 { get; private set; }
        public double volRR25 { get; private set; }
        public double volRR10 { get; private set; }
        public DeltaType deltaType { get; private set; }

        public InitialParameter(
            double spotRate,
            double rateOfDomestic,
            double rateOfForeign,
            double tau,
            double volATM,
            double volMS25,
            double volMS10,
            double volRR25,
            double volRR10,
            DeltaType deltaType)
        {
            this.spotRate = spotRate;
            this.rateOfDomestic = rateOfDomestic;
            this.rateOfForeign = rateOfForeign;
            this.tau = tau;
            this.volATM = volATM;
            this.volMS25 = volMS25;
            this.volMS10 = volMS10;
            this.volRR25 = volRR25;
            this.volRR10 = volRR10;
            this.deltaType = deltaType;
        }
    }
        public enum CallPutType
        {
            call = 1,
            put = -1
        }

        public enum DeltaType
        {
            PipsSpotDelta,
            PercentageSpotDelta,
            PipsForwardDelta,
            PercentageForwardDelta
        }
        public enum VolatilityType
        {
            MS10Put,
            MS25Put,
            ATM,
            MS25Call,
            MS10Call
        }
    }
