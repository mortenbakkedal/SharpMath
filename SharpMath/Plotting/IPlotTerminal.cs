// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting
{
	public interface IPlotTerminal
	{
		void Generate(Gnuplot plot);

		void FinalizeTerminal();
	}
}
