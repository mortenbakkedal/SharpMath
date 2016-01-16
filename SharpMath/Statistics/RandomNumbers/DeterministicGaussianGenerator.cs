// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

using SharpMath.LinearAlgebra;

namespace SharpMath.Statistics.RandomNumbers
{
	[Serializable]
	public class DeterministicGaussianGenerator : IGaussianRandomGenerator
	{
		public DeterministicGaussianGenerator(Vector values)
		{
			Values = values;
			NextIndex = 0;
		}

		public DeterministicGaussianGenerator(IEnumerable<double> values)
			: this(new Vector(values))
		{
		}

		public Vector Values
		{
			get;
			private set;
		}

		public int NextIndex
		{
			get;
			private set;
		}

		public double NextGaussian()
		{
			return Values[NextIndex++];
		}
	}
}
