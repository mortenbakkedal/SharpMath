// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class TeXPlotLabel : IPlotLabel
	{
		public TeXPlotLabel(string plainLabel, string texLabel, params ILabelStyle[] styles)
		{
			PlainLabel = plainLabel;
			TeXLabel = texLabel;
		}

		public string Generate(Gnuplot plot)
		{
			if (plot.CurrentTerminal is TikzPlotTerminal)
			{
				// Only use the TeX label with the Tikz terminal.
				return TeXLabel;
			}

			return PlainLabel;
		}

		public string PlainLabel
		{
			get;
			private set;
		}

		public string TeXLabel
		{
			get;
			private set;
		}

		string IPlotLabel.Label
		{
			get
			{
				return PlainLabel;
			}
		}
	}
}
