// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class PlotPoint : IPlotPoint
	{
		public PlotPoint(double x, double y)
		{
			X = x;
			Y = y;
		}

		public double X
		{
			get;
			private set;
		}

		public double Y
		{
			get;
			private set;
		}
	}
}
