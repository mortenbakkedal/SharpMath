// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Text;

namespace SharpMath.Plotting
{
	[Serializable]
	public class PlotProperties
	{
		public PlotProperties(IEnumerable<IPlotStyle> styles)
		{
			foreach (IPlotStyle style in styles)
			{
				if (style is Axes)
				{
					Axes = (Axes)style;
				}
				else if (style is IPlotLabel)
				{
					Label = (IPlotLabel)style;
				}
				else if (style is LineType)
				{
					LineType = (LineType)style;
				}
				else if (style is LineWidth)
				{
					LineWidth = (LineWidth)style;
				}
				else if (style is LineColor)
				{
					LineColor = (LineColor)style;
				}
				else if (style is PointType)
				{
					PointType = (PointType)style;
				}
				else if (style is PointSize)
				{
					PointSize = (PointSize)style;
				}
				else
				{
					throw new NotSupportedException();
				}
			}
		}

		public string Generate(object sender, Gnuplot plot)
		{
			// Styles as defined here:
			// http://www.gnuplot.info/docs_4.2/gnuplot.html#x1-14700034.7

			StringBuilder sb = new StringBuilder();

			if (LineType != null)
			{
				sb.Append(LineType.Generate(plot));
				sb.Append(" ");
			}
			else if (sender is IFunctionPlot)
			{
				sb.Append(LineType.Solid.Generate(plot));
				sb.Append(" ");
			}

			if (LineWidth != null)
			{
				sb.Append(LineWidth.Generate(plot));
				sb.Append(" ");
			}
			else if (sender is IFunctionPlot && plot.CurrentTerminal is TikzPlotTerminal)
			{
				sb.Append(LineWidth.Thick.Generate(plot));
				sb.Append(" ");
			}

			if (LineColor != null)
			{
				sb.Append(LineColor.Generate(plot));
				sb.Append(" ");
			}

			if (PointType != null)
			{
				sb.Append(PointType.Generate(plot));
				sb.Append(" ");
			}

			if (PointSize != null)
			{
				sb.Append(PointSize.Generate(plot));
				sb.Append(" ");
			}
			else if (sender is IDataPlot && plot.CurrentTerminal is TikzPlotTerminal)
			{
				sb.Append(PointSize.Medium.Generate(plot));
				sb.Append(" ");
			}

			return sb.ToString();
		}

		public Axes Axes
		{
			get;
			private set;
		}

		public IPlotLabel Label
		{
			get;
			private set;
		}

		public LineType LineType
		{
			get;
			private set;
		}
		
		public LineWidth LineWidth
		{
			get;
			private set;
		}

		public LineColor LineColor
		{
			get;
			private set;
		}

		public PointType PointType
		{
			get;
			private set;
		}

		public PointSize PointSize
		{
			get;
			private set;
		}
	}
}
