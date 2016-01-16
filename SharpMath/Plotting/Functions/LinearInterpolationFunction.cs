// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using SharpMath.Collections;

namespace SharpMath.Plotting.Functions
{
	[Serializable]
	public class LinearInterpolationFunction : IRangedPlotFunction, ILinearInterpolationFunction
	{
		public LinearInterpolationFunction(params IPlotPoint[] points)
			: this((IEnumerable<IPlotPoint>)points)
		{
		}

		public LinearInterpolationFunction(IEnumerable<IPlotPoint> points)
		{
			Points = new ImmutableList<IPlotPoint>(points.OrderBy(p => p.X));

			if (Points.Count == 0)
			{
				XMin = double.NaN;
				XMax = double.NaN;
			}
			else
			{
				XMin = Points[0].X;
				XMax = Points[Points.Count - 1].X;
			}
		}

		public double Value(double x)
		{
			if (Points.Count == 0)
			{
				return double.NaN;
			}

			if (x <= Points[0].X)
			{
				return Points[0].Y;
			}

			for (int i = 1; i < Points.Count; i++)
			{
				if (x <= Points[i].X)
				{
					double lambda = (Points[i].X - x) / (Points[i].X - Points[i - 1].X);
					return (1.0 - lambda) * Points[i].Y + lambda * Points[i - 1].Y;
				}
			}

			return Points[Points.Count - 1].Y;
		}

		public ImmutableList<IPlotPoint> Points
		{
			get;
			private set;
		}

		IEnumerable<IPlotPoint> ILinearInterpolationFunction.Points
		{
			get
			{
				return Points;
			}
		}
		
		public double XMin
		{
			get;
			private set;
		}

		public double XMax
		{
			get;
			private set;
		}
	}
}
