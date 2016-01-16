// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class LineColor : IFunctionPlotStyle, IDataPlotStyle, ILabelStyle
	{
		static LineColor()
		{
			// http://cpansearch.perl.org/src/KWMAK/Chart-Gnuplot-0.14/doc/colors.txt
			Red = new LineColor("red");
			Green = new LineColor("green");
			Blue = new LineColor("blue");
			LightBlue = new LineColor("light-blue");
			Magenta = new LineColor("magenta");
			Yellow = new LineColor("yellow");
			Black = new LineColor("black");
			Orange = new LineColor("orange");
			Gray = new LineColor("gray");
			Brown = new LineColor("brown");
			Violet = new LineColor("violet");
			Pink = new LineColor("pink");
			LightPink = new LineColor("light-pink");

			// Printer-friendly color from 2010-shang-wang-kim-liu.
			OrangeBrown = new LineColor("#b83e24");
		}

		public LineColor(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			Index = index;
		}

		public LineColor(string color)
		{
			if (color == null)
			{
				throw new ArgumentNullException();
			}

			Color = color;
		}

		public string Generate(Gnuplot plot)
		{
			if (Color != null)
			{
				return "linecolor rgb '" + Color + "'";
			}
			else
			{
				return "linecolor " + Index.ToString();
			}
		}

		public int? Index
		{
			get;
			private set;
		}

		public string Color
		{
			get;
			private set;
		}

		public static LineColor Red
		{
			get;
			private set;
		}

		public static LineColor Green
		{
			get;
			private set;
		}

		public static LineColor Blue
		{
			get;
			private set;
		}
		
		public static LineColor LightBlue
		{
			get;
			private set;
		}

		public static LineColor Magenta
		{
			get;
			private set;
		}

		public static LineColor Yellow
		{
			get;
			private set;
		}

		public static LineColor Black
		{
			get;
			private set;
		}

		public static LineColor Orange
		{
			get;
			private set;
		}

		public static LineColor Gray
		{
			get;
			private set;
		}

		public static LineColor Brown
		{
			get;
			private set;
		}

		public static LineColor Violet
		{
			get;
			private set;
		}

		public static LineColor Pink
		{
			get;
			private set;
		}

		public static LineColor LightPink
		{
			get;
			private set;
		}

		public static LineColor OrangeBrown
		{
			get;
			private set;
		}
	}
}
