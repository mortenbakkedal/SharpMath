// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.Optimization.DualNumbers
{
	/// <summary>
	/// This class represents a function value and its first and second partial derivatives at a given point. Several standard
	/// mathematical operations are defined. The class is immutable.
	/// </summary>
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{Value}")]
	public sealed class DualNumber
	{
		private static DualNumber zero;
		private double value;
		private int n;
		private double[] gradientArray, hessianArray;
		[NonSerialized]
		private Vector gradient;
		[NonSerialized]
		private Matrix hessian;

		static DualNumber()
		{
			// Used very often (e.g. in matrix initialization).
			zero = new DualNumber(0.0);
		}

		/// <summary>
		/// A new <see cref="DualNumber" /> with the specified value, and zero first and second partial derivatives. Use the
		/// implicit conversion operator from <see cref="double" /> instead of using this constructor directly.
		/// </summary>
		public DualNumber(double value)
			: this(value, null)
		{
		}

		/// <summary>
		/// A new <see cref="DualNumber" /> with the specified value and first partial derivatives, and zero second partial derivatives.
		/// </summary>
		public DualNumber(double value, Vector gradient)
			: this(value, gradient, null)
		{
		}

		/// <summary>
		/// A new <see cref="DualNumber" /> with the specified value and first and second partial derivatives.
		/// </summary>
		public DualNumber(double value, Vector gradient, Matrix hessian)
		{
			this.value = value;
			this.gradient = gradient;
			this.hessian = hessian;

			if (gradient != null)
			{
				n = gradient.Length;
				gradientArray = gradient.ToArray();
			}

			if (hessian != null)
			{
				if (gradient == null)
				{
					throw new ArgumentException("The gradient must be specified if the Hessian is specified.");
				}

				if (hessian.Rows != n || hessian.Columns != n)
				{
					throw new ArgumentException("Inconsistent number of derivatives.");
				}

				// Since the Hessian is symmetric we only need to store the upper triangular part of it. Use a
				// one dimensional array until the matrix is requested (if ever). Doing it this way is almost
				// a factor of 10 faster than using naive matrix operations.

				hessianArray = new double[HessianSize(n)];

				for (int i = 0, k = 0; i < n; i++)
				{
					for (int j = i; j < n; j++, k++)
					{
						if (hessian[i, j] != hessian[j, i] && !(double.IsNaN(hessian[i, j]) && double.IsNaN(hessian[j, i])) && !(double.IsPositiveInfinity(hessian[i, j]) && double.IsPositiveInfinity(hessian[j, i])) && !(double.IsNegativeInfinity(hessian[i, j]) && double.IsNegativeInfinity(hessian[j, i])))
						{
							throw new ArgumentException("The Hessian must be symmetric.");
						}

						hessianArray[k] = hessian[i, j];
					}
				}
			}
		}

		/// <summary>
		/// A new <see cref="DualNumber" /> with the specified value and first and second partial derivatives using
		/// the compact internal representation of the Hessian.
		/// </summary>
		public DualNumber(double value, double[] gradientArray, double[] hessianArray)
		{
			this.value = value;

			if (gradientArray != null)
			{
				n = gradientArray.Length;
				this.gradientArray = (double[])gradientArray.Clone();
			}

			if (hessianArray != null)
			{
				if (gradientArray == null)
				{
					throw new ArgumentException("The gradient must be specified if the Hessian is specified.");
				}

				if (hessianArray.Length != HessianSize(n))
				{
					throw new ArgumentException("Inconsistent number of derivatives.");
				}

				this.hessianArray = (double[])hessianArray.Clone();
			}
		}

		/// <summary>
		/// Perform the unary operation h(x)=g(f(x)). Using the chain rule we're able to compute
		/// h(x), h'(x), and h''(x). The value and the first and second derivative of the outer
		/// function (the unary operation) must be specified.
		/// </summary>
		/// <param name="f">The inner function f.</param>
		/// <param name="g">g(f(x)).</param>
		/// <param name="g1">g'(f(x)).</param>
		/// <param name="g11">g''(f(x)).</param>
		public DualNumber(DualNumber f, double g, double g1, double g11)
		{
			value = g;

			if (f.gradientArray != null)
			{
				n = f.n;
				gradientArray = new double[n];

				if (g1 != 0.0)
				{
					for (int i = 0; i < n; i++)
					{
						gradientArray[i] += g1 * f.gradientArray[i];
					}
				}

				if (f.hessianArray != null || g11 != 0.0)
				{
					hessianArray = new double[HessianSize(n)];

					if (g1 != 0.0 && f.hessianArray != null)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g1 * f.hessianArray[k];
							}
						}
					}

					if (g11 != 0.0)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g11 * f.gradientArray[i] * f.gradientArray[j];
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Perform the binary operation h(x)=g(f_1(x),f_2(x)). Using the chain rule we're able to compute
		/// h(x), h'(x), and h''(x). The value and the first and second partial derivatives of the outer
		/// function (the binary operation) must be specified.
		/// </summary>
		/// <param name="f1">The first inner function f_1 (left of the binary operator).</param>
		/// <param name="f2">The second inner function f_2 (right of the binary operator).</param>
		/// <param name="g">g(f_1(x),f_2(x)).</param>
		/// <param name="g1">The partial derivative $\frac{\partial g}{\partial x_1}(f_1(x),f_2(x))$.</param>
		/// <param name="g2">The partial derivative $\frac{\partial g}{\partial x_2}(f_1(x),f_2(x))$.</param>
		/// <param name="g11">The partial derivative $\frac{\partial^2g}{\partial x_1^2}(f_1(x),f_2(x))$.</param>
		/// <param name="g12">The partial derivative $\frac{\partial^2g}{\partial x_1\partial x_2}(f_1(x),f_2(x))$.</param>
		/// <param name="g22">The partial derivative $\frac{\partial^2g}{\partial x_2^2}(f_1(x),f_2(x))$.</param>
		public DualNumber(DualNumber f1, DualNumber f2, double g, double g1, double g2, double g11, double g12, double g22)
		{
			value = g;

			if (f1.gradientArray != null || f2.gradientArray != null)
			{
				if (f1.gradientArray != null && f2.gradientArray != null && f1.n != f2.n)
				{
					throw new ArgumentException("Inconsistent number of derivatives.");
				}

				// One of the counters may be zero if the corresponding DualNumber is a constant.
				n = Math.Max(f1.n, f2.n);
				gradientArray = new double[n];

				if (g1 != 0.0 && f1.gradientArray != null)
				{
					for (int i = 0; i < n; i++)
					{
						gradientArray[i] += g1 * f1.gradientArray[i];
					}
				}

				if (g2 != 0.0 && f2.gradientArray != null)
				{
					for (int i = 0; i < n; i++)
					{
						gradientArray[i] += g2 * f2.gradientArray[i];
					}
				}

				if (f1.hessianArray != null || f2.hessianArray != null || g11 != 0.0 || g12 != 0.0 || g22 != 0.0)
				{
					hessianArray = new double[HessianSize(n)];

					if (g1 != 0.0 && f1.hessianArray != null)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g1 * f1.hessianArray[k];
							}
						}
					}

					if (g2 != 0.0 && f2.hessianArray != null)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g2 * f2.hessianArray[k];
							}
						}
					}

					if (g11 != 0.0 && f1.gradientArray != null)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g11 * f1.gradientArray[i] * f1.gradientArray[j];
							}
						}
					}

					if (g22 != 0.0 && f2.gradientArray != null)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g22 * f2.gradientArray[i] * f2.gradientArray[j];
							}
						}
					}

					if (g12 != 0.0 && f1.gradientArray != null && f2.gradientArray != null)
					{
						for (int i = 0, k = 0; i < n; i++)
						{
							for (int j = i; j < n; j++, k++)
							{
								hessianArray[k] += g12 * (f1.gradientArray[i] * f2.gradientArray[j] + f2.gradientArray[i] * f1.gradientArray[j]);
							}
						}
					}
				}
			}
		}

		public double[] ToGradientArray()
		{
			if (gradientArray == null)
			{
				return null;
			}

			return (double[])gradientArray.Clone();
		}

		public double[] ToHessianArray()
		{
			if (hessianArray == null)
			{
				return null;
			}

			return (double[])hessianArray.Clone();
		}

		/// <summary>
		/// Size of the linearized Hessian array given the size of the gradient.
		/// </summary>
		public static int HessianSize(int n)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			return n * (n + 1) / 2;
		}

		public static int HessianIndex(int n, int i, int j)
		{
			if (n < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			if (i > j)
			{
				return HessianIndex(n, j, i);
			}

			// Now i <= j as required.

			if (i < 0 || j >= n)
			{
				throw new ArgumentOutOfRangeException();
			}

			// This is the same as:
			// return HessianSize(n) - HessianSize(n - i) + j - i;
			return i * (1 - i) / 2 + i * n + j - i;
		}

		public static DualNumber Basis(double value, int n, int i)
		{
			return new DualNumber(value, Vector.Basis(n, i), Matrix.Zero(n, n));
		}

		public static DualNumber Exp(DualNumber f)
		{
			double g = Math.Exp(f.value);
			double g1 = g;
			double g11 = g;

			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Log(DualNumber f)
		{
			double g = Math.Log(f.value);
			double g1 = 1.0 / f.value;
			double g11 = -g1 / f.value;

			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Sqr(DualNumber f)
		{
			return new DualNumber(f, f.value * f.value, 2.0 * f.value, 2.0);
		}

		public static DualNumber Sqrt(DualNumber f)
		{
			double g = Math.Sqrt(f.value);
			double g1 = 0.5 / g;
			double g11 = -0.5 * g1 / f.value;

			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Pow(DualNumber f, double a)
		{
			double g = Math.Pow(f.value, a);
			double g1 = a * Math.Pow(f.value, a - 1.0);
			double g11 = a * (a - 1.0) * Math.Pow(f.value, a - 2.0);

			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Pow(double a, DualNumber f)
		{
			double g = Math.Pow(a, f.value);
			double c = Math.Log(a);
			double g1 = c * g;
			double g11 = c * g1;

			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Pow(DualNumber f1, DualNumber f2)
		{
			double g = Math.Pow(f1.value, f2.value);

			double c1 = Math.Pow(f1.value, f2.value - 1.0);
			double g1 = f2.value * c1;
			double g11 = f2.value * (f2.value - 1.0) * Math.Pow(f1.value, f2.value - 2.0);

			double c2 = Math.Log(f1.value);
			double g2 = c2 * g;
			double g22 = c2 * g2;

			double g12 = c1 * (1.0 + c2 * f2.value);

			return new DualNumber(f1, f2, g, g1, g2, g11, g12, g22);
		}

		public static DualNumber Cos(DualNumber f)
		{
			double g = Math.Cos(f.value);
			double g1 = -Math.Sin(f.value);
			double g11 = -g;
			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Sin(DualNumber f)
		{
			double g = Math.Sin(f.value);
			double g1 = Math.Cos(f.value);
			double g11 = -g;
			return new DualNumber(f, g, g1, g11);
		}

		public static DualNumber Min(DualNumber f, DualNumber g)
		{
			// Like the step function.
			return f.Value < g.Value ? f : g;
		}

		public static DualNumber Max(DualNumber f, DualNumber g)
		{
			// Like the step function.
			return f.Value > g.Value ? f : g;
		}

		public static implicit operator DualNumber(double a)
		{
			if (a == 0.0)
			{
				return zero;
			}

			return new DualNumber(a);
		}

		public static DualNumber operator +(DualNumber f1, DualNumber f2)
		{
			return new DualNumber(f1, f2, f1.value + f2.value, 1.0, 1.0, 0.0, 0.0, 0.0);
		}

		public static DualNumber operator +(DualNumber f, double a)
		{
			return new DualNumber(f, f.value + a, 1.0, 0.0);
		}

		public static DualNumber operator +(double a, DualNumber f)
		{
			return new DualNumber(f, a + f.value, 1.0, 0.0);
		}

		public static DualNumber operator -(DualNumber f)
		{
			return new DualNumber(f, -f.value, -1.0, 0.0);
		}

		public static DualNumber operator -(DualNumber f1, DualNumber f2)
		{
			return new DualNumber(f1, f2, f1.value - f2.value, 1.0, -1.0, 0.0, 0.0, 0.0);
		}

		public static DualNumber operator -(DualNumber f, double a)
		{
			return new DualNumber(f, f.value - a, 1.0, 0.0);
		}

		public static DualNumber operator -(double a, DualNumber f)
		{
			return new DualNumber(f, a - f.value, -1.0, 0.0);
		}

		public static DualNumber operator *(DualNumber f1, DualNumber f2)
		{
			return new DualNumber(f1, f2, f1.value * f2.value, f2.value, f1.value, 0.0, 1.0, 0.0);
		}

		public static DualNumber operator *(DualNumber f, double a)
		{
			return new DualNumber(f, f.value * a, a, 0.0);
		}

		public static DualNumber operator *(double a, DualNumber f)
		{
			return new DualNumber(f, a * f.value, a, 0.0);
		}

		public static DualNumber operator /(DualNumber f1, DualNumber f2)
		{
			double g = f1.value / f2.value;
			double g1 = 1.0 / f2.value;
			double g2 = -g / f2.value;
			double g11 = 0.0;
			double g12 = -g1 / f2.value;
			double g22 = -2.0 * g2 / f2.value;

			return new DualNumber(f1, f2, g, g1, g2, g11, g12, g22);
		}

		public static DualNumber operator /(DualNumber f, double a)
		{
			return new DualNumber(f, f.value / a, 1.0 / a, 0.0);
		}

		public static DualNumber operator /(double a, DualNumber f)
		{
			double g = a / f.value;
			double g1 = -g / f.value;
			double g11 = -2.0 * g1 / f.value;

			return new DualNumber(f, g, g1, g11);
		}

		/// <summary>
		/// Number of variables.
		/// </summary>
		public int N
		{
			get
			{
				return n;
			}
		}

		/// <summary>
		/// The numerical value of the <see cref="DualNumber" />, i.e. the pure value without derivatives.
		/// </summary>
		public double Value
		{
			get
			{
				return value;
			}
		}

		/// <summary>
		/// A <see cref="Vector" /> representation of the gradient.
		/// </summary>
		public Vector Gradient
		{
			get
			{
				if (gradient == null && gradientArray != null)
				{
					gradient = new Vector(gradientArray);
				}

				return gradient;
			}
		}

		/// <summary>
		/// A <see cref="Matrix " /> representation of the Hessian. The Hessian is converted from the internal
		/// linearized representation as returned by <see cref="ToHessianArray" />.
		/// </summary>
		public Matrix Hessian
		{
			get
			{
				if (hessian == null && hessianArray != null)
				{
					double[,] a = new double[n, n];
					for (int i = 0, k = 0; i < n; i++)
					{
						for (int j = i; j < n; j++, k++)
						{
							a[i, j] = a[j, i] = hessianArray[k];
						}
					}

					hessian = new Matrix(a);
				}

				return hessian;
			}
		}
	}
}
