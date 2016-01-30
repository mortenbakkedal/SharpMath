// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	[Serializable]
	[DebuggerStepThrough]
	public class VariableAssignment : IVariableAssignment
	{
		public VariableAssignment(Variable variable, double value)
		{
			Variable = variable;
			Value = value;
		}

		public static VariableAssignment[] Create(IDictionary<Variable, double> assignments)
		{
			return new List<KeyValuePair<Variable, double>>(assignments).ConvertAll<VariableAssignment>(assignment => new VariableAssignment(assignment.Key, assignment.Value)).ToArray();
		}

		public static VariableAssignment[] Create(Variable[] variables, double[] values)
		{
			int n = variables.Length;

			if (values.Length != n)
			{
				throw new ArgumentException("Number number of variables and the number of values don't agree.");
			}

			List<VariableAssignment> assignments = new List<VariableAssignment>();
			for (int i = 0; i < n; i++)
			{
				assignments.Add(new VariableAssignment(variables[i], values[i]));
			}

			return assignments.ToArray();
		}

		/*public static implicit operator VariableFunctionAssignment(VariableAssignment assignment)
		{
			// Allow a constant value assignment to be used in Function.Substitute.
			return new VariableFunctionAssignment(assignment.Variable, assignment.Value);
		}*/

		public Variable Variable
		{
			get;
			private set;
		}

		public double Value
		{
			get;
			private set;
		}
		

		public IFunction SubstitutionFunction
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		IVariable IVariableAssignment.Variable
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
