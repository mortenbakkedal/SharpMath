// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using SharpMath.Plotting.Functions;
using SharpMath.Plotting.SpacePlotting;

namespace SharpMath.Plotting
{
	/// <summary>
	/// An object-oriented abstraction on top of the versatile gnuplot command-line tool. The implementation tries to resemble the gnuplot command flow as closely as possible.
	/// </summary>
	public class Gnuplot : IDisposable
	{
		private TempDirectory directory;
		private List<string> expressions;

		/*public static void Test()
		{
			new Gnuplot()
				//.SetTerminal(new X11PlotTerminal())
				.SetXLabel("y_N")
				.SetXRange(0.0, 2.0 * Math.PI)
				.SetYLabel("Energy per atom")
				.SetYSecondaryLabel("Entropy per atom")
				//.SetYRange(-1.5, 1.0)
				// FunctionPlotSegment, also for FunctionPlot, use for T=0 plot showing step functions
				//.UnsetKey()
				.Plot(
					new DataPlot(new IPlotPoint[]{new PlaneVector(1, 0.5), new PlaneVector(1, 0.7)}, LineColor.Black, PointType.Cross, LineWidth.Thick, new PointSize(1), new PlotLabel("g")),
					new FunctionPlot(x => Math.Sin(3*x), 0.0, 2.0 * Math.PI, Axes.XYSecondary, LineColor.Blue, LineWidth.Thick, LineType.Dashed, new PlotLabel("f")))
				.Pause()
				.Run();
		}*/

		public Gnuplot()
		{
			expressions = new List<string>();
			directory = new TempDirectory();
		}

		~Gnuplot()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// Following this dispose pattern: http://stackoverflow.com/a/898867

			// Explicitly delete temporary directory here.
			directory.Dispose();
		}

		/// <summary>
		/// Adds the specified unparsed expression directly to gnuplot.
		/// </summary>
		public Gnuplot Add(string expression)
		{
			expressions.Add(expression);

			return this;
		}

		public Gnuplot SetMargin(double leftMargin, double rightMargin, double bottomMargin, double topMargin)
		{
			Add(string.Format(CultureInfo.InvariantCulture, "set lmargin at screen {0}", leftMargin));
			Add(string.Format(CultureInfo.InvariantCulture, "set rmargin at screen {0}", rightMargin));
			Add(string.Format(CultureInfo.InvariantCulture, "set bmargin at screen {0}", bottomMargin));
			Add(string.Format(CultureInfo.InvariantCulture, "set tmargin at screen {0}", topMargin));

			return this;
		}

		public Gnuplot SetMultiPlot()
		{
			Add("set multiplot");

			return this;
		}

		/// <summary>
		/// For convience the specified terminal is stored in the Terminal property.
		/// </summary>
		public Gnuplot SetTerminal(IPlotTerminal terminal)
		{
			CurrentTerminal = terminal;
			terminal.Generate(this);

			return this;
		}
		
		public Gnuplot SetTitle(string label)
		{
			return SetTitle(new PlotLabel(label));
		}

		public Gnuplot SetTitle(IPlotLabel label)
		{
			Add(string.Format("set title '{0}'", label.Generate(this)));

			return this;
		}

		public Gnuplot SetXLabel(string label)
		{
			return SetXLabel(new PlotLabel(label));
		}

		public Gnuplot SetXLabel(IPlotLabel label)
		{
			Add(string.Format("set xlabel '{0}'", label.Generate(this)));

			return this;
		}

		public Gnuplot SetXRange(double a, double b)
		{
			Add(string.Format(CultureInfo.InvariantCulture, "set xrange [{0}:{1}]", a, b));

			return this;
		}

		public Gnuplot SetXSecondaryRange(double a, double b)
		{
			Add(string.Format(CultureInfo.InvariantCulture, "set x2range [{0}:{1}]", a, b));

			return this;
		}

		public Gnuplot SetYLabel(string label)
		{
			return SetYLabel(new PlotLabel(label));
		}

		public Gnuplot SetYLabel(IPlotLabel label)
		{
			Add(string.Format("set ylabel '{0}'", label.Generate(this)));

			return this;
		}

		public Gnuplot SetYRange(double a, double b)
		{
			Add(string.Format(CultureInfo.InvariantCulture, "set yrange [{0}:{1}]", a, b));

			return this;
		}

		public Gnuplot SetYSecondaryRange(double a, double b)
		{
			Add(string.Format(CultureInfo.InvariantCulture, "set y2range [{0}:{1}]", a, b));
			PrepareSecondaryAxis();

			return this;
		}

		public Gnuplot SetYSecondaryLabel(string label)
		{
			return SetYSecondaryLabel(new PlotLabel(label));
		}

		public Gnuplot SetYSecondaryLabel(IPlotLabel label)
		{
			Add(string.Format("set y2label '{0}'", label.Generate(this)));
			PrepareSecondaryAxis();

			return this;
		}

