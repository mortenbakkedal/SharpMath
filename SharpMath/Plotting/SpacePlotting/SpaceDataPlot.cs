// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using SharpMath.Collections;

namespace SharpMath.Plotting.SpacePlotting
{
	[Serializable]
	public class SpaceDataPlot : ISpacePlot
	{
		public SpaceDataPlot(params ISpacePoint[] points)
			: this((IEnumerable<ISpacePoint>)points)
		{
		}

		public SpaceDataPlot(IEnumerable<ISpacePoint> points, params IDataPlotStyle[] styles)
		{
			Points = new ImmutableList<ISpacePoint>(points);
			Properties = new PlotProperties(styles);
		}

		public string Generate(Gnuplot plot)
		{
			string fileName = Path.GetRandomFileName();
			using (StreamWriter writer = new StreamWriter(Path.Combine(plot.WorkingDirectory.FullName, fileName)))
			{
				foreach (ISpacePoint point in Points)
				{
					writer.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} {1} {2}", point.X, point.Y, point.Z));
				}
				writer.Close();
			}

			// Format specification here:
			// http://www.gnuplot.info/docs_4.2/gnuplot.html#x1-10100034

			return string.Format("'{0}' using 1:2:3 {1} {2} {3}",
				fileName,
				Properties.Axes != null ? "axes " + Properties.Axes.Generate(plot) : "",
				Properties.Label != null ? "title '" + Properties.Label.Generate(plot) + "'" : "notitle",
				Properties.Generate(this, plot));
		}

		public ImmutableList<ISpacePoint> Points
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
