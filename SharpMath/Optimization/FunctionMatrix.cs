// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	/// <summary>
	/// A matrix class with <see cref="Function" /> entries. Performance is not as critical as for the <see cref="Matrix" /> class due to the significant
	/// overhead of object representation of the entries, so this class is just a wrapper around a Function[,] array.
	/// </summary>
	[Serializable]
	[DebuggerStepThrough]
	public sealed class FunctionMatrix
	{
		private Function[,] entries;

		public FunctionMatrix(Function[,] entries)
		{
			// Copy to keep immutable.
			this.entries = (Function[,])entries.Clone();
		}

		private FunctionMatrix(int rows, int columns)
		{
			// This constructor is intentionally kept private. Use Zero instead.

			entries = new Function[rows, columns];

			// We can afford to zero out all entries, since the assignments are only performed once for Function objects.
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					entries[i, j] = 0.0;
				}
			}
		}

		public Matrix Value(IPoint point)
		{
			return Value(Evaluator.Create(point));
		}

		public Matrix Value(params VariableAssignment[] assignments)
		{
			return Value(new Point(assignments));
		}

		public Matrix Value(IEvaluator evaluator)
		{
			int n = Rows;
			int m = Columns;

			double[,] values = new double[n, m];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					values[i, j] = this[i, j].Value(evaluator);
				}
			}

			return new Matrix(values);
		}

		public FunctionMatrix Derivative(Variable variable)
		{
			int n = Rows;
			int m = Columns;

			Function[,] derivatives = new Function[n, m];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					derivatives[i, j] = this[i, j].Derivative(variable);
				}
			}

			return new FunctionMatrix(derivatives);
		}

		public FunctionMatrix Derivative(Variable variable, int order)
		{
			if (order < 0)
			{
				throw new ArgumentOutOfRangeException("order");
			}

			FunctionMatrix a = this;
			for (int i = 0; i < order; i++)
			{
				a = a.Derivative(variable);
			}

			return a;
		}

		public FunctionMatrix Derivative(params Variable[] variables)
		{
			FunctionMatrix a = this;
			for (int i = 0; i < variables.Length; i++)
			{
				a = a.Derivative(variables[i]);
			}

			return a;
		}

		public FunctionMatrix SetEntry(int row, int column, Function f)
		{
			FunctionMatrix a = new FunctionMatrix(entries);
			a[row, column] = f;

			return a;
		}

		public FunctionMatrix SetMatrix(int row, int column, FunctionMatrix a)
		{
			int n = a.Rows;
			int m = a.Columns;

			FunctionMatrix b = new FunctionMatrix(entries);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[row + i, column + j] = a[i, j];
				}
			}

			return b;
		}

		public FunctionMatrix GetMatrix(int row, int column, int rows, int columns)
		{
			int n = rows;
			int m = columns;

			FunctionMatrix a = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					a[i, j] = this[row + i, column + j];
				}
			}

			return a;
		}

		public FunctionMatrix GetRow(int row)
		{
			int n = Columns;

			return GetMatrix(row, 0, 1, n);
		}

		public FunctionVector GetColumn(int column)
		{
			int n = Rows;

			// Use conversion operator defined in FunctionVector.
			return (FunctionVector)(GetMatrix(0, column, n, 1));
		}

		public Function[,] ToArray()
		{
			return (Function[,])entries.Clone();
		}

		public static FunctionMatrix Zero(int rows, int columns)
		{
			return new FunctionMatrix(rows, columns);
		}

		public static FunctionMatrix Identity(int rows, int columns)
		{
			int n = Math.Min(rows, columns);

			FunctionMatrix a = new FunctionMatrix(rows, columns);
			for (int i = 0; i < n; i++)
			{
				a[i, i] = 1.0;
			}

			return a;
		}

		public static FunctionMatrix Diagonal(Function[] values)
		{
			int n = values.Length;

			FunctionMatrix a = new FunctionMatrix(n, n);
			for (int i = 0; i < n; i++)
			{
				a[i, i] = values[i];
			}

			return a;
		}

		public static FunctionMatrix Basis(int rows, int columns, int row, int column)
		{
			FunctionMatrix a = new FunctionMatrix(rows, columns);
			a[row, column] = 1.0;

			return a;
		}

		public static FunctionMatrix Transpose(FunctionMatrix a)
		{
			int n = a.Columns;
			int m = a.Rows;

			FunctionMatrix b = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[j, i];
				}
			}

			return b;
		}

		public static Function Trace(FunctionMatrix a)
		{
			int n = a.Rows;

			if (a.Columns != n)
			{
				throw new ArgumentException("The matrix isn't a square matrix.");
			}

			Function s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += a[i, i];
			}

			return s;
		}

		public static FunctionMatrix Inverse(FunctionMatrix a)
		{
			FunctionMatrix ainv;
			Function adet;
			InverseDeterminant(a, out ainv, out adet);

			// Ignore the unused determinant object. It's reclaimed at a neglectable performance penalty.
			return ainv;
		}

		public static Function Determinant(FunctionMatrix a)
		{
			FunctionMatrix ainv;
			Function adet;
			InverseDeterminant(a, out ainv, out adet);

			// Here the inverse matrix object is actually used by the determinant object.
			return adet;
		}

		/// <summary>
		/// Computes the inverse and the determinant simultaneously. This avoids evaluating similar expressions twice.
		/// </summary>
		public static void InverseDeterminant(FunctionMatrix a, out FunctionMatrix ainv, out Function adet)
		{
			int n = a.Rows;

			if (a.Columns != n)
			{
				throw new ArgumentException("The matrix isn't a square matrix.");
			}

			// Use formulas for small matrices. This seems to be more efficient.

			if (n == 0)
			{
				// Allow inverses for degenerated matrices with no entries. May be useful.
				ainv = a;
				adet = 0.0;

				return;
			}

			if (n == 1)
			{
				Function a11 = a[0, 0];

				adet = a11;
				ainv = new FunctionMatrix(new Function[,] { { 1.0 / a11 } });

				return;
			}

			if (n == 2)
			{
				Function a11 = a[0, 0];
				Function a12 = a[0, 1];
				Function a21 = a[1, 0];
				Function a22 = a[1, 1];

				adet = a11 * a22 - a12 * a21;
				ainv = new FunctionMatrix(new Function[,] { { a22 / adet, -a12 / adet }, { -a21 / adet, a11 / adet } });

				return;
			}

			if (n == 3)
			{
				Function a11 = a[0, 0];
				Function a12 = a[0, 1];
				Function a13 = a[0, 2];
				Function a21 = a[1, 0];
				Function a22 = a[1, 1];
				Function a23 = a[1, 2];
				Function a31 = a[2, 0];
				Function a32 = a[2, 1];
				Function a33 = a[2, 2];

				// Using http://mathworld.wolfram.com/MatrixInverse.html.
				adet = a11 * a22 * a33 + a12 * a23 * a31 + a13 * a21 * a32 - a11 * a23 * a32 - a12 * a21 * a33 - a13 * a22 * a31;
				ainv = new FunctionMatrix(new Function[,] {
					{ (a22 * a33 - a23 * a32) / adet, (a13 * a32 - a12 * a33) / adet, (a12 * a23 - a13 * a22) / adet },
					{ (a23 * a31 - a21 * a33) / adet, (a11 * a33 - a13 * a31) / adet, (a13 * a21 - a11 * a23) / adet },
					{ (a21 * a32 - a22 * a31) / adet, (a12 * a31 - a11 * a32) / adet, (a11 * a22 - a12 * a21) / adet }
				});

				return;
			}

			// Otherwise use the generic algorithm.
			ainv = InverseHelper.Create(a);
			adet = new DeterminantFunction(a, ainv);
		}

		public static implicit operator FunctionMatrix(Matrix a)
		{
			int n = a.Rows;
			int m = a.Columns;

			FunctionMatrix b = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j];
				}
			}

			return b;
		}

		public static FunctionMatrix operator +(FunctionMatrix a, FunctionMatrix b)
		{
			int n = a.Rows;
			int m = a.Columns;

			if (b.Rows != n || b.Columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			FunctionMatrix c = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c[i, j] = a[i, j] + b[i, j];
				}
			}

			return c;
		}

		public static FunctionMatrix operator +(FunctionMatrix a, Function f)
		{
			int n = a.Rows;
			int m = a.Columns;

			FunctionMatrix b = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j] + f;
				}
			}

			return b;
		}

		public static FunctionMatrix operator +(Function f, FunctionMatrix a)
		{
			return a + f;
		}

		public static FunctionMatrix operator -(FunctionMatrix a)
		{
			return a * (-1.0);
		}

		public static FunctionMatrix operator -(FunctionMatrix a, FunctionMatrix b)
		{
			int n = a.Rows;
			int m = a.Columns;

			if (b.Rows != n || b.Columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			FunctionMatrix c = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c[i, j] = a[i, j] - b[i, j];
				}
			}

			return c;
		}

		public static FunctionMatrix operator -(FunctionMatrix a, Function f)
		{
			return a + (-f);
		}

		public static FunctionMatrix operator -(Function f, FunctionMatrix a)
		{
			int n = a.Rows;
			int m = a.Columns;

			FunctionMatrix b = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = f - a[i, j];
				}
			}

			return b;
		}

		public static FunctionMatrix operator *(FunctionMatrix a, FunctionMatrix b)
		{
			int n = a.Rows;
			int m = b.Columns;
			int l = a.Columns;

			if (b.Rows != l)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			FunctionMatrix c = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					Function s = 0.0;
					for (int k = 0; k < l; k++)
					{
						s += a[i, k] * b[k, j];
					}

					c[i, j] = s;
				}
			}

			return c;
		}

		public static FunctionMatrix operator *(FunctionMatrix a, Function f)
		{
			int n = a.Rows;
			int m = a.Columns;

			FunctionMatrix b = new FunctionMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j] * f;
				}
			}

			return b;
		}

		public static FunctionMatrix operator *(Function f, FunctionMatrix a)
		{
			return a * f;
		}

		public static FunctionMatrix operator /(FunctionMatrix a, Function f)
		{
			return a * (1.0 / f);
		}

		public Function this[int row, int column]
		{
			get
			{
				return entries[row, column];
			}
			private set
			{
				// This property is kept private. The class is immutable by design.
				entries[row, column] = value;
			}
		}

		public int Rows
		{
			get
			{
				return entries.GetLength(0);
			}
		}

		public int Columns
		{
			get
			{
				return entries.GetLength(1);
			}
		}

		[Serializable]
		private class DeterminantFunction : Function
		{
			private FunctionMatrix a, ainv;

			public DeterminantFunction(FunctionMatrix a, FunctionMatrix ainv)
			{
				this.a = a;
				this.ainv = ainv;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Matrix.Determinant(a.Value(evaluator));
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				// Using (41) from The Matrix Cookbook.
				return this * FunctionMatrix.Trace(ainv * a.Derivative(variable));
			}
		}

		[Serializable]
		private class InverseHelper
		{
			private FunctionMatrix a, ainv;
			private PointCache<Matrix> inverses;
			private Dictionary<Variable, FunctionMatrix> derivatives;

			private InverseHelper(FunctionMatrix a)
			{
				this.a = a;

				inverses = new PointCache<Matrix>();
				derivatives = new Dictionary<Variable, FunctionMatrix>();
			}

			private void UpdateInverse(FunctionMatrix ainv)
			{
				this.ainv = ainv;
			}

			public static FunctionMatrix Create(FunctionMatrix a)
			{
				int n = a.Rows;

				InverseHelper helper = new InverseHelper(a);

				Function[,] entries = new Function[n, n];
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < n; j++)
					{
						entries[i, j] = new ValueFunction(helper, i, j);
					}
				}

				FunctionMatrix ainv = new FunctionMatrix(entries);

				// We need the inverse matrix object to compute derivatives.
				helper.UpdateInverse(ainv);

				return ainv;
			}

			private Matrix ComputeValue(IEvaluator evaluator)
			{
				Matrix inverse;
				if (!inverses.TryGetValue(evaluator.Point, out inverse))
				{
					inverse = Matrix.Inverse(a.Value(evaluator));
					inverses.Add(evaluator.Point, inverse);
				}

				return inverse;
			}
			
			private FunctionMatrix ComputeDerivative(Variable variable)
			{
				// It's important for efficient evaluation to use the same object for all entries.

				FunctionMatrix derivative;
				if (!derivatives.TryGetValue(variable, out derivative))
				{
					// Using (53) from The Matrix Cookbook.
					derivative = -ainv * a.Derivative(variable) * ainv;
					derivatives.Add(variable, derivative);
				}

				return derivative;
			}

			[Serializable]
			private class ValueFunction : Function
			{
				private InverseHelper helper;
				private int row, column;

				public ValueFunction(InverseHelper helper, int row, int column)
				{
					this.helper = helper;
					this.row = row;
					this.column = column;
				}

				protected override double ComputeValue(IEvaluator evaluator)
				{
					return helper.ComputeValue(evaluator)[row, column];
				}
				
				protected override Function ComputeDerivative(Variable variable)
				{
					return helper.ComputeDerivative(variable)[row, column];
				}
			}
		}
	}
}
