using System;

namespace SharpMath.Integration
{
	public sealed class GaussLaguerreQuadrature
	{
		private int n;
		private double[] x, w;

		public GaussLaguerreQuadrature(int n)
			: this(0.0, n)
		{
		}

		public GaussLaguerreQuadrature(double alpha, int n)
		{
			this.n = n;

			ComputeNodes(alpha, n, out x, out w);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty x^\alpha e^{-x}f(x)dx$ using n-point Gauss-Laguerre quadrature.
		/// </summary>
		public double Integrate(Func<double, double> f)
		{
			double s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += w[i] * f(x[i]);
			}

			return s;
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{-x}f(x)dx$ using n-point Gauss-Laguerre quadrature.
		/// </summary>
		public static double Integrate(Func<double, double> f, int n)
		{
			return Integrate(f, 0.0, n);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty x^\alpha e^{-x}f(x)dx$ using n-point Gauss-Laguerre quadrature.
		/// </summary>
		public static double Integrate(Func<double, double> f, double alpha, int n)
		{
			return new GaussLaguerreQuadrature(alpha, n).Integrate(f);
		}

		/// <summary>
		/// Computes the n-point Gauss-Laguerre quadrature nodes such that $\int_0^\infty x^\alpha e^{-x}f(x)dx\approxeq\sum_iw_if(x_i)$.
		/// </summary>
		public static void ComputeNodes(double alpha, int n, out double[] x, out double[] w)
		{
			if (n < 1)
			{
				throw new ArgumentException("Invalid number of nodes.");
			}

			if (alpha <= -1.0)
			{
				throw new ArgumentException("The alpha parameter must be greater than or equal to -1.");
			}

			// See also:
			// http://mathworld.wolfram.com/Gauss-LaguerreQuadrature.html
			// http://www.alglib.net/integral/gq/glaguerre.php

			x = new double[n];
			w = new double[n];

			double r = 0.0;

			for (int i = 0; i < n; i++)
			{
				if (i == 0)
				{
					r = (1.0 + alpha) * (3.0 + 0.92 * alpha) / (1.0 + 2.4 * n + 1.8 * alpha);
				}
				else if (i == 1)
				{
					r = r + (15.0 + 6.25 * alpha) / (1.0 + 0.9 * alpha + 2.5 * n);
				}
				else
				{
					r = r + ((1.0 + 2.55 * (i - 1.0)) / (1.9 * (i - 1.0)) + 1.26 * (i - 1.0) * alpha / (1.0 + 3.5 * (i - 1.0))) / (1.0 + 0.3 * alpha) * (r - x[i - 2]);
				}

				double r1, p2, dp3;
				do
				{
					p2 = 0;
					double p3 = 1;
					for (int j = 0; j < n; j++)
					{
						double p1 = p2;
						p2 = p3;
						p3 = ((-r + 2.0 * j + alpha + 1.0) * p2 - (j + alpha) * p1) / (j + 1.0);
					}
					dp3 = (n * p3 - (n + alpha) * p2) / r;
					r1 = r;
					r = r - p3 / dp3;
				} while (Math.Abs(r - r1) >= 5.0e-14 * (1.0 + Math.Abs(r)));

				x[i] = r;
				w[i] = -(Math.Exp(SpecialFunctions.LogGamma(alpha + n) - SpecialFunctions.LogGamma(n)) / (dp3 * n * p2));
			}
		}
	}
}
