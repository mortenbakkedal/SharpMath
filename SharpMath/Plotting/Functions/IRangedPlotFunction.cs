// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.Functions
{
	public interface IRangedPlotFunction : IPlotFunction
	{
		double XMin
		{
			get;
		}

		double XMax
		{
			get;
		}
	}
}
