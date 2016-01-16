// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using SharpMath.LinearAlgebra;

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	public class PolynomialRegression
	{
		private SimpleLinearRegression lr;

		public PolynomialRegression(IEnumerable<double> y, IEnumerable<double> x, int order)
		{
			if (order < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			lr = SimpleLinearRegression.Fit(y, x.Select(t => Transform(t, order)), false);

			Order = order;
			Coefficients = lr.Beta;
		}

		public int Order
		{
			get;
			private set;
		}

		public Vector Coefficients
		{
			get;
			private set;
		}

		private static Vector Transform(double x, int n)
		{
			double[] a = new double[n + 1];
			a[0] = 1.0;
			for (int i = 1; i <= n; i++)
			{
				a[i] = a[i - 1] * x;
			}

			return new Vector(a);
			//return new Vector(1.0, x, x * x);
		}

		public double Value(double x)
		{
			return lr.Value(Transform(x, Order));
		}
	}
}
