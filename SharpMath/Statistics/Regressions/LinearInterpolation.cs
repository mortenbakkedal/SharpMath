// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	public class LinearInterpolation
	{
		private int n;

		public LinearInterpolation(Vector x, Vector y)
		{
			if (x == null || y == null)
			{
				throw new ArgumentNullException();
			}

			n = x.Length;
			if (y.Length != n)
			{
				throw new ArgumentException();
			}

			for (int i = 1; i < n; i++)
			{
				if (x[i - 1] >= x[i])
				{
					throw new ArgumentException();
				}
			}

			X = x;
			Y = y;
		}

		public double Value(double x)
		{
			if (n == 0)
			{
				return double.NaN;
			}

			if (x <= X[0])
			{
				return Y[0];
			}

			for (int i = 1; i < n; i++)
			{
				if (x <= X[i])
				{
					double lambda = (X[i] - x) / (X[i] - X[i - 1]);
					return (1.0 - lambda) * Y[i] + lambda * Y[i - 1];
				}
			}

			return Y[n - 1];
		}

		public Vector X
		{
			get;
			private set;
		}

		public Vector Y
		{
			get;
			private set;
		}

		/*[Serializable]
		public class Point
		{
			public double X
			{
				get;
				private set;
			}

			public double Y
			{
				get;
				private set;
			}
		}*/
	}
}
