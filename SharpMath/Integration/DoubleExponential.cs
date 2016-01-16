using System;

namespace SharpMath.Integration
{
	public sealed class DoubleExponential //DoubleExponentialQuadrature
	{
		private int n;
		private double h;
		private double[] x, w;

		public DoubleExponential(int n, double h)
		{
			this.n = n;
			this.h = h;

			x = new double[n + 1];
			w = new double[n + 1];

			for (int i = 0; i <= n; i++)
			{
				double t = n * i;
				x[i] = Math.Tanh(0.5 * Math.PI * Math.Sinh(t));
				w[i] = 0.5 * Math.PI * (1.0 - (x[i] * x[i])) * Math.Cosh(t);
			}
		}

		public DoubleExponential(double eps, double h)
			//: this((int)Math.Ceiling(Math.Log(Math.Log((1.0 + eps) / (1.0 - eps)) / Math.PI + Math.Sqrt(1.0 + Math.Log(2.0 * (1.0 + eps) / (1.0 - eps)) / (Math.PI * Math.PI))) / h), h)
			: this(ComputeSteps(eps, h), h)
		{
		}

		private static int ComputeSteps(double eps, double h)
		{
			// Determine the smallest integer satisfying tanh(0.5*pi*sinh(h*n))=eps.
			double x = (Math.Log(1.0 + eps) - Math.Log(1.0 - eps)) / Math.PI;
			return (int)Math.Ceiling(Math.Log(x + Math.Sqrt(1.0 + x * x)) / h);
		}

		/// <summary>
		/// Computes the integral $\int_{-1}^1f(x)dx$.
		/// </summary>
		public double Integrate(Func<double, double> f)
		{
			double s = w[0] * f(x[0]);
			for (int i = 1; i <= n; i++)
			{
				s += w[i] * (f(x[i]) + f(-x[i]));
			}
			return h * s;
		}

		/// <summary>
		/// Computes the integral $\int_a^bf(x)dx$.
		/// </summary>
		public double Integrate(Func<double, double> f, double a, double b)
		{
			double c = 0.5 * (b - a);
			double d = 0.5 * (a + b);
			return c * Integrate(x => f(c * x + d));
		}

		public static double Integrate(Func<double, double> f, int n, double h)
		{
			return new DoubleExponential(n, h).Integrate(f);
		}

		// ... tre andre muligheder ...
	}
}
