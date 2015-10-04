// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Globalization;

namespace SharpMath.Optimization.DualNumbers
{
	/// <summary>
	/// To avoid errors in handcrafted <see cref="DualNumber" /> instances this derivative tester may be used. The verification is done by a simple
	/// finite differences approximation, where each component of the user-provided point is perturbed one by one. The second derivatives
	/// are approximated by finite differences of the first derivatives, so first derivatives should be tested before the second derivatives.
	/// </summary>
	public class DualNumberDerivativeTest
	{
		private Function function;
		private ImmutableVariableCollection variables;
		private int n;
		private double tolerance, perturbation;
		private bool showAll, showNames;

		/// <summary>
		/// Creates a new instance of the derivative tester.
		/// </summary>
		/// <param name="function">The function to test.</param>
		/// <param name="variables">The variables to test.</param>
		public DualNumberDerivativeTest(Function function, params Variable[] variables)
		{
			this.function = function;
			this.variables = ImmutableVariableCollection.Create(variables);

			n = variables.Length;
			tolerance = 0.0001;
			perturbation = 1.0e-6;
			showAll = false;
			showNames = false;
		}

		/// <summary>
		/// Creates a new instance of the derivative tester.
		/// </summary>
		/// <param name="function">The function to test constructed using <see cref="DualNumberFunction" />.</param>
		/// <param name="variables">The variables that the function depends on.</param>
		public DualNumberDerivativeTest(Func<IDualNumberTransform, DualNumber> function, params Variable[] variables)
			: this(DualNumberFunction.Create(function, variables), variables)
		{
		}

		public void Test(IPoint point)
		{
			// http://en.wikipedia.org/wiki/Finite_difference_coefficients

			//int k = 2;
			//int[] xdelta = new int[] { 0, 1 };
			//double[] wdelta = new double[] { -1.0, 1.0 };

			//int k = 2;
			//int[] xdelta = new int[] { -1, 1 };
			//double[] wdelta = new double[] { -0.5, 0.5 };

			int k = 4;
			int[] xdelta = new int[] { -2, -1, 1, 2 };
			double[] wdelta = new double[] { 0.083333333333333329, -0.66666666666666663, 0.66666666666666663, -0.083333333333333329 };

			//int k = 6;
			//int[] xdelta = new int[] { -3, -2, -1, 1, 2, 3 };
			//double[] wdelta = new double[] { -0.016666666666666666, 0.15, -0.75, 0.75, -0.15, 0.016666666666666666 };

			// The following test code is heavily inspired by Ipopt::TNLPAdapter::CheckDerivatives in IPOPT (see IpTNLPAdapter.cpp).

			Console.WriteLine("Evaluating unperturbed function");
			Console.WriteLine();

			DualNumber y = Compute(point);
			double[] delta = new double[n];
			DualNumber[] ydelta = new DualNumber[n];

			Console.WriteLine("Starting derivative checker for first derivatives");
			Console.WriteLine();

			for (int i = 0; i < n; i++)
			{
				double exact = y.Gradient[i];

				try
				{
					if (point[variables[i]] != 0.0)
					{
						delta[i] = perturbation * Math.Abs(point[variables[i]]);
					}
					else
					{
						// Don't know the scale in this particular case. Choose an arbitrary scale (i.e. 1).
						delta[i] = perturbation;
					}

					double approx = 0.0;
					for (int j = 0; j < k; j++)
					{
						approx += wdelta[j] * ComputeDelta(point, i, xdelta[j] * delta[i]).Value;
					}
					approx /= delta[i];

					ydelta[i] = ComputeDelta(point, i, delta[i]);
					//approx = (ydelta[i].Value - y.Value) / delta[i];
					double relativeError = Math.Abs(approx - exact) / Math.Max(1.0, Math.Abs(approx));

					bool error = relativeError >= tolerance;
					if (error || showAll)
					{
						Console.WriteLine("{0} Gradient [{1} {2}] = {3} ~ {4} [{5}]",
							error ? "*" : " ", FormatIndex(-1), FormatIndex(i), FormatValue(exact), FormatValue(approx), FormatRelativeValue(relativeError));
					}
				}
				catch (ArithmeticException)
				{
					Console.WriteLine("* Gradient [{0} {1}] = {2} ~ FAILED", FormatIndex(-1), FormatIndex(i), FormatValue(exact));
				}
			}

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Starting derivative checker for second derivatives");
			Console.WriteLine();

			for (int i = 0; i < n; i++)
			{
				// Though the Hessian is supposed to be symmetric test the full matrix anyway
				// (the finite difference could very well be different and provide insight).
				for (int j = 0; j < n; j++)
				{
					double exact = y.Hessian[i, j];

					try
					{
						double approx = 0.0;
						for (int l = 0; l < k; l++)
						{
							approx += wdelta[l] * ComputeDelta(point, i, xdelta[l] * delta[i]).Gradient[j];
						}
						approx /= delta[i];

						//approx = (ydelta[i].Gradient[j] - y.Gradient[j]) / delta[i];
						double relativeError = Math.Abs(approx - exact) / Math.Max(1.0, Math.Abs(approx));

						bool error = relativeError >= tolerance;
						if (error || showAll)
						{
							Console.WriteLine("{0} Hessian  [{1},{2}] = {3} ~ {4} [{5}]",
								error ? "*" : " ", FormatIndex(i), FormatIndex(j), FormatValue(exact), FormatValue(approx), FormatRelativeValue(relativeError));
						}
					}
					catch (ArithmeticException)
					{
						Console.WriteLine("* Hessian  [{0},{1}] = {2} ~ FAILED", FormatIndex(i), FormatIndex(j), FormatValue(exact));
					}
				}
			}
		}

