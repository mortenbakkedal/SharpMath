// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Statistics.RandomNumbers
{
	public class PolarBoxMullerGenerator : IGaussianRandomGenerator
	{
		private IRandomGenerator uniformGenerator;
		private double next;
		private bool hasNext;

		public PolarBoxMullerGenerator(IRandomGenerator uniformGenerator)
		{
			this.uniformGenerator = uniformGenerator;
		}

		public double NextGaussian()
		{
			if (hasNext)
			{
				hasNext = false;
				return next;
			}
			else
			{
				double x, y, w;
				do
				{
					x = 2.0 * uniformGenerator.NextUniform() - 1.0;
					y = 2.0 * uniformGenerator.NextUniform() - 1.0;
					w = x * x + y * y;
				}
				while (w >= 1.0 || w == 0.0);

				w = Math.Sqrt(-2.0 * Math.Log(w) / w);

				x *= w;
				y *= w;
				next = y;
				hasNext = true;
				return x;
			}
		}
	}
}
