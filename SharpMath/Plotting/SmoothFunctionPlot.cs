// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using SharpMath.Collections;
using SharpMath.Plotting.Functions;

namespace SharpMath.Plotting
{
	/// <summary>
	/// Translates a generic function to a set points in gnuplot connected through a smooth cubic spline. 
	/// This makes the representation more compact (faster to process in case of TeX output) and the final plot more smooth
	/// compared to the simpler FunctionPlot.
	/// </summary>
	[Serializable]
	public class SmoothFunctionPlot : IFunctionPlot
	{
		public SmoothFunctionPlot(Func<double, double> function, double a, double b, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), a, b, styles)
		{
		}

		public SmoothFunctionPlot(Func<double, double> function, double a, double b, int n, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), a, b, n, styles)
		{
		}

		public SmoothFunctionPlot(Func<double, double> function, double[] points, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), points, styles)
		{
		}

		public SmoothFunctionPlot(IPlotFunction function, double a, double b, params IFunctionPlotStyle[] styles)
			: this(function, a, b, 1000, styles)
		{
		}

		public SmoothFunctionPlot(IPlotFunction function, double a, double b, int n, params IFunctionPlotStyle[] styles)
			: this(function, new FunctionPlotSegment[] { new FunctionPlotSegment(a, b, n) }, styles)
		{
		}
		
		public SmoothFunctionPlot(IPlotFunction function, double[] points, params IFunctionPlotStyle[] styles)
			: this(function, new FunctionPlotSegment[] { new FunctionPlotSegment(points) }, styles)
		{
		}

		public SmoothFunctionPlot(Func<double, double> function, params FunctionPlotSegment[] segments)
			: this(function, (IEnumerable<FunctionPlotSegment>)segments)
		{
		}

		public SmoothFunctionPlot(Func<double, double> function, IEnumerable<FunctionPlotSegment> segments, params IPlotStyle[] styles)
			: this(PlotFunction.Create(function), segments, styles)
		{
		}

		public SmoothFunctionPlot(IPlotFunction function, params FunctionPlotSegment[] segments)
			: this(function, (IEnumerable<FunctionPlotSegment>)segments)
		{
		}

		public SmoothFunctionPlot(IPlotFunction function, IEnumerable<FunctionPlotSegment> segments, params IPlotStyle[] styles)
		{
			Function = function;
			Segments = new ImmutableList<FunctionPlotSegment>(segments);
			Properties = new PlotProperties(styles);
		}

		public string Generate(Gnuplot plot)
		{
			if (Segments.Count != 1)
			{
				throw new NotImplementedException();
			}

			/*if (Segments[0].Points.Count < 3)
			{
				throw new Exception("Cubic spline needs at least three points.");
			}*/

			string fileName = Path.GetRandomFileName();
			using (StreamWriter writer = new StreamWriter(Path.Combine(plot.WorkingDirectory.FullName, fileName)))
			{
				foreach (double x in Segments[0].CreatePoints(Function))
				{
					double y = Function.Value(x);
					writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1}", x, y));
				}
				writer.Close();
			}

			// Format specification here:
			// http://www.gnuplot.info/docs_4.2/gnuplot.html#x1-10100034

			return string.Format("'{0}' using 1:2 smooth csplines {1} {2} {3}",
				fileName,
				Properties.Axes != null ? "axes " + Properties.Axes.Generate(plot) : "",
				Properties.Label != null ? "title '" + Properties.Label.Generate(plot) + "'" : "notitle",
				Properties.Generate(this, plot));
		}

		public IPlotFunction Function
		{
			get;
			private set;
		}

		public ImmutableList<FunctionPlotSegment> Segments
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
