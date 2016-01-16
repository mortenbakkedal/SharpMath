// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class FunctionEqualityConstraint : FunctionConstraint
	{
		public FunctionEqualityConstraint(Function function, double value)
			: base(function, value, value)
		{
			Value = value;
		}

		public double Value
		{
			get;
			private set;
		}
	}
}
