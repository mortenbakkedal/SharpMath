// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	public interface IFunction
	{
		double Value(IEnumerable<IVariableAssignment> assignments);

		/// Operations represents compounding of functions (if g(y) is a function of the variable y and f(x) is a function of the variable x, then g(f(x)) is the compounded function of the variable x).
		IFunction Substitute(IEnumerable<IVariableSubstitution> substitutions);

		IFunction Derivative(IVariable variable);
	}
}
