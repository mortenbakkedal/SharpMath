// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Statistics
{
	[Serializable]
	public class MeanStatistics
	{
		private double mean;
		private long samples;

		public void Add(double x)
		{
			samples++;
			mean += (x - mean) / samples;
		}

		public void Clear()
		{
			mean = 0.0;
			samples = 0;
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
	}
}
