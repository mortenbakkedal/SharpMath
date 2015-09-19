// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class VariableConstraint : FunctionConstraint
	{
		public VariableConstraint(Variable variable, double minValue, double maxValue)
			: base(variable, minValue, maxValue)
		{
			Variable = variable;
		}

		public Variable Variable
		{
			get;
			private set;
		}
	}
}
