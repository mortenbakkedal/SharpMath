// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class FunctionConstraint
	{
		public FunctionConstraint(Function function, double minValue, double maxValue)
		{
			Function = function;
			MinValue = minValue;
			MaxValue = maxValue;
		}

		public Function Function
		{
			get;
			private set;
		}

		public double MinValue
		{
			get;
			private set;
		}

		public double MaxValue
		{
			get;
			private set;
		}
	}
}
