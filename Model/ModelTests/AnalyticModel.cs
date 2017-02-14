using System;

namespace RIVM.radau5.DotNumericsTests
{
    internal class AnalyticModel

    {
        private double C0, T, k02, k12, sqD, l, lp, lm, C0T, C1T, mu, nu;

        public AnalyticModel(double C0, double T, double k02, double k12)
        {
            this.C0 = C0;
            this.T = T;
            this.k02 = k02;
            this.k12 = k12;

            sqD = Math.Sqrt(4 * k12 * k12 + k02 * k02);

            l = -k12 - k02 / 2;

            lp = l + sqD / 2;
            lm = l - sqD / 2;

            C0T = AirConcentration(T);
            C1T = ProductConcentration(T);
        }

        public double AirConcentration(double t)
        {
            if (t <= T)
                return (C0 / sqD) * k12 * (Math.Exp(lp * t) - Math.Exp(lm * t));
            else
                return C0T * Math.Exp(-k02 * (t - T));
        }

        public double ProductConcentration(double t)
        {
            double C1;

            if (t <= T)
                C1 = C0 * Math.Exp(lm * t) * (k02 * (Math.Exp(sqD * t) - 1) + (Math.Exp(sqD * t) + 1) * sqD) / (2 * sqD);
            else
                C1 = ProductConcentration(T);

            return C1;
        }
    }
}