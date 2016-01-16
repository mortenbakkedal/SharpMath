// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	public interface IEvaluator<T>
	{
		T Evaluate(Function function);

		IPoint Point
		{
			get;
		}
	}

	public interface IEvaluator : IEvaluator<double>
	{
	}
}