		public Gnuplot SetZLabel(string label)
		{
			return SetZLabel(new PlotLabel(label));
		}

		public Gnuplot SetZLabel(IPlotLabel label)
		{
			Add(string.Format("set zlabel '{0}'", label.Generate(this)));

			return this;
		}

		public Gnuplot SetZRange(double a, double b)
		{
			Add(string.Format(CultureInfo.InvariantCulture, "set zrange [{0}:{1}]", a, b));

			return this;
		}

		public Gnuplot UnsetKey()
		{
			Add("unset key");
			return this;
		}
		
		public Gnuplot UnsetMultiPlot()
		{
			Add("unset multiplot");

			return this;
		}

		/// <summary>
		/// Corresponds to the plot command. Multiple different plots may be added.
		/// </summary>
		public Gnuplot Plot(IEnumerable<IPlot> plots)
		{
			Add("plot " + string.Join(", ", new List<IPlot>(plots).ConvertAll<string>(p => p.Generate(this))));

			return this;
		}

		/// <summary>
		/// Corresponds to the plot command. Multiple different plots may be added.
		/// </summary>
		public Gnuplot Plot(params IPlot[] plots)
		{
			return Plot((IEnumerable<IPlot>)plots);
		}

		/// <summary>
		/// Corresponds to the plot command. Multiple different plots may be added.
		/// </summary>
		public Gnuplot Plot(params IEnumerable<IPlot>[] plots)
		{
			return Plot(plots.SelectMany(p => p));
		}

		public static void Plot(string xLabel, string yLabel, params IPlot[] plots)
		{
			new Gnuplot().SetXLabel(xLabel).SetYLabel(yLabel).Plot(plots).Pause().Run();
		}

		public static void Plot(string xLabel, string yLabel, string ySecondaryLabel, params IPlot[] plots)
		{
			new Gnuplot().SetXLabel(xLabel).SetYLabel(yLabel).SetYSecondaryLabel(ySecondaryLabel).Plot(plots).Pause().Run();
		}

		/*public static void Plot(IRangedPlotFunction function)
		{
			double a = function.XMin;
			double b = function.XMax;
			new Gnuplot().SetXRange(a, b).Plot(new FunctionPlot(function, a, b)).Pause().Run();
		}*/

		/*public static void Plot(double x1, double x2, double y1, double y2, params IPlot[] plots)
		{
		}*/

		public Gnuplot SurfacePlot(IEnumerable<ISpacePlot> plots)
		{
			Add("splot " + string.Join(", ", new List<ISpacePlot>(plots).ConvertAll<string>(p => p.Generate(this))));

			return this;
		}

		public Gnuplot SurfacePlot(params ISpacePlot[] plots)
		{
			return SurfacePlot((IEnumerable<ISpacePlot>)plots);
		}

		public Gnuplot SurfacePlot(params IEnumerable<ISpacePlot>[] plots)
		{
			return SurfacePlot(plots.SelectMany(p => p));
		}

		public static void SurfacePlot(string xLabel, string yLabel, string zLabel, params ISpacePlot[] plots)
		{
			new Gnuplot().SetXLabel(xLabel).SetYLabel(yLabel).SetZLabel(zLabel).SurfacePlot(plots).Pause().Run();
		}

		public Gnuplot Pause()
		{
			// Ignore on non-X11 terminals.
			if (CurrentTerminal == null || CurrentTerminal is X11PlotTerminal)
			{
				//Add("pause -1 'Press return to continue...'");

				// http://gnuplot.info/announce.4.2.4
				// NEW "pause mouse close" waits until the plot window is closed
				Add("pause mouse close");
			}

			return this;
		}

		public void Run()
		{
			using (StreamWriter writer = new StreamWriter(Path.Combine(WorkingDirectory.FullName, "plot.plt")))
			{
				foreach (string expression in expressions)
				{
					writer.WriteLine(expression);
				}
				writer.Close();
			}

			using (Process p = new Process())
			{
				p.StartInfo.FileName = "gnuplot";
				p.StartInfo.WorkingDirectory = WorkingDirectory.FullName;
				p.StartInfo.Arguments = "plot.plt";

				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;

				p.Start();
				p.WaitForExit();

				if (p.ExitCode != 0)
				{
					throw new Exception(string.Format("Running gnuplot failed: {0}", p.StandardError.ReadToEnd()));
				}
			}

			if (CurrentTerminal != null)
			{
				CurrentTerminal.FinalizeTerminal();
			}
		}

		private void PrepareSecondaryAxis()
		{
			// This seems to be useful.
			Add("set ytics nomirror");
			Add("set y2tics");
		}

		public IPlotTerminal CurrentTerminal
		{
			get;
			private set;
		}

		public DirectoryInfo WorkingDirectory
		{
			get
			{
				return directory.Directory;
			}
		}
	}
}
