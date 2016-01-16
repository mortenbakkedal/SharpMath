// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Statistics
{
	[Serializable]
	public class CovarianceStatistics
	{
		private long samples;
		private double meanX, meanY;
		private double productX, coproductXY, productY;

		public void Add(double x, double y)
		{
			// Use this single pass algorithm to update correlation. See http://en.wikipedia.org/wiki?title=Talk:Correlation.
			//
			// sum_sq_x = 0
			// sum_sq_y = 0
			// sum_coproduct = 0
			// mean_x = x[1]
			// mean_y = y[1]
			// for i in 2 to N:
			//     sweep = (i - 1.0) / i
			//     delta_x = x[i] - mean_x
			//     delta_y = y[i] - mean_y
			//     sum_sq_x += delta_x * delta_x * sweep
			//     sum_sq_y += delta_y * delta_y * sweep
			//     sum_coproduct += delta_x * delta_y * sweep
			//     mean_x += delta_x / i
			//     mean_y += delta_y / i 
			// pop_sd_x = sqrt( sum_sq_x / N )
			// pop_sd_y = sqrt( sum_sq_y / N )
			// cov_x_y = sum_coproduct / N
			// correlation = cov_x_y / (pop_sd_x * pop_sd_y)

			if (samples++ == 0)
			{
				meanX = x;
				meanY = y;
			}
			else
			{
				double sweep = (samples - 1.0) / samples;

				double deltaX = x - meanX;
				double deltaY = y - meanY;

				productX += deltaX * deltaX * sweep;
				productY += deltaY * deltaY * sweep;
				coproductXY += deltaX * deltaY * sweep;

				meanX += deltaX / samples;
				meanY += deltaY / samples;
			}
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

		public double MeanX
		{
			get
			{
				return meanX;
			}
		}

		public double MeanY
		{
			get
			{
				return meanY;
			}
		}

		public double VarianceX
		{
			get
			{
				return productX / samples;
			}
		}

		public double VarianceY
		{
			get
			{
				return productY / samples;
			}
		}

		public double Covariance
		{
			get
			{
				return coproductXY / samples;
			}
		}

		public double StDevX
		{
			get
			{
				return Math.Sqrt(VarianceX);
			}
		}

		public double StDevY
		{
			get
			{
				return Math.Sqrt(VarianceY);
			}
		}

		public double Correlation
		{
			get
			{
				return Covariance / (StDevX * StDevY);
			}
		}
	}
}
