using DotNumerics.ODE;

namespace RIVM.radau5.Computations
{
    /// <summary>
    /// A wrapper around the DotNumerics ODE implicit Runge Kutta solver.
    /// </summary>
    internal class OdeSolver
    {
        protected const double FloatingPointZero = 1e-12;

        public static double[,] Solve(OdeFunction model, OdeJacobian jacobian, double x0, double xf, int numberOfEquations, double[] y0, double relTol, int numberOfTimeSteps)
        {
            double[,] sol;
            //Fix: reduce the timesteps slightly to avoid rouding errors that cause the ODE integrator to skip the last time step.
            double dx = (xf - x0) / (numberOfTimeSteps + FloatingPointZero);

            // OdeImplicitRungeKutta5 class is used to solve the diffusion-emission equations:
            OdeImplicitRungeKutta5 rungeKutta = new OdeImplicitRungeKutta5(model, jacobian, numberOfEquations);

            rungeKutta.ErrorToleranceType = ErrorToleranceEnum.Scalar;
            rungeKutta.RelTol = relTol;

            sol = rungeKutta.Solve(y0, x0, dx, xf);

            return sol;
        }
    }
}