// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SharpMath.Plotting
{
	[Serializable]
	public class TikzPlotTerminal : IPlotTerminal
	{
		public TikzPlotTerminal(FileInfo file)
			: this(file, "color solid")
		{
			// Color plot with solid lines by default.
		}

		public TikzPlotTerminal(FileInfo file, double width, double height)
			: this(file, string.Format(CultureInfo.InvariantCulture, "color solid size {0:f4}cm,{1:f4}cm", width, height))
		{
			if (width <= 0.0 || height <= 0.0)
			{
				throw new ArgumentOutOfRangeException();
			}

			Width = width;
			Height = height;
		}

		public TikzPlotTerminal(FileInfo file, string options)
		{
			File = file;
			TempFile = new TempFile();//new FileInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName() + ".tex"));
			Options = options;
		}

		public void Generate(Gnuplot plot)
		{
			if (Options != null)
			{
				plot.Add("set terminal tikz " + Options);
			}
			else
			{
				plot.Add("set terminal tikz");
			}
			plot.Add(string.Format("set output '{0}'", TempFile.File.FullName));

			// Allow dashed lines, but remember to keep default solid lines.
			plot.Add("set termoption dashed");

			//plot.Add("set object rectangle from screen 0,0 to screen 1,1 lw 0 fillstyle noborder behind");

			// Prefer a slightly thicker line width for this terminal.
			// http://www.gnuplot.info/docs_4.2/gnuplot.html#x1-16400043.6
			plot.Add("set border linewidth " + LineWidth.Thick.Width.ToString(CultureInfo.InvariantCulture));
		}

		public void FinalizeTerminal()
		{
			// Remove timestamp to make output exactly reproducible. Try to be restrictive in removing it. The latter index is for multiplots.
			System.IO.File.WriteAllLines(File.FullName, System.IO.File.ReadLines(TempFile.File.FullName).Where((s, i) => !(s.StartsWith("%% ") && (i == 2 || i == 10) && s.Length <= 32)));
		}
		
		public FileInfo File
		{
			get;
			private set;
		}

		public TempFile TempFile
		{
			get;
			private set;
		}

		public double Width
		{
			get;
			private set;
		}

		public double Height
		{
			get;
			private set;
		}

		public string Options
		{
			get;
			private set;
		}
	}
}
