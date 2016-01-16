// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Globalization;

namespace SharpMath.Plotting
{
	[Serializable]
	public class PointSize : IDataPlotStyle
	{
		static PointSize()
		{
			VerySmall = new PointSize(0.75);
			Small = new PointSize(1.0);
			Medium = new PointSize(1.5);
			Large = new PointSize(2.0);
		}

		public PointSize(double size)
		{
			if (size < 0.0)
			{
				throw new ArgumentOutOfRangeException();
			}

			Size = size;
		}

		public string Generate(Gnuplot plot)
		{
			return "pointsize " + Size.ToString(CultureInfo.InvariantCulture);
		}

		public double Size
		{
			get;
			private set;
		}

		public static PointSize VerySmall
		{
			get;
			private set;
		}

		public static PointSize Small
		{
			get;
			private set;
		}

		public static PointSize Medium
		{
			get;
			private set;
		}

		public static PointSize Large
		{
			get;
			private set;
		}
	}
}
