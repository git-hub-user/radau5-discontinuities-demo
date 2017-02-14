using DotNumerics.ODE;
using RIVM.radau5.Computations;
using System;
using System.Diagnostics;

namespace RIVM.radau5.DotNumericsTests
{
    internal class NumericModel
    {
        private int numberOfTimeSteps;
        private const int NumberOfCompartments = 2;

        private double T, k02, k12;

        /// <summary>
        /// Solves the set of differential equation from time = 0, start of emission, to time max.
        /// </summary>
        /// <param name="C0">Intial product concentration</param>
        /// <param name="T">The time at which the product is removed.</param>
        /// <param name="tf">The end time of the simulation.</param>
        /// <param name="k02">The K02.</param>
        /// <param name="k12">The K12.</param>
        /// <param name="relTol">The relative tolerance of the solution error.</param>
        /// <remarks>
        /// OdeImplicitRungeKutta5 class is used to solve the diffusion-emission equations.
        /// </remarks>
        public double[,] Solve(double C0, double T, double tf, double k02, double k12, int numberOfTimeSteps, double relTol)
        {
            this.T = T;
            this.k02 = k02;
            this.k12 = k12;
            this.numberOfTimeSteps = numberOfTimeSteps;
            double[] y0 = new double[NumberOfCompartments];

            // initial values
            y0[0] = 0.0; // concentration in the air.
            y0[1] = C0;  // concentration in the product.

            double t0 = 0;

            //using DotNumerics.ODE
            OdeFunction YDot = new OdeFunction(EmissionModel);
            OdeJacobian Jac = new OdeJacobian(JacEmissionModel);

            return OdeSolver.Solve(YDot, Jac, t0, tf, NumberOfCompartments, y0, relTol, numberOfTimeSteps);
        }

        /// <summary>
        /// The set of differential equations describing the emission model.
        /// </summary>
        /// <param name="t">The time.</param>
        /// <param name="y">The vector of concentrations in air, all product layers and the mean air concentration.</param>
        private double[] EmissionModel(double t, double[] y)
        {
            double[] dydt = new double[NumberOfCompartments];

            // model equations
            if (t <= T)
            {
                dydt[0] = k12 * (y[1] - y[0]) - k02 * y[0]; // air concentration
                dydt[1] = -k12 * (y[1] - y[0]);             // concentration in product
            }
            else
            {
                dydt[0] = -k02 * y[0];                      // air concentration
                dydt[1] = 0;                                // concentration in product
            }

            Trace.WriteLine(String.Format("Model\t{0}\t{1}\t{2}\t{3}\t{4}", t, y[0], y[1], dydt[0], dydt[1]));

            return dydt;
        }

        /// <summary>
        /// The jacobian of the emission model. Needed for implicit integration.
        /// </summary>
        /// <param name="t">The time.</param>
        /// <param name="y">The vector of concentrations in air, all product layers and the mean air concentration.</param>
        /// <returns></returns>
        public double[,] JacEmissionModel(double t, double[] y)
        {
            double[,] jacobian = new double[NumberOfCompartments, NumberOfCompartments];

            // Jacobian is a sparse matrix, initialize by setting all elements to zero
            Array.Clear(jacobian, 0, jacobian.Length);

            // model equations
            if (t <= T)
            {
                jacobian[0, 0] = -k12 - k02;
                jacobian[0, 1] = k12;
                jacobian[1, 0] = k12;
                jacobian[1, 1] = -k12;
            }
            else
            {
                jacobian[0, 0] = -k02;
                jacobian[0, 1] = 0;
                jacobian[1, 0] = 0;
                jacobian[1, 1] = 0;
            }

            Trace.WriteLine(String.Format("Jacobian\t{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", t, y[0], y[1], jacobian[0, 0], jacobian[0, 1], jacobian[1, 0], jacobian[1, 1]));

            return jacobian;
        }
    }
}