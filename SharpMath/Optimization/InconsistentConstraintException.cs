// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class InconsistentConstraintException : ConstraintViolationException
	{
		public InconsistentConstraintException(string message, FunctionConstraint constraint)
			: base(message, constraint)
		{
		}
	}
}
