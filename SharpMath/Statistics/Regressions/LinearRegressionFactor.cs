// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class LinearRegressionFactor
	{
		public LinearRegressionFactor(IRegressionVariable regressor, double coefficient, double standardError, double nullProbability)
		{
			Regressor = regressor;
			Coefficient = coefficient;
			StandardError = standardError;
			T = coefficient / standardError;
			NullProbability = nullProbability;
		}

		public IRegressionVariable Regressor
		{
			get;
			private set;
		}

		public double Coefficient
		{
			get;
			private set;
		}

		public double StandardError
		{
			get;
			private set;
		}

		public double T
		{
			get;
			private set;
		}

		/// <summary>
		/// Estimated probability of the null hypothesis that the coefficient is equal to zero.
		/// </summary>
		public double NullProbability
		{
			get;
			private set;
		}

		public string SignificanceCode
		{
			get
			{
				if (NullProbability < 0.001)
				{
					return "***";
				}
				else if (NullProbability < 0.01)
				{
					return "**";
				}
				else if (NullProbability < 0.05)
				{
					return "*";
				}
				else if (NullProbability < 0.1)
				{
					return ".";
				}
				else
				{
					return "";
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				string prefix;
				if (Regressor is RegressionVariable && ((RegressionVariable)Regressor).Name != null && ((RegressionVariable)Regressor).Name != "")
				{
					prefix = ((RegressionVariable)Regressor).Name + ": ";
				}
				else if (Regressor is ConstantRegressionVariable)
				{
					prefix = "(Const): ";
				}
				else
				{
					prefix = "";
				}

				string suffix = SignificanceCode;
				if (suffix != "")
				{
					suffix = " " + suffix;
				}

				return string.Format("{0}Coefficient = {1}, NullProbability = {2}{3}", prefix, Coefficient, NullProbability, suffix);
			}
		}
	}
}
