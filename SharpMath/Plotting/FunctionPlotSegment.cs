// SharpMath - C# Math Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using SharpMath.Plotting.Functions;

namespace SharpMath.Plotting
{
	[Serializable]
	public class FunctionPlotSegment
	{
		private IPointGenerator generator;

		public FunctionPlotSegment(double a, double b, int n)
			: this(CreatePoints(a, b, n))
		{
		}

		public FunctionPlotSegment(double a, double b, int n, double tolerance)
		{
			if (a >= b || n < 2 || tolerance < 0.0)
			{
				throw new ArgumentOutOfRangeException();
			}

			generator = new DynamicPointGenerator(a, b, n, tolerance);
		}

		public FunctionPlotSegment(IEnumerable<double> points)
		{
			if (points == null)
			{
				throw new ArgumentNullException();
			}

			points = CreatePoints(points);

			/*int n = 0;
			double x0 = double.NegativeInfinity;
			foreach (double x in points)
			{
				if (x < x0)
				{
					throw new ArgumentException("Points must be in ascending order.");
				}
				x0 = x;
				n++;
			}*/

			if (points.Count() < 2)
			{
				throw new ArgumentException("Need at least two points.");
			}

			generator = new PointGenerator(points);
		}

		public IEnumerable<double> CreatePoints(IPlotFunction function)
		{
			return generator.CreatePoints(function);
		}

		public static IEnumerable<double> CreatePoints(double a, double b, int n)
		{
			if (a > b || n < 2)
			{
				throw new ArgumentOutOfRangeException();
			}

			for (int i = 0; i < n - 1; i++)
			{
				yield return a + i * (b - a) / (n - 1);
			}

			// Reaching end-points exactly with floating-point arithmetic (overshooting could be bad).
			yield return b;
		}

		public static IEnumerable<double> CreatePoints(IEnumerable<double> points)
		{
			return points.Distinct().OrderBy(p => p).ToArray();
		}

		public static IEnumerable<double> CreatePoints(double a, double b, IEnumerable<double> points)
		{
			// A selection of points with end-points added.
			return CreatePoints(points.Where(p => p > a && p < b).Concat(new double[] { a, b }));
		}

		public static IEnumerable<double> CreatePoints(double a, double b, ILinearInterpolationFunction function)
		{
			return CreatePoints(a, b, function.Points.Select(p => p.X));
		}

		private interface IPointGenerator
		{
			IEnumerable<double> CreatePoints(IPlotFunction function);
		}

		[Serializable]
		private class PointGenerator : IPointGenerator
		{
			private List<double> points;

			public PointGenerator(IEnumerable<double> points)
			{
				this.points = new List<double>(points);
			}

			public IEnumerable<double> CreatePoints(IPlotFunction function)
			{
				return points;
			}
		}

		[Serializable]
		private class DynamicPointGenerator : IPointGenerator
		{
			private double a, b, tolerance;
			private int n;

			public DynamicPointGenerator(double a, double b, int n, double tolerance)
			{
				this.a = a;
				this.b = b;
				this.n = n;
				this.tolerance = tolerance;
			}

			public IEnumerable<double> CreatePoints(IPlotFunction function)
			{
				if (function == null)
				{
					throw new ArgumentNullException();
				}

				List<double> points = new List<double>(FunctionPlotSegment.CreatePoints(a, b, n));
				List<double> values = new List<double>();
				foreach (double x in points)
				{
					values.Add(function.Value(x));
				}

				List<double> points2 = new List<double>();

				// Always keep the left-most point.
				points2.Add(a);
				//yield return a;

				int i0 = 0;
				while (i0 < n - 1)
				{
					// Find the largest span of points such that deviations in the y-direction between the linear interpolation
					// and the true function values of intermediate points are no more than the tolerance.

					// Currently using this left end-point.
					double xl = points[i0];
					double yl = values[i0];

					// Perform at least one step.
					int i = i0 + 1;
					while (i < n - 1)
					{
						// Test with this right end-point one more step ahead.
						double xr = points[i + 1];
						double yr = values[i + 1];

						// And then recheck all points in the range between these points.
						bool stop = false;
						for (int j = i0 + 1; j < i + 1; j++)
						{
							double x = points[j];

							// The true value.
							double y0 = values[j];

							// The interpolated value.
							double lambda = (xr - x) / (xr - xl);
							double y = (1.0 - lambda) * yr + lambda * yl;

							if (Math.Abs(y - y0) > tolerance || double.IsNaN(y) || double.IsNaN(y0))
							{
								// Deviation too large. Stop the expansion of the range.
								stop = true;
								break;
							}
						}

						if (stop)
						{
							break;
						}

						// We've confirmed that the range can be expanded by one more point.
						i++;
					}

					points2.Add(points[i]);
					//yield return points[i];

					// Use this point as the next left end-point.
					i0 = i;
				}

				return points2;
			}
		}
	}
}
