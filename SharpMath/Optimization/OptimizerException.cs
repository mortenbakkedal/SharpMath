// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class OptimizerException : Exception
	{
		public OptimizerException()
		{
		}

		public OptimizerException(string message)
			: base(message)
		{
		}
	}
}
