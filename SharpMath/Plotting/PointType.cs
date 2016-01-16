// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	[Serializable]
	public class PointType : IDataPlotStyle
	{
		static PointType()
		{
			// Point types are shown here:
			// http://stelweb.asu.cas.cz/~nemeth/work/stuff/gnuplot/gnuplot-line-and-point-types-bw.png

			Plus = new PointType(1);
			Cross = new PointType(2);
			Star = new PointType(3);
			Square = new PointType(4);
			FilledSquare = new PointType(5);
			Circle = new PointType(6);
			FilledCircle = new PointType(7);
			Triangle = new PointType(8);
			FilledTriangle = new PointType(9);
			TriangleDown = new PointType(10);
			FilledTriangleDown = new PointType(11);
			Diamond = new PointType(12);
			FilledDiamond = new PointType(13);
		}

		public PointType(int index)
		{
			if (index < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			Index = index;
		}

		public string Generate(Gnuplot plot)
		{
			return "pointtype " + Index.ToString();
		}

		public int Index
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $+$.
		/// </summary>
		public static PointType Plus
		{
			get;
			private set;
		}

		/// <summary>
		/// No LaTeX code.
		/// </summary>
		public static PointType Cross
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\star$.
		/// </summary>
		public static PointType Star
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\square$.
		/// </summary>
		public static PointType Square
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\blacksquare$.
		/// </summary>
		public static PointType FilledSquare
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\circ$.
		/// </summary>
		public static PointType Circle
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\bullet$.
		/// </summary>
		public static PointType FilledCircle
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\vartriangle$.
		/// </summary>
		public static PointType Triangle
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\blacktriangle$.
		/// </summary>
		public static PointType FilledTriangle
		{
			get;
			private set;
		}

		/// <summary>
		/// LaTeX code $\triangledown$.
		/// </summary>
		public static PointType TriangleDown
		{
			get;
			private set;
		}
		
		/// <summary>
		/// LaTeX code $\blacktriangledown$.
		/// </summary>
		public static PointType FilledTriangleDown
		{
			get;
			private set;
		}
		
		/// <summary>
		/// LaTeX code $\lozenge$.
		/// </summary>
		public static PointType Diamond
		{
			get;
			private set;
		}
		
		/// <summary>
		/// LaTeX code $\blacklozenge$.
		/// </summary>
		public static PointType FilledDiamond
		{
			get;
			private set;
		}
	}
}
