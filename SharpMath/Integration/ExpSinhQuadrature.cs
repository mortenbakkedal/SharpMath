// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Integration
{
	public sealed class ExpSinhQuadrature
	{
		private int n;
		private double[] x, w;

		public ExpSinhQuadrature(int n1, int n2, double h)
		{
			n = n1 + n2 + 1;
			ComputeNodes(n1, n2, h, out x, out w);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty f(x)dx$ using the Double Exponential quadrature.
		/// </summary>
		public double Integrate(Func<double, double> f)
		{
			double s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += f(x[i]) * w[i];
			}
			
			return s;
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty f(x)dx$ using the Double Exponential quadrature.
		/// </summary>
		public static double Integrate(Func<double, double> f, int n1, int n2, double h)
		{
			return new ExpSinhQuadrature(n1, n2, h).Integrate(f);
		}

		public static void ComputeNodes(int n1, int n2, double h, out double[] x, out double[] w)
		{
			if (n1 < 0 || n2 < 0)
			{
				throw new ArgumentException("Invalid number of mesh points.");
			}

			if (h <= 0.0)
			{
				throw new ArgumentException("Invalid mesh size.");
			}

			int n = n1 + n2 + 1;

			x = new double[n];
			w = new double[n];

			for (int i = 0; i < n; i++)
			{
				double t = (i - n1) * h;
				x[i] = Math.Exp(0.5 * Math.PI * Math.Sinh(t));
				w[i] = 0.5 * Math.PI * h * Math.Cosh(t) * x[i];
				Console.WriteLine("{0}\t{1}\t{2}", t, x[i], w[i]);
			}
			Console.WriteLine("--");
		}
	}
}
