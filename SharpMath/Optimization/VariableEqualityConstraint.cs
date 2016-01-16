// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	public class VariableEqualityConstraint : VariableConstraint
	{
		public VariableEqualityConstraint(Variable variable, double value)
			: base(variable, value, value)
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
