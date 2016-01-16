// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.LinearAlgebra;

namespace SharpMath.Statistics.Regressions
{
	/// <summary>
	/// Numerically stable linear regression estimation using the SVD decomposition.
	/// </summary>
	[Serializable]
	public class LinearRegression
	{
		private LinearRegression(Vector y, Matrix x, Vector beta, Vector sigma, double sigma0)
		{
			Y = y;
			X = x;
			Beta = beta;
			Sigma = sigma;
			Sigma0 = sigma0;
		}

		/*public static void Test()
		{
			// Sample from the paper.
			Vector y = new Vector(41.38, 31.01, 37.41, 50.05, 39.17, 38.86, 46.14, 44.47);
			Matrix x = new Matrix(
				new double[,] {
				{ 1.0, 16.85, 1.46 },
				{ 1.0, 24.81, -4.61 },
				{ 1.0, 18.85, -0.21 },
				{ 1.0, 12.63, 4.93 },
				{ 1.0, 21.38, -1.36 },
				{ 1.0, 18.78, -0.08 },
				{ 1.0, 15.58, 2.98 },
				{ 1.0, 16.30, 1.73 }}
			);

			LinearRegression r = Fit(y, x);
		}

		public static void Test2()
		{
			double[] y = new double[] { 52.21, 53.12, 54.48, 55.84, 57.20, 58.57, 59.93, 61.29, 63.11, 64.47, 66.28, 68.10, 69.92, 72.19, 74.46 };
			double[] x = new double[] { 1.47, 1.50, 1.52, 1.55, 1.57, 1.60, 1.63, 1.65, 1.68, 1.70, 1.73, 1.75, 1.78, 1.80, 1.83 };
		}*/

		public static LinearRegression Fit(Vector y, Matrix x)
		{
			return Fit(y, x, false);
		}

		public static LinearRegression Fit(Vector y, Matrix x, bool intercept)
		{
			// John Mandel, Use of the Singular Value Decomposition in Regression Analysis, The American Statistician, Vol. 36, No. 1 (Feb., 1982), pp. 15-24
			// http://www.jstor.org/stable/2684086
			// http://www.ime.unicamp.br/~marianar/MI602/material%20extra/svd-regression-analysis.pdf

			if (y == null || x == null)
			{
				throw new ArgumentNullException();
			}

			if (intercept)
			{
				throw new NotImplementedException();
			}

			int n = x.Rows;
			int p = x.Columns;

			if (y.Length != n)
			{
				throw new ArgumentException();
			}

			throw new NotImplementedException();
			/*// Perform the SVD decomposition.
			SingularValueDecomposition svd = SingularValueDecomposition.Decompose(x);
			Matrix u = svd.U;
			Matrix ut = Matrix.Transpose(u);
			Matrix v = svd.V;
			Vector theta = svd.W;

			Vector alpha = ut * y;

			double[] beta = new double[p];
			for (int j = 0; j < p; j++)
			{
				for (int k = 0; k < p; k++)
				{
					beta[j] += v[j, k] * alpha[k] / theta[k];
				}
			}

			// Estimator of the measurement error variance sigma^2.
			double sigma0 = 0.0;
			for (int i = 0; i < n; i++)
			{
				sigma0 += y[i] * y[i];
			}
			for (int j = 0; j < p; j++)
			{
				sigma0 -= alpha[j] * alpha[j];
			}
			sigma0 = Math.Sqrt(sigma0 / (n - p));

			// Estimator of variance of each beta, given the estimate of measurement variance.
			double[] sigma = new double[p];
			for (int j = 0; j < p; j++)
			{
				for (int k = 0; k < p; k++)
				{
					double s = v[j, k] / theta[k];
					sigma[j] += s * s;
				}
				sigma[j] = Math.Sqrt(sigma[j]) * sigma0;
			}

			return new LinearRegression(y, x, new Vector(beta), new Vector(sigma), sigma0);*/
		}

		public Vector Y
		{
			get;
			private set;
		}

		public Matrix X
		{
			get;
			private set;
		}

		/// <summary>
		/// The $\beta$ coefficients.
		/// </summary>
		public Vector Beta
		{
			get;
			private set;
		}

		/// <summary>
		/// Estimated standard deviation of each of the $\beta$ coefficients.
		/// </summary>
		public Vector Sigma
		{
			get;
			private set;
		}

		/// <summary>
		/// Estimated measurement error standard deviation.
		/// </summary>
		public double Sigma0
		{
			get;
			private set;
		}
	}
}
