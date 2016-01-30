// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	public interface IVariableAssignment
	{
		IVariable Variable
		{
			get;
		}

		double Value
		{
			get;
		}
	}
}
