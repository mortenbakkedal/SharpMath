// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Plotting.Functions
{
	public interface ILinearInterpolationFunction : IPlotFunction
	{
		IEnumerable<IPlotPoint> Points
		{
			get;
		}
	}
}
