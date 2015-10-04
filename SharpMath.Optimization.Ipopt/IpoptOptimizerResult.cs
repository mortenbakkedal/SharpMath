// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization.Ipopt
{
	[Serializable]
	public class IpoptOptimizerResult : OptimizerResult
	{
		public IpoptOptimizerResult(bool status, bool hasConverged, IPoint optimalPoint, double optimalValue, IpoptReturnCode returnCode, int iterations)
			: base(status, optimalPoint, optimalValue)
		{
			HasConverged = hasConverged;
			ReturnCode = returnCode;
			Iterations = iterations;
		}

		/// <summary>
		/// Like <see cref="Status" />, but is only true if the convergence criteria has been met.
		/// </summary>
		public bool HasConverged
		{
			get;
			private set;
		}

		public IpoptReturnCode ReturnCode
		{
			get;
			private set;
		}

		/// <summary>
		/// Number of iterations performed.
		/// </summary>
		public int Iterations
		{
			get;
			private set;
		}
	}
}
