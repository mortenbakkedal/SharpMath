// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class VariableNotAssignedException : Exception
	{
		public VariableNotAssignedException(Variable variable)
			: this(variable, null)
		{
		}

		public VariableNotAssignedException(Variable variable, string message)
			: base(message)
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
