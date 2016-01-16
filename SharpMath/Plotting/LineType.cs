// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class LineType : IFunctionPlotStyle
	{
		static LineType()
		{
			//http://www2.yukawa.kyoto-u.ac.jp/~akira.ohnishi/Lib/gnuplot.html
			Solid = new LineType(1);
			Dashed = new LineType(2);
			Dotted = new LineType(4);
		}

		public LineType(int index)
		{
			Index = index;
		}

		public string Generate(Gnuplot plot)
		{
			return "linetype " + Index.ToString();
		}

		public int Index
		{
			get;
			private set;
		}

		public static LineType Solid
		{
			get;
			private set;
		}

		public static LineType Dashed
		{
			get;
			private set;
		}

		public static LineType Dotted
		{
			get;
			private set;
		}
	}
}
