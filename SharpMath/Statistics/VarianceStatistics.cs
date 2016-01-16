// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Statistics
{
	/// <summary>
	/// Single pass numerically stable computation of the unbiased variance estimator.
	/// </summary>
	[Serializable]
	public class VarianceStatistics
	{
		private long samples;
		private double mean, m2;

		/*public static bool Test()
		{
			// Test from http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#Example.

			VarianceStatistics vs = new VarianceStatistics();
			vs.Add(1.0e9 + 4.0);
			vs.Add(1.0e9 + 7.0);
			vs.Add(1.0e9 + 13.0);
			vs.Add(1.0e9 + 16.0);
            
			return vs.Variance == 30.0 && vs.Mean - 1.0e9 == 10.0;
		}*/

		public void Add(double x)
		{
			// Using Knuth algorithm. See http://en.wikipedia.org/wiki/Algorithms_for_calculating_variance#On-line_algorithm.

			samples++;

			double delta = x - mean;
			mean += delta / samples;

			// This expression uses the new value of mean.
			m2 += delta * (x - mean);
		}

		public int Samples
		{
			get
			{
				return (int)samples;
			}
		}

		public long LongSamples
		{
			get
			{
				return samples;
			}
		}

		public double Mean
		{
			get
			{
				return mean;
			}
		}

		public double Variance
		{
			get
			{
				return m2 / (samples - 1);
			}
		}

		public double StDev
		{
			get
			{
				return Math.Sqrt(Variance);
			}
		}
	}
}
