// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class VariableSubstitution : IVariableSubstitution
	{
		public VariableSubstitution(Variable variable, IFunction substitutionFunction)
		{
			Variable = variable;
			SubstitutionFunction = substitutionFunction;
		}

		public static implicit operator VariableSubstitution(VariableAssignment assignment)
		{
			return new VariableSubstitution(assignment.Variable, (Function)assignment.Value);
		}

		public IVariable Variable
		{
			get;
			private set;
		}

		public IFunction SubstitutionFunction
		{
			get;
			private set;
		}
	}
}
