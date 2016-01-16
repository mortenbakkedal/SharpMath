// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.IO;

namespace SharpMath.Plotting
{
	[Serializable]
	public class PngTerminal : IPlotTerminal
	{
		public PngTerminal(FileInfo file, int width, int height)
		{
			File = file;
			Width = width;
			Height = height;
		}

		public void Generate(Gnuplot plot)
		{
			plot.Add(string.Format("set terminal pngcairo size {0},{1}", Width, Height));
			plot.Add(string.Format("set output '{0}'", File.FullName));
		}

		public void FinalizeTerminal()
		{
		}
		
		public FileInfo File
		{
			get;
			private set;
		}

		public int Width
		{
			get;
			private set;
		}

		public int Height
		{
			get;
			private set;
		}
	}
}
