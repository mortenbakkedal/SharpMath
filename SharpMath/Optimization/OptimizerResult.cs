// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	[Serializable]
	public class OptimizerResult : IOptimizerResult
	{
		public OptimizerResult(bool status, IPoint optimalPoint, double optimalValue)
		{
			Status = status;
			OptimalPoint = optimalPoint;
			OptimalValue = optimalValue;
		}

		public bool Status
		{
			get;
			private set;
		}

		public IPoint OptimalPoint
		{
			get;
			private set;
		}

		public double OptimalValue
		{
			get;
			private set;
		}
	}
}
