// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Globalization;

namespace SharpMath.Plotting
{
	public class LineWidth : IFunctionPlotStyle, IDataPlotStyle
	{
		static LineWidth()
		{
			Thin = new LineWidth(1.0);
			Thick = new LineWidth(1.5);
			VeryThick = new LineWidth(2.0);
		}

		public LineWidth(double width)
		{
			if (width < 0.0)
			{
				throw new ArgumentOutOfRangeException();
			}

			Width = width;
		}

		public string Generate(Gnuplot plot)
		{
			return "linewidth " + Width.ToString(CultureInfo.InvariantCulture);
		}

		public double Width
		{
			get;
			private set;
		}

		public static LineWidth Thin
		{
			get;
			private set;
		}
		
		public static LineWidth Thick
		{
			get;
			private set;
		}

		public static LineWidth VeryThick
		{
			get;
			private set;
		}
	}
}
