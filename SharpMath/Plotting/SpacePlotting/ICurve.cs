// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.SpacePlotting
{
	public interface ICurve
	{
		ISpacePoint Value(double t);
	}
}
