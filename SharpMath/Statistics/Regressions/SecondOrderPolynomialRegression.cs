// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	public class SecondOrderPolynomialRegression : PolynomialRegression
	{
		public SecondOrderPolynomialRegression(IEnumerable<double> y, IEnumerable<double> x)
			: base(y, x, 2)
		{
			A = Coefficients[2];
			B = Coefficients[1];
			C = Coefficients[0];

			X0 = -B / (2.0 * A);
			Y0 = -(B * B - 4.0 * A * C) / (4.0 * A);
		}

		public double A
		{
			get;
			private set;
		}

		public double B
		{
			get;
			private set;
		}

		public double C
		{
			get;
			private set;
		}

		public double X0
		{
			get;
			private set;
		}

		public double Y0
		{
			get;
			private set;
		}
	}
}
