// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Statistics
{
	[Serializable]
	public class RootMeanSquareStatistics
	{
		private MeanStatistics squares;

		public RootMeanSquareStatistics()
		{
			squares = new MeanStatistics();
		}

		public RootMeanSquareStatistics(IEnumerable<double> values)
			: this()
		{
			foreach (double value in values)
			{
				Add(value);
			}
		}

		public void Add(double x)
		{
			squares.Add(x * x);
		}

		public int Samples
		{
			get
			{
				return squares.Samples;
			}
		}

		public double RootMeanSquare
		{
			get
			{
				return Math.Sqrt(squares.Mean);
			}
		}
	}
}
