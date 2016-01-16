// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	public class VariableEqualityConstraintCollection : List<VariableEqualityConstraint>
	{
		public void Add(Variable variable, double value)
		{
			Add(new VariableEqualityConstraint(variable, value));
		}

		public void Add(IEnumerable<VariableEqualityConstraint> constraints)
		{
			AddRange(constraints);
		}

		public void Add(params VariableEqualityConstraint[] constraints)
		{
			Add((IEnumerable<VariableEqualityConstraint>)constraints);
		}
	}
}
