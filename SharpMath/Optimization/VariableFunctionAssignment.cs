// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	[Serializable]
	[DebuggerStepThrough]
	public class VariableFunctionAssignment
	{
		public VariableFunctionAssignment(Variable variable, Function function)
		{
			Variable = variable;
			Function = function;
		}

		public Variable Variable
		{
			get;
			private set;
		}

		public Function Function
		{
			get;
			private set;
		}
	}
}
