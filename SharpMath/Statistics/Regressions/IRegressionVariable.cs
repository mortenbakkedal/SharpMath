// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Statistics.Regressions
{
	public interface IRegressionVariable
	{
		IEnumerable<double> Values
		{
			get;
		}

		/// <summary>
		/// Whether or not this variable is required in a regression fitting.
		/// </summary>
		bool Required
		{
			get;
		}
	}

	public interface IRegressionVariable<T> : IRegressionVariable
	{
		double Transform(T value);
	}
}
