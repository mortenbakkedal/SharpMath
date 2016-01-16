// SharpMath - C# Mathematical Library
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
	public class CurvePlot : ISpacePlot
	{
		public CurvePlot(Func<double, ISpacePoint> curve, double a, double b, params IFunctionPlotStyle[] styles)
			: this(SpacePlotting.Curve.Create(curve), a, b, styles)
		{
		}

		public CurvePlot(Func<double, ISpacePoint> curve, double a, double b, int n, params IFunctionPlotStyle[] styles)
			: this(SpacePlotting.Curve.Create(curve), a, b, n, styles)
		{
		}

		public CurvePlot(ICurve curve, double a, double b, params IFunctionPlotStyle[] styles)
			: this(curve, a, b, 1000, styles)
		{
		}

		public CurvePlot(ICurve curve, double a, double b, int n, params IFunctionPlotStyle[] styles)
		{
			Curve = curve;
			Segment = new FunctionPlotSegment(a, b, n);
			Properties = new PlotProperties(styles);
		}

		public string Generate(Gnuplot plot)
		{
			string fileName = Path.GetRandomFileName();
			using (StreamWriter writer = new StreamWriter(Path.Combine(plot.WorkingDirectory.FullName, fileName)))
			{
				// Using a dummy plot function here.
				IPlotFunction function = PlotFunction.Create(x => 0.0);

				foreach (double t in Segment.CreatePoints(function))
				{
					ISpacePoint c = Curve.Value(t);
					writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", c.X, c.Y, c.Z));
				}
				writer.Close();
			}

			// Format specification here:
			// http://www.gnuplot.info/docs_4.2/gnuplot.html#x1-10100034

			return string.Format("'{0}' using 1:2:3 with lines {1} {2} {3}",
				fileName,
				Properties.Axes != null ? "axes " + Properties.Axes.Generate(plot) : "",
				Properties.Label != null ? "title '" + Properties.Label.Generate(plot) + "'" : "notitle",
				Properties.Generate(this, plot));
		}

		public ICurve Curve
		{
			get;
			private set;
		}

		public FunctionPlotSegment Segment
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
