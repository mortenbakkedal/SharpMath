// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class Axes : IFunctionPlotStyle, IDataPlotStyle
	{
		private Axes(string identifier)
		{
			// http://www.gnuplot.info/docs_4.2/gnuplot.html#x1-10100034
			Identifier = identifier;
		}

		public string Generate(Gnuplot plot)
		{
			return Identifier;
		}

		public string Identifier
		{
			get;
			private set;
		}

		public static Axes XY
		{
			get
			{
				return new Axes("x1y1");
			}
		}

		public static Axes XYSecondary
		{
			get
			{
				return new Axes("x1y2");
			}
		}
	}
}
