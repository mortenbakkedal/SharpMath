// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using SharpMath.Plotting.Functions;

namespace SharpMath.Plotting.SpacePlotting
{
	[Serializable]
	public class SurfacePlot : ISpacePlot
	{
		public SurfacePlot(Func<double, double, double> surface, double a1, double b1, double a2, double b2, params IFunctionPlotStyle[] styles)
			: this(SpacePlotting.Surface.Create(surface), a1, b1, a2, b2, styles)
		{
		}

		public SurfacePlot(Func<double, double, double> surface, double a1, double b1, int n1, double a2, double b2, int n2, params IFunctionPlotStyle[] styles)
			: this(SpacePlotting.Surface.Create(surface), a1, b1, n1, a2, b2, n2, styles)
		{
		}

		public SurfacePlot(ISurface surface, double a1, double b1, double a2, double b2, params IFunctionPlotStyle[] styles)
			: this(surface, a1, b1, 20, a2, b2, 20, styles)
		{
		}

		public SurfacePlot(ISurface surface, double a1, double b1, int n1, double a2, double b2, int n2, params IFunctionPlotStyle[] styles)
		{
			Surface = surface;
			Segment1 = new FunctionPlotSegment(a1, b1, n1);
			Segment2 = new FunctionPlotSegment(a2, b2, n2);
			Properties = new PlotProperties(styles);
		}

		public string Generate(Gnuplot plot)
		{
			// Many samples here:
			// http://gnuplot.sourceforge.net/demo/contours.html

			/*// Override to align with specified x values for LinearInterpolationSurface.
			if (surface is LinearInterpolationSurface && ((LinearInterpolationSurface)surface).Projection == LinearInterpolationSurfaceProjection.XZPlane)
			{
				xs = ((LinearInterpolationSurface)surface).Values;
			}

			// Or with the specified y values.
			if (surface is LinearInterpolationSurface && ((LinearInterpolationSurface)surface).Projection == LinearInterpolationSurfaceProjection.YZPlane)
			{
				ys = ((LinearInterpolationSurface)surface).Values;
			}*/

			string fileName = Path.GetRandomFileName();
			using (StreamWriter writer = new StreamWriter(Path.Combine(plot.WorkingDirectory.FullName, fileName)))
			{
				// Using a dummy plot function here.
				IPlotFunction function = PlotFunction.Create(x => 0.0);

				foreach (double x in Segment1.CreatePoints(function))
				{
					foreach (double y in Segment2.CreatePoints(function))
					{
						double z = Surface.Value(x, y);
						writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", x, y, z));
					}
					writer.WriteLine();
				}
				writer.Close();
			}

			return string.Format("'{0}' using 1:2:3 with lines {1} {2} {3}",
				fileName,
				Properties.Axes != null ? "axes " + Properties.Axes.Generate(plot) : "",
				Properties.Label != null ? "title '" + Properties.Label.Generate(plot) + "'" : "notitle",
				Properties.Generate(this, plot));
		}

		public ISurface Surface
		{
			get;
			private set;
		}

		public FunctionPlotSegment Segment1
		{
			get;
			private set;
		}

		public FunctionPlotSegment Segment2
		{
			get;
			private set;
		}

		public PlotProperties Properties
		{
			get;
			private set;
		}
	}
}
