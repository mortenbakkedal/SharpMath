// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization.DualNumbers
{
	/// <summary>
	/// A sparse version of DualNumber based on SparseMatrix. First version with only the gradient.
	/// </summary>
	public class SparseDualNumber
	{
		private double value;
		private int[] variables;
		private double[] gradient;
		//private Dictionary<int, double> gradient;

		private SparseDualNumber(double value, Dictionary<int, double> gradient)
		{
			this.value = value;
			//this.gradient = gradient;
		}

		public SparseDualNumber(SparseDualNumber f, double g, double g1)
		{
			value = g;

			if (g1 != 0.0 && f.variables != null)
			{
				variables = f.variables;

				int n = variables.Length;

				gradient = new double[n];
				for (int i = 0; i < n; i++)
				{
					gradient[i] = g1 * f.gradient[i];
				}
			}
		}

		public SparseDualNumber(SparseDualNumber f1, SparseDualNumber f2, double g, double g1, double g2)
		{
			value = g;

			/*gradient = new Dictionary<int, double>();

			if (g1 != 0.0)
			{
				foreach (KeyValuePair<int, double> gr in f1.gradient)
				{
					gradient[gr.Key] += g1 * gr.Value;
				}
			}

			if (g2 != 0.0)
			{
				foreach (KeyValuePair<int, double> gr in f1.gradient)
				{
					gradient[gr.Key] += g2 * gr.Value;
				}
			}*/
		}

		public static SparseDualNumber Basis(double value, int variableIndex)
		{
			Dictionary<int, double> gradient = new Dictionary<int, double>();
			gradient[variableIndex] = 1.0;
			return new SparseDualNumber(value, gradient);
		}

		public static SparseDualNumber operator +(SparseDualNumber f1, SparseDualNumber f2)
		{
			return new SparseDualNumber(f1, f2, f1.value + f2.value, 1.0, 1.0);
		}

		public static SparseDualNumber Exp(SparseDualNumber f)
		{
			double c = Math.Exp(f.value);
			return new SparseDualNumber(f, c, c);
		}
	}
}
