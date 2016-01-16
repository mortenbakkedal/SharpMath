// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using SharpMath.Collections;
using SharpMath.Plotting.Functions;

namespace SharpMath.Plotting
{
	[Serializable]
	public class FunctionPlot : IFunctionPlot
	{
		public FunctionPlot(Func<double, double> function, double a, double b, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), a, b, styles)
		{
		}

		public FunctionPlot(Func<double, double> function, double a, double b, int n, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), a, b, n, styles)
		{
		}

		public FunctionPlot(Func<double, double> function, double a, double b, int n, double tolerance, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), a, b, n, tolerance, styles)
		{
		}
	
		public FunctionPlot(Func<double, double> function, double[] points, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), points, styles)
		{
		}

		public FunctionPlot(IPlotFunction function, double a, double b, params IFunctionPlotStyle[] styles)
			: this(function, a, b, 1000, styles)
		{
		}

		public FunctionPlot(IPlotFunction function, double a, double b, int n, params IFunctionPlotStyle[] styles)
			: this(function, new FunctionPlotSegment[] { new FunctionPlotSegment(a, b, n) }, styles)
		{
		}
		
		public FunctionPlot(IPlotFunction function, double a, double b, int n, double tolerance, params IFunctionPlotStyle[] styles)
			: this(function, new FunctionPlotSegment[] { new FunctionPlotSegment(a, b, n, tolerance) }, styles)
		{
		}

		public FunctionPlot(IPlotFunction function, double[] points, params IFunctionPlotStyle[] styles)
			: this(function, new FunctionPlotSegment[] { new FunctionPlotSegment(points) }, styles)
		{
		}

		public FunctionPlot(ILinearInterpolationFunction function, double a, double b, params IFunctionPlotStyle[] styles)
			: this((IPlotFunction)function, new FunctionPlotSegment[] { new FunctionPlotSegment(FunctionPlotSegment.CreatePoints(a, b, function)) }, styles)
		{
		}

		public FunctionPlot(Func<double, double> function, params FunctionPlotSegment[] segments)
			: this(function, (IEnumerable<FunctionPlotSegment>)segments)
		{
		}

		public FunctionPlot(Func<double, double> function, IEnumerable<FunctionPlotSegment> segments, params IFunctionPlotStyle[] styles)
			: this(PlotFunction.Create(function), segments, styles)
		{
		}

		public FunctionPlot(IPlotFunction function, params FunctionPlotSegment[] segments)
			: this(function, (IEnumerable<FunctionPlotSegment>)segments)
		{
		}

		public FunctionPlot(IPlotFunction function, IEnumerable<FunctionPlotSegment> segments, params IFunctionPlotStyle[] styles)
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

			/*if (function is LinearInterpolationFunction)
			{
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

			return string.Format("'{0}' using 1:2 with lines {1} {2} {3}",
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
