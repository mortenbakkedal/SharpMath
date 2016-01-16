// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

using SharpMath.LinearAlgebra;

#pragma warning disable

namespace SharpMath.Statistics.Regressions
{
	/// <summary>
	/// Linear regression.
	/// http://en.wikipedia.org/w/index.php?title=Linear_regression&oldid=595475658#Introduction_to_linear_regression
	/// </summary>
	[Serializable]
	public class SimpleLinearRegression
	{
		private int p;

		private SimpleLinearRegression(Vector y, Matrix x, double alpha, Vector beta, int p)
		{
			this.p = p;

			Y = y;
			X = x;
			Alpha = alpha;
			Beta = beta;
		}

		/// <summary>
		/// 1-dimensional linear regression.
		/// </summary>
		public static SimpleLinearRegression Fit(Vector y, Vector x)
		{
			return Fit(y, (Matrix)x);
		}

		public static SimpleLinearRegression Fit(Vector y, Vector x, bool intercept)
		{
			return Fit(y, (Matrix)x, intercept);
		}

		public static SimpleLinearRegression Fit(Vector y, Matrix x)
		{
			return Fit(y, x, false);
		}

		public static SimpleLinearRegression Fit(Vector y, Matrix x, bool intercept)
		{
			if (y == null || x == null)
			{
				throw new ArgumentNullException();
			}

			// Number of fitting points for the linear regression.
			int n = y.Length;

			// And the dimension.
			int p = x.Columns;

			if (x.Rows != n)
			{
				throw new ArgumentException("Dimensions doesn't match.");
			}

			double alpha;
			Vector beta;

			throw new NotImplementedException();
			/*if (intercept)
			{
				double[,] a = new double[n, p + 1];
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < p; j++)
					{
						a[i, j] = x[i, j];
					}
					a[i, p] = 1.0;
				}

				Vector b = SingularValueDecomposition.PseudoInverse(new Matrix(a)) * y;
				beta = b.GetVector(0, p);
				alpha = b[p];
			}
			else
			{
				//beta = Matrix.Inverse(Matrix.Transpose(x) * x) * Matrix.Transpose(x) * y;
				beta = SingularValueDecomposition.PseudoInverse(x) * y;
				alpha = 0.0;
			}

			return new SimpleLinearRegression(y, x, alpha, beta, p);*/
		}

		public static SimpleLinearRegression Fit(IEnumerable<double> y, IEnumerable<Vector> x)
		{
			return Fit(new Vector(y), CreateX(new List<Vector>(x).ToArray()));
		}

		public static SimpleLinearRegression Fit(IEnumerable<double> y, IEnumerable<Vector> x, bool intercept)
		{
			return Fit(new Vector(y), CreateX(new List<Vector>(x).ToArray()), intercept);
		}

		private static Matrix CreateX(Vector[] x)
		{
			int n = x.Length;
			int p = x[0].Length;

			double[,] x2 = new double[n, p];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < p; j++)
				{
					if (x[i].Length != p)
					{
						throw new ArgumentException();
					}

					x2[i, j] = x[i][j];
				}
			}

			return new Matrix(x2);
		}

		public double Value(double x)
		{
			if (p != 1)
			{
				throw new NotSupportedException("Only supported for the 1-dimensional case.");
			}

			return Value(new Vector(new double[] { x }));
		}

		public double Value(Vector x)
		{
			if (x == null)
			{
				throw new ArgumentNullException();
			}

			if (x.Length != p)
			{
				throw new ArgumentException("Dimensions doesn't match.");
			}

			// The actual independent variables are treated as a row-vector, just as in the estimation.
			//return (Alpha + Matrix.Transpose(x) * Beta)[0];
			return Value(Matrix.Transpose(x));
		}

		private double Value(Matrix x)
		{
			return (Alpha + x * Beta)[0];
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

		public double Alpha
		{
			get;
			private set;
		}

		public Vector Beta
		{
			get;
			private set;
		}

		/*public Vector Epsilon
		{
			get;
			private set;
		}

		public double Sigma0
		{
			get;
			private set;
		}

		public Vector Sigma
		{
			get;
			private set;
		}*/
	}
}
