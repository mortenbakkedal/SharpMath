// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

using FuncLib.Mathematics;

namespace SharpMath.Optimization
{
	[Serializable]
	[DebuggerStepThrough]
	public sealed class FunctionVector
	{
		private FunctionMatrix inner;

		private FunctionVector(FunctionMatrix inner)
		{
			if (inner.Columns != 1)
			{
				throw new ArgumentException();
			}

			this.inner = inner;
		}

		public FunctionVector(Function[] entries)
		{
			int n = entries.Length;

			Function[,] a = new Function[n, 1];
			for (int i = 0; i < n; i++)
			{
				a[i, 0] = entries[i];
			}

			inner = new FunctionMatrix(a);
		}


		public Vector Value(IPoint point)
		{
			return (Vector)inner.Value(point);
		}

		public Vector Value(params VariableAssignment[] assignments)
		{
			return (Vector)inner.Value(assignments);
		}

		public Vector Value(IEvaluator evaluator)
		{
			return (Vector)inner.Value(evaluator);
		}

		public FunctionVector Derivative(Variable variable)
		{
			return new FunctionVector(inner.Derivative(variable));
		}

		public FunctionVector Derivative(Variable variable, int order)
		{
			return new FunctionVector(inner.Derivative(variable, order));
		}

		public FunctionVector Derivative(params Variable[] variables)
		{
			return new FunctionVector(inner.Derivative(variables));
		}

		public FunctionVector SetEntry(int index, Function f)
		{
			return new FunctionVector(inner.SetEntry(index, 0, f));
		}

		public FunctionVector SetVector(int index, FunctionVector a)
		{
			return new FunctionVector(inner.SetMatrix(index, 0, a.inner));
		}

		public FunctionVector GetVector(int index, int length)
		{
			return new FunctionVector(inner.GetMatrix(index, 0, length, 1));
		}

		public Function[] ToArray()
		{
			int n = Length;

			Function[] entries = new Function[n];
			for (int i = 0; i < n; i++)
			{
				entries[i] = this[i];
			}

			return entries;
		}

		public static FunctionVector Zero(int length)
		{
			return new FunctionVector(FunctionMatrix.Zero(length, 1));
		}

		public static FunctionVector Basis(int length, int index)
		{
			return new FunctionVector(FunctionMatrix.Basis(length, 1, index, 0));
		}

		public static implicit operator FunctionMatrix(FunctionVector a)
		{
			return a.inner;
		}

		public static explicit operator FunctionVector(FunctionMatrix a)
		{
			if (a.Columns == 1)
			{
				return new FunctionVector(a);
			}

			if (a.Rows == 1)
			{
				// Requires copying of the references to Function entries.
				return new FunctionVector(FunctionMatrix.Transpose(a));
			}

			throw new InvalidCastException("The matrix has no vector representation.");
		}

		public static implicit operator FunctionVector(Vector a)
		{
			return new FunctionVector((Matrix)a);
		}

		public static FunctionVector operator +(FunctionVector a, FunctionVector b)
		{
			return new FunctionVector(a.inner + b.inner);
		}

		public static FunctionVector operator +(FunctionVector a, Function f)
		{
			return new FunctionVector(a.inner + f);
		}

		public static FunctionVector operator +(Function f, FunctionVector a)
		{
			return new FunctionVector(f + a.inner);
		}

		public static FunctionVector operator -(FunctionVector a)
		{
			return new FunctionVector(-a.inner);
		}

		public static FunctionVector operator -(FunctionVector a, FunctionVector b)
		{
			return new FunctionVector(a.inner - b.inner);
		}

		public static FunctionVector operator -(FunctionVector a, Function f)
		{
			return new FunctionVector(a.inner - f);
		}

		public static FunctionVector operator -(Function f, FunctionVector a)
		{
			return new FunctionVector(f - a.inner);
		}

		public static FunctionVector operator *(FunctionMatrix a, FunctionVector b)
		{
			return new FunctionVector(a * b.inner);
		}

		public static FunctionVector operator *(FunctionVector a, Function f)
		{
			return new FunctionVector(a.inner * f);
		}

		public static FunctionVector operator *(Function f, FunctionVector a)
		{
			return new FunctionVector(f * a.inner);
		}

		public static FunctionVector operator /(FunctionVector a, Function f)
		{
			return new FunctionVector(a.inner / f);
		}

		public Function this[int index]
		{
			get
			{
				return inner[index, 0];
			}
		}

		public int Length
		{
			get
			{
				return inner.Rows;
			}
		}
	}
}
