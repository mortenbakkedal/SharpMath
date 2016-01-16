// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	public class FunctionConstraintCollection : List<FunctionConstraint>
	{
		public void Add(Function function, double minValue, double maxValue)
		{
			Add(new FunctionConstraint(function, minValue, maxValue));
		}

		public void Add(IEnumerable<FunctionConstraint> constraints)
		{
			AddRange(constraints);
		}

		public void Add(params FunctionConstraint[] constraints)
		{
			Add((IEnumerable<FunctionConstraint>)constraints);
		}
	}
}
