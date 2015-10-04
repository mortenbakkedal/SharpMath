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
