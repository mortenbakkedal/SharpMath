// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.SpacePlotting
{
	[Serializable]
	public abstract class Surface : ISurface
	{
		public abstract double Value(double x, double y);

		public static Surface Create(Func<double, double, double> f)
		{
			return new InnerSurface(f);
		}

		[Serializable]
		private class InnerSurface : Surface
		{
			private Func<double, double, double> f;

			public InnerSurface(Func<double, double, double> f)
			{
				this.f = f;
			}

			public override double Value(double x, double y)
			{
				return f(x, y);
			}
		}
	}
}
