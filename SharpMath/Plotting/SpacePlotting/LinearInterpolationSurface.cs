// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using SharpMath.Collections;
using SharpMath.Plotting.Functions;

namespace SharpMath.Plotting.SpacePlotting
{
	[Serializable]
	public class LinearInterpolationSurface : ISurface
	{
		public LinearInterpolationSurface(LinearInterpolationSurfaceProjection projection)
			: this(new double[0], new IPlotFunction[0], projection)
		{
		}

		public LinearInterpolationSurface(IEnumerable<double> values, IEnumerable<IPlotFunction> functions, LinearInterpolationSurfaceProjection projection)
			: this(new List<double>(values).ToArray(), new List<IPlotFunction>(functions).ToArray(), projection)
		{
		}

		private LinearInterpolationSurface(double[] values, IPlotFunction[] functions, LinearInterpolationSurfaceProjection projection)
		{
			int n = values.Length;

			if (functions.Length != n)
			{
				throw new ArgumentException();
			}

			List<Tuple<double, IPlotFunction>> pairs = new List<Tuple<double, IPlotFunction>>();
			for (int i = 0; i < n; i++)
			{
				pairs.Add(Tuple.Create<double, IPlotFunction>(values[i], functions[i]));
			}
			pairs.Sort((p1, p2) => p1.Item1.CompareTo(p2.Item1));

			for (int i = 0; i < n; i++)
			{
				values[i] = pairs[i].Item1;
				functions[i] = pairs[i].Item2;
			}

			Count = n;
			Values = new ImmutableList<double>(values);
			Functions = new ImmutableList<IPlotFunction>(functions);
			Projection = projection;
		}

		public double Value(double x, double y)
		{
			switch (Projection)
			{
				case LinearInterpolationSurfaceProjection.XZPlane:
					return ValueXZ(x, y);

				case LinearInterpolationSurfaceProjection.YZPlane:
					return ValueYZ(x, y);

				default:
					throw new NotSupportedException();
			}
		}

		private double ValueXZ(double x, double y)
		{
			if (Count == 0)
			{
				return double.NaN;
			}

			if (x <= Values[0])
			{
				return Functions[0].Value(y);
			}

			for (int i = 1; i < Count; i++)
			{
				if (x <= Values[i])
				{
					double lambda = (Values[i] - x) / (Values[i] - Values[i - 1]);
					return (1.0 - lambda) * Functions[i].Value(y) + lambda * Functions[i - 1].Value(y);
				}
			}

			return Functions[Count - 1].Value(y);
		}

		private double ValueYZ(double x, double y)
		{
			return ValueXZ(y, x);
		}

		public LinearInterpolationSurface Add(double value, IPlotFunction function)
		{
			return new LinearInterpolationSurface(Values.Concat(new double[] { value }), Functions.Concat(new IPlotFunction[] { function }), Projection);
		}

		public int Count
		{
			get;
			private set;
		}
			
		public ImmutableList<double> Values
		{
			get;
			private set;
		}

		public ImmutableList<IPlotFunction> Functions
		{
			get;
			private set;
		}

		public LinearInterpolationSurfaceProjection Projection
		{
			get;
			private set;
		}
	}
}
