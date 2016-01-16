// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class BfgsOptimizerResult : OptimizerResult
	{
		public BfgsOptimizerResult(bool status, IPoint optimalPoint, double optimalValue, BfgsOptimizerConvergenceStatus convergenceStatus)
			: base(status, optimalPoint, optimalValue)
		{
			ConvergenceStatus = convergenceStatus;
		}

		public BfgsOptimizerConvergenceStatus ConvergenceStatus
		{
			get;
			private set;
		}
	}
}
