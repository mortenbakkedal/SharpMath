// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;

using SharpMath.Collections;
using SharpMath.LinearAlgebra;

#pragma warning disable

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	public class AutomatedLinearRegression
	{
		public AutomatedLinearRegression(IEnumerable<LinearRegressionFactor> factors)
		{
			Factors = new ImmutableList<LinearRegressionFactor>(factors);
		}

		public static AutomatedLinearRegression Fit(IRegressionVariable regressand, IEnumerable<IRegressionVariable> regressors)
		{
			if (regressors == null)
			{
				throw new ArgumentNullException();
			}

			return Fit(regressand, regressors.ToArray());
		}

		public static AutomatedLinearRegression Fit(IRegressionVariable regressand, params IRegressionVariable[] regressors)
		{
			if (regressand == null || regressors == null)
			{
				throw new ArgumentNullException();
			}

			int p = regressors.Length;
			for (int i = 0; i < p; i++)
			{
				if (regressors[i] == null)
				{
					throw new ArgumentNullException();
				}
			}

			// The regressand is required not to be an infinite sequence.
			double[] y = regressand.Values.ToArray();
			int n = y.Length;

			// The regressor sequences may be. But must be at least as long as the regressand.
			double[,] x = new double[n, p];
			for (int i = 0; i < p; i++)
			{
				using (IEnumerator<double> iterator = regressors[i].Values.GetEnumerator())
				{
					for (int j = 0; j < n; j++)
					{
						if (!iterator.MoveNext())
						{
							throw new Exception("Regressor sequence ended unexpectedly.");
						}
						x[j, i] = iterator.Current;
					}
				}
			}

			LinearRegression lr = LinearRegression.Fit(new Vector(y), new Matrix(x));
			//SimpleLinearRegression lr = new SimpleLinearRegression.Fit(new Vector(y), new Matrix(x));

			LinearRegressionFactor[] factors = new LinearRegressionFactor[p];
			for (int j = 0; j < p; j++)
			{
				double t = lr.Beta[j] / lr.Sigma[j];

				// Null probability, $Pr(>|t|)$.
				// http://sourceforge.net/p/varianttools/mailman/message/29121481/
				throw new NotImplementedException(); double pr;//double pr = 2.0 * Gsl.gsl_cdf_tdist_Q(Math.Abs(t), n - p);

				factors[j] = new LinearRegressionFactor(regressors[j], lr.Beta[j], lr.Sigma[j], pr);
			}

			return new AutomatedLinearRegression(factors);
		}

		public static AutomatedLinearRegression FitBySignificance(double significanceLevel, IRegressionVariable regressand, IEnumerable<IRegressionVariable> regressors)
		{
			if (significanceLevel < 0.0 || significanceLevel > 1.0)
			{
				throw new ArgumentOutOfRangeException();
			}

			return FitBySignificanceFactors(significanceLevel, int.MaxValue, regressand, regressors);
		}

		public static AutomatedLinearRegression FitBySignificance(double significanceLevel, IRegressionVariable regressand, params IRegressionVariable[] regressors)
		{
			return FitBySignificance(significanceLevel, regressand, (IEnumerable<IRegressionVariable>)regressors);
		}

		public static AutomatedLinearRegression FitByFactors(int factors, IRegressionVariable regressand, IEnumerable<IRegressionVariable> regressors)
		{
			// The backward mode described here:
			// http://faculty.smu.edu/tfomby/eco5385/lecture/Multiple%20Linear%20Regression%20and%20Subset%20Selection.pdf

			if (factors < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			return FitBySignificanceFactors(double.NegativeInfinity, factors, regressand, regressors);
		}

		public static AutomatedLinearRegression FitByFactors(int factors, IRegressionVariable regressand, params IRegressionVariable[] regressors)
		{
			return FitBySignificance(factors, regressand, (IEnumerable<IRegressionVariable>)regressors);
		}

		private static AutomatedLinearRegression FitBySignificanceFactors(double significanceLevel, int factors, IRegressionVariable regressand, IEnumerable<IRegressionVariable> regressors)
		{
			if (regressors.Count(r => r.Required) > factors)
			{
				throw new ArgumentException("Too many factors are required.");
			}

			// Threshold p-value.
			double p0 = 1.0 - significanceLevel;

			// Start by a non-automated regression.
			AutomatedLinearRegression lr = AutomatedLinearRegression.Fit(regressand, regressors);

			while (true)
			{
				// Find worst non-significant factor.
				LinearRegressionFactor f0 = lr.Factors.Where(f => !f.Regressor.Required).OrderByDescending(f => f.NullProbability).First();

				if (lr.Factors.Count <= factors && f0.NullProbability <= p0)
				{
					// Satisfying maximum number of factors and worst significance level.
					return lr;
				}

				if (lr.Factors.Count == 1)
				{
					// About to remove last factor.
					return new AutomatedLinearRegression(new LinearRegressionFactor[0]);
				}

				lr = AutomatedLinearRegression.Fit(regressand, lr.Factors.Where(f => f != f0).Select(f => f.Regressor));
			}
		}


		public static AutomatedLinearRegression FitByFactorsCombinatorial(int factors, IRegressionVariable regressand, IEnumerable<IRegressionVariable> regressors)
		{
			if (factors < 1)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = regressors.Count(r => !r.Required);

			if (n <= factors)
			{
				// Already below maximum number of factors.
				return Fit(regressand, regressors);
			}

			throw new NotImplementedException();
		}

		public static AutomatedLinearRegression FitByFactorsCombinatorial(int factors, IRegressionVariable regressand, params IRegressionVariable[] regressors)
		{
			return FitByFactorsCombinatorial(factors, regressand, (IEnumerable<IRegressionVariable>)regressors);
		}

		public ImmutableList<LinearRegressionFactor> Factors
		{
			get;
			private set;
		}
	}

	[Serializable]
	public class AutomatedLinearRegression<T> : AutomatedLinearRegression
	{
		public AutomatedLinearRegression(IEnumerable<LinearRegressionFactor> factors)
			: base(factors)
		{
		}

		public static AutomatedLinearRegression<T> Fit(IRegressionVariable regressand, IEnumerable<IRegressionVariable<T>> regressors)
		{
			return new AutomatedLinearRegression<T>(AutomatedLinearRegression.Fit(regressand, regressors).Factors);
		}

		public static AutomatedLinearRegression<T> Fit(IRegressionVariable regressand, params IRegressionVariable<T>[] regressors)
		{
			return Fit(regressand, (IEnumerable<IRegressionVariable<T>>)regressors);
		}

		public static AutomatedLinearRegression<T> FitBySignificance(double significanceLevel, IRegressionVariable regressand, IEnumerable<IRegressionVariable<T>> regressors)
		{
			return new AutomatedLinearRegression<T>(AutomatedLinearRegression.FitBySignificance(significanceLevel, regressand, regressors).Factors);
		}

		public static AutomatedLinearRegression<T> FitBySignificance(double significanceLevel, IRegressionVariable regressand, params IRegressionVariable<T>[] regressors)
		{
			return FitBySignificance(significanceLevel, regressand, (IEnumerable<IRegressionVariable<T>>)regressors);
		}

		public static AutomatedLinearRegression<T> FitByFactors(int factors, IRegressionVariable regressand, IEnumerable<IRegressionVariable<T>> regressors)
		{
			return new AutomatedLinearRegression<T>(AutomatedLinearRegression.FitByFactors(factors, regressand, regressors).Factors);
		}

		public static AutomatedLinearRegression<T> FitByFactors(int factors, IRegressionVariable regressand, params IRegressionVariable<T>[] regressors)
		{
			return FitByFactors(factors, regressand, (IEnumerable<IRegressionVariable<T>>)regressors);
		}

		/// <summary>
		/// The regresson value.
		/// </summary>
		public double Value(T value)
		{
			double x = 0.0;
			foreach (LinearRegressionFactor factor in Factors)
			{
				x += factor.Coefficient * ((IRegressionVariable<T>)factor.Regressor).Transform(value);
			}

			return x;
		}
	}
}
