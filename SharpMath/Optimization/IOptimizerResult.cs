// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	public interface IOptimizerResult
	{
		/// <summary>
		/// Indicates if the optimization has succeeded. Is true if the predefined convergence criteria has been met
		/// or if the maximum number of iterations has been exceeded; false if some error has occured.
		/// </summary>
		bool Status
		{
			get;
		}

		IPoint OptimalPoint
		{
			get;
		}

		double OptimalValue
		{
			get;
		}
	}
}
