// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	public class VariableConstraintCollection : List<VariableConstraint>
	{
		public void Add(Variable variable, double minValue, double maxValue)
		{
			Add(new VariableConstraint(variable, minValue, maxValue));
		}

		public void Add(IEnumerable<VariableConstraint> constraints)
		{
			AddRange(constraints);
		}

		public void Add(params VariableConstraint[] constraints)
		{
			Add((IEnumerable<VariableConstraint>)constraints);
		}
	}
}
