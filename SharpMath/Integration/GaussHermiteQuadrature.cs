// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Integration
{
	public sealed class GaussHermiteQuadrature
	{
		private int n;
		private double[] x, w;

		public GaussHermiteQuadrature(int n)
		{
			this.n = n;

			ComputeNodes(n, out x, out w);
		}

		/// <summary>
		/// Computes the integral $\int_{-\infty}^\infty e^{-x^2}f(x)$ using n-point Gauss-Hermite quadrature.
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
		/// Computes the integral $\int_{-\infty}^\infty e^{-x^2}f(x)$ using n-point Gauss-Hermite quadrature.
		/// </summary>
		public static double Integrate(Func<double, double> f, int n)
		{
			return new GaussHermiteQuadrature(n).Integrate(f);
		}

		/// <summary>
		/// Computes the n-point Gauss-Hermite quadrature nodes such that $\int_{-\infty}^\infty e^{-x^2}f(x)dx\approxeq\sum_iw_if(x_i)$.
		/// </summary>
		public static void ComputeNodes(int n, out double[] x, out double[] w)
		{
			if (n < 1)
			{
				throw new ArgumentException("Invalid number of nodes.");
			}

			// See also:
			// http://mathworld.wolfram.com/Hermite-GaussQuadrature.html
			// http://www.alglib.net/integral/gq/ghermite.php

			x = new double[n];
			w = new double[n];

			double pipm4 = Math.Pow(Math.PI, -0.25);
			double r = 0;

			for (int i = 0; i < (n + 1) / 2; i++)
			{
				if (i == 0)
				{
					r = Math.Sqrt(2.0 * n + 1.0) - 1.85575 * Math.Pow(2.0 * n + 1.0, -1.0 / 6.0);
				}
				else if (i == 1)
				{
					r = r - 1.14 * Math.Pow(n, 0.426) / r;
				}
				else if (i == 2)
				{
					r = 1.86 * r - 0.86 * x[0];
				}
				else if (i == 3)
				{
					r = 1.91 * r - 0.91 * x[1];
				}
				else
				{
					r = 2.0 * r - x[i - 2];
				}

				double r1, dp3;
				do
				{
					double p2 = 0.0;
					double p3 = pipm4;
					for (int j = 0; j < n; j++)
					{
						double p1 = p2;
						p2 = p3;
						p3 = p2 * r * Math.Sqrt(2.0 / (j + 1.0)) - p1 * Math.Sqrt(j / (j + 1.0));
					}
					dp3 = Math.Sqrt(2.0 * n) * p2;
					r1 = r;
					r = r - p3 / dp3;
				} while (Math.Abs(r - r1) >= 5.0e-14 * (1.0 + Math.Abs(r)));

				x[i] = r;
				w[i] = 2.0 / (dp3 * dp3);
				x[n - 1 - i] = -x[i];
				w[n - 1 - i] = w[i];
			}
		}
	}
}
