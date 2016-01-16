// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization.DualNumbers
{
	[Serializable]
	public class Tensor
	{
		private double[, ,] entries;

		public Tensor(double[, ,] entries)
		{
			this.entries = (double[, ,])entries.Clone();
		}

		public double this[int i, int j, int k]
		{
			get
			{
				return entries[i, j, k];
			}
		}
	}

	/*[Serializable]
	public abstract class Tensor
	{
		public abstract double this[int i, int j, int k]
		{
			get;
		}
	}*/
}
