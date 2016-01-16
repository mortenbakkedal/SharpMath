// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class PlotLabel : IPlotLabel
	{
		public PlotLabel(string label, params ILabelStyle[] styles)
		{
			Label = label;
		}

		public string Generate(Gnuplot plot)
		{
			if (plot.CurrentTerminal is TikzPlotTerminal)
			{
				// Probably incomplete list.
				return Label.Replace(@"\", @"\\").Replace("_", @"\_").Replace("{", @"\{").Replace("}", @"\}").Replace("$", @"\$");
			}

			return Label;
		}

		public string Label
		{
			get;
			private set;
		}
	}
}
