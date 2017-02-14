using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.IO;

namespace RIVM.radau5.DotNumericsTests
{
    [TestClass]
    public class DotNumericsTests
    {
        private const int NumberOfTimeSteps = 100;

        [TestInitialize]
        public void SetupTrace()
        {
            Trace.Listeners.Add(new TextWriterTraceListener(Path.Combine(Path.GetTempPath(), @"DotNumericsTests.txt")));
        }

        [TestCleanup]
        public void CloseTrace()
        {
            Trace.Flush();
        }

        [TestMethod]
        public void CompareNumericToAnalyticLowTol()
        {
            double C0 = 1;
            double T = 20;
            double tf = 60;
            double k02 = 0.005;
            double k12 = 0.1;
            double relTol = 0.01;

            CompareNumericToAnalytic(C0, T, tf, k02, k12, relTol);
        }

        [TestMethod]
        public void CompareNumericToAnalyticTinyTol()
        {
            double C0 = 1;
            double T = 20;
            double tf = 60;
            double k02 = 0.005;
            double k12 = 0.1;
            double relTol = 0.001;

            CompareNumericToAnalytic(C0, T, tf, k02, k12, relTol);
        }

        /// <summary>
        /// Compares the numeric soltuion to the to analytic solution.
        /// </summary>
        /// <param name="C0">The concentration at t = 0 in compartment I.</param>
        /// <param name="T">The end time of emission.</param>
        /// <param name="tf">The end time of the simulation.</param>
        /// <param name="k02">The constant for the rate of ventilation from compartment II.</param>
        /// <param name="k12">The constant for the rate of emission form compartment I to compartment II.</param>
        /// <param name="relTol">The relative tol.</param>
        private static void CompareNumericToAnalytic(double C0, double T, double tf, double k02, double k12, double relTol)
        {
            var analyticModel = new AnalyticModel(C0, T, k02, k12);
            var numericModel = new NumericModel();

            Trace.WriteLine(String.Format("double C0 = {0};", C0));
            Trace.WriteLine(String.Format("double T = {0};", T));
            Trace.WriteLine(String.Format("double tf = {0};", tf));
            Trace.WriteLine(String.Format("double k02 = {0};", k02));
            Trace.WriteLine(String.Format("double k12 = {0};", k12));
            Trace.WriteLine(String.Format("double relTol = {0};", relTol));

            Trace.WriteLine("");

            Trace.WriteLine("Eval type\tt\tCair\tCproduct\tmodel/jacobian values");

            double[,] timeSeriesNumeric = numericModel.Solve(C0, T, tf, k02, k12, NumberOfTimeSteps, relTol);

            Trace.WriteLine("");

            Trace.WriteLine("t\tair analytic\tair numeric\tproduct analytic\tproduct numeric");

            for (int step = 0; step <= NumberOfTimeSteps; step++)
            {
                double t = tf * (double)step / (double)NumberOfTimeSteps;
                double analyticAirConcentration = analyticModel.AirConcentration(t);
                double analyticProductConcentration = analyticModel.ProductConcentration(t);
                double numericAirConcentration = timeSeriesNumeric[step, 1];
                double numericProductConcentration = timeSeriesNumeric[step, 2];

                Trace.WriteLine(string.Format("{0}\t{1}\t{2}\t{3}\t{4}", t, analyticAirConcentration, numericAirConcentration, analyticProductConcentration, numericProductConcentration));
            }
        }
    }
}