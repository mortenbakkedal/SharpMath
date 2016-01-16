// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Representation of BFGS numerical status codes.
	/// </summary>
	[Serializable]
	public enum BfgsOptimizerConvergenceStatus
	{
		Unknown = 0,
		FunctionDecreaseLessThanEpsF = 1,
		StepLessThanEpsX = 2,
		GradientNormLessThanEpsG = 4,
		MaxIterationsExceeded = 5
	}
}
