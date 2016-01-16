// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.Functions
{
	/*[Serializable]
	public abstract class PlotFunction<T> : IPlotFunction<T>
	{
		public abstract double Value(T x);

		public static PlotFunction<T> Create(IPlotFunction<T> f)
		{
			return Create(x => f.Value(x));
		}

		public static PlotFunction<T> Create(Func<T, double> f)
		{
			return new InnerFunction(f);
		}

		public static implicit operator PlotFunction<T>(double a)
		{
			return Create(x => a);
		}

		public static PlotFunction<T> operator +(PlotFunction<T> f, PlotFunction<T> g)
		{
			return Create(x => f.Value(x) + g.Value(x));
		}

		public static PlotFunction<T> operator +(PlotFunction<T> f, double a)
		{
			return Create(x => f.Value(x) + a);
		}

		public static PlotFunction<T> operator +(double a, PlotFunction<T> f)
		{
			return f + a;
		}

		public static PlotFunction<T> operator -(PlotFunction<T> f)
		{
			return Create(x => -f.Value(x));
		}

		public static PlotFunction<T> operator -(PlotFunction<T> f, PlotFunction<T> g)
		{
			return Create(x => f.Value(x) - g.Value(x));
		}

		public static PlotFunction<T> operator -(PlotFunction<T> f, double a)
		{
			return Create(x => f.Value(x) - a);
		}

		public static PlotFunction<T> operator -(double a, PlotFunction<T> f)
		{
			return Create(x => a - f.Value(x));
		}

		public static PlotFunction<T> operator *(PlotFunction<T> f, PlotFunction<T> g)
		{
			return Create(x => f.Value(x) * g.Value(x));
		}

		public static PlotFunction<T> operator *(PlotFunction<T> f, double a)
		{
			return Create(x => f.Value(x) * a);
		}

		public static PlotFunction<T> operator *(double a, PlotFunction<T> f)
		{
			return f * a;
		}

		public static PlotFunction<T> operator /(PlotFunction<T> f, PlotFunction<T> g)
		{
			return Create(x => f.Value(x) / g.Value(x));
		}

		public static PlotFunction<T> operator /(PlotFunction<T> f, double a)
		{
			return Create(x => f.Value(x) / a);
		}

		public static PlotFunction<T> operator /(double a, PlotFunction<T> f)
		{
			return Create(x => a / f.Value(x));
		}

		[Serializable]
		private class InnerFunction : PlotFunction<T>
		{
			private Func<T, double> f;

			public InnerFunction(Func<T, double> f)
			{
				this.f = f;
			}

			public override double Value(T x)
			{
				return f(x);
			}
		}
	}*/

	[Serializable]
	public abstract class PlotFunction : IPlotFunction
	{
		public abstract double Value(double x);

		public static PlotFunction Create(IPlotFunction f)
		{
			return Create(x => f.Value(x));
		}

		public static PlotFunction Create(Func<double, double> f)
		{
			return new InnerFunction(f);
		}

		public static implicit operator PlotFunction(double a)
		{
			return Create(x => a);
		}

		public static PlotFunction operator +(PlotFunction f, PlotFunction g)
		{
			return Create(x => f.Value(x) + g.Value(x));
		}

		public static PlotFunction operator +(PlotFunction f, double a)
		{
			return Create(x => f.Value(x) + a);
		}

		public static PlotFunction operator +(double a, PlotFunction f)
		{
			return f + a;
		}

		public static PlotFunction operator -(PlotFunction f)
		{
			return Create(x => -f.Value(x));
		}

		public static PlotFunction operator -(PlotFunction f, PlotFunction g)
		{
			return Create(x => f.Value(x) - g.Value(x));
		}

		public static PlotFunction operator -(PlotFunction f, double a)
		{
			return Create(x => f.Value(x) - a);
		}

		public static PlotFunction operator -(double a, PlotFunction f)
		{
			return Create(x => a - f.Value(x));
		}

		public static PlotFunction operator *(PlotFunction f, PlotFunction g)
		{
			return Create(x => f.Value(x) * g.Value(x));
		}

		public static PlotFunction operator *(PlotFunction f, double a)
		{
			return Create(x => f.Value(x) * a);
		}

		public static PlotFunction operator *(double a, PlotFunction f)
		{
			return f * a;
		}

		public static PlotFunction operator /(PlotFunction f, PlotFunction g)
		{
			return Create(x => f.Value(x) / g.Value(x));
		}

		public static PlotFunction operator /(PlotFunction f, double a)
		{
			return Create(x => f.Value(x) / a);
		}

		public static PlotFunction operator /(double a, PlotFunction f)
		{
			return Create(x => a / f.Value(x));
		}

		[Serializable]
		private class InnerFunction : PlotFunction
		{
			private Func<double, double> f;

			public InnerFunction(Func<double, double> f)
			{
				this.f = f;
			}

			public override double Value(double x)
			{
				return f(x);
			}
		}
	}
}