		private string FormatValue(double value)
		{
			return value.ToString("0.00000000000000e+00", CultureInfo.InvariantCulture).PadLeft(21);
		}

		private string FormatRelativeValue(double value)
		{
			return value.ToString("0.000e+00", CultureInfo.InvariantCulture).PadLeft(10);
		}

		private string FormatIndex(int index)
		{
			int size = 4;

			if (showNames)
			{
				// Try to resolve the name of the variables.
				size = 12;
				if (index != -1 && variables[index].Name != null)
				{
					return variables[index].Name.PadRight(size);
				}

				// Fall through to show the index if not defined.
			}

			return (index != -1 ? index.ToString() : "").PadLeft(size);
		}

		/*private IPoint PointDelta(IPoint point, int index, double delta)
		{
			Dictionary<Variable, double> values = new Dictionary<Variable, double>(point.ToDictionary());
			values[variables[index]] = values[variables[index]] + delta;
			return new Point(values);
		}*/

		public DualNumber Compute(IPoint point)
		{
			double value = function.Value(point);

			double[] gradientArray = new double[n];
			double[] hessianArray = new double[DualNumber.HessianSize(n)];

			for (int i = 0, k = 0; i < n; i++)
			{
				Function derivative = function.Derivative(variables[i]);
				gradientArray[i] = derivative.Value(point);

				for (int j = i; j < n; j++, k++)
				{
					hessianArray[k] = derivative.Derivative(variables[j]).Value(point);
				}
			}

			return new DualNumber(value, gradientArray, hessianArray);
		}

		public DualNumber ComputeDelta(IPoint point, int index, double delta)
		{
			if (index < 0 || index >= n)
			{
				throw new IndexOutOfRangeException();
			}

			VariableAssignment[] assignments = new VariableAssignment[n];
			for (int i = 0; i < n; i++)
			{
				Variable variable = variables[i];
				double value = point[variable];

				// Add delta to the selected variable.
				if (i == index)
				{
					value += delta;
				}

				assignments[i] = new VariableAssignment(variable, value);
			}

			return Compute(new Point(assignments));
		}

		/// <summary>
		/// Threshold for indicating wrong derivative.
		/// </summary>
		public double Tolerance
		{
			get
			{
				return tolerance;
			}
			set
			{
				tolerance = value;
			}
		}

		/// <summary>
		/// The relative size of the perturbation in the derivative test. The default value (1e-8, about the square root of the machine precision) is probably fine in most cases.
		/// </summary>
		public double Perturbation
		{
			get
			{
				return perturbation;
			}
			set
			{
				perturbation = value;
			}
		}

		/// <summary>
		/// Toggle to show the user-provided and estimated derivative values with the relative deviation for each single partial derivative.
		/// </summary>
		public bool ShowAll
		{
			get
			{
				return showAll;
			}
			set
			{
				showAll = value;
			}
		}

		/// <summary>
		/// Show resolved variable names instead of indices.
		/// </summary>
		public bool ShowNames
		{
			get
			{
				return showNames;
			}
			set
			{
				showNames = value;
			}
		}
	}
}
