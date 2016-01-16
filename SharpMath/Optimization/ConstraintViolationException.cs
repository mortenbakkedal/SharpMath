// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class ConstraintViolationException : OptimizerException
	{
		public ConstraintViolationException(string message)
			: this(message, null)
		{
		}

		public ConstraintViolationException(string message, FunctionConstraint constraint)
			: base(message)
		{
			Constraint = constraint;
		}

		public FunctionConstraint Constraint
		{
			get;
			private set;
		}
	}
}
