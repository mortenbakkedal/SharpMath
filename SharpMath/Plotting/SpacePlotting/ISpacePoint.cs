// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.SpacePlotting
{
	/// <summary>
	/// Represents a point in the 3-dimensional space.
	/// </summary>
	public interface ISpacePoint
	{
		double X
		{
			get;
		}

		double Y
		{
			get;
		}

		double Z
		{
			get;
		}
	}
}
