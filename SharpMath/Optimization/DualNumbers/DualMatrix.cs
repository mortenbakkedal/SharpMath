// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SharpMath.Optimization.DualNumbers
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class DualMatrix
	{
		private DualNumber[,] entries;
		//hashCode

		public DualMatrix(DualNumber[,] entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException();
			}

			for (int i = 0; i < entries.GetLength(0); i++)
			{
				for (int j = 0; j < entries.GetLength(1); j++)
				{
					if (entries[i, j] == null)
					{
						throw new ArgumentNullException();
					}
				}
			}

			this.entries = (DualNumber[,])entries.Clone();
		}

		// more overloads

		public DualMatrix(int rows, int columns)
			: this(rows, columns, 0.0)
		{
			// Dependence is the other way around here to ensure non-null initialization values.
		}

		public DualMatrix(int rows, int columns, DualNumber value)
		{
			if (rows < 0 || columns < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			entries = new DualNumber[rows, columns];
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					entries[i, j] = value;
				}
			}
		}

		public DualMatrix(Matrix values)
			: this(values, null)
		{
		}

		public DualMatrix(Matrix values, Matrix[] gradients)
			: this(values, gradients, null)
		{
		}

		public DualMatrix(Matrix values, Matrix[] gradients, Matrix[,] hessians)
		{
			if (values == null || gradients == null || hessians == null)
			{
				throw new ArgumentNullException();
			}

			int rows = values.Rows;
			int columns = values.Columns;

			entries = new DualNumber[rows, columns];

			int n = 0;

			if (gradients != null)
			{
				n = gradients.Length;

				for (int i = 0; i < n; i++)
				{
					if (gradients[i] == null)
					{
						throw new ArgumentNullException("gradients", "The gradients must be fully specified.");
					}

					if (gradients[i].Rows != rows || gradients[i].Columns != columns)
					{
						throw new ArgumentException("Inconsistent matrix sizes.");
					}
				}
			}

			if (hessians != null)
			{
				if (gradients == null)
				{
					throw new ArgumentException("The gradients must be specified if the Hessians are specified.");
				}

				if (hessians.GetLength(0) != n || hessians.GetLength(1) != n)
				{
					throw new ArgumentException("Inconsistent number of derivatives.");
				}

				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < n; j++)
					{
						if (hessians[i, j] == null)
						{
							throw new ArgumentNullException("hessians", "The Hessians must be fully specified.");
						}

						if (hessians[i, j].Rows != rows || hessians[i, j].Columns != columns)
						{
							throw new ArgumentException("Inconsistent matrix sizes.");
						}
					}
				}
			}

			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					double value = values[i, j];

					Vector gradient = null;
					if (gradients != null)
					{
						double[] a = new double[n];
						for (int k = 0; k < n; k++)
						{
							a[k] = gradients[k][i, j];
						}
						gradient = new Vector(a);
					}

					Matrix hessian = null;
					if (hessians != null)
					{
						double[,] a = new double[n, n];
						for (int k = 0; k < n; k++)
						{
							for (int l = 0; l < n; l++)
							{
								a[k, l] = hessians[k, l][i, j];
							}
						}
						hessian = new Matrix(a);
					}

					entries[i, j] = new DualNumber(value, gradient, hessian);
				}
			}
		}

		public DualMatrix SetEntry(int row, int column, DualNumber value)
		{
			if (row < 0 || row >= Rows || column < 0 || column >= Columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			DualMatrix a = new DualMatrix(entries);
			a[row, column] = value;

			return a;
		}

		public DualMatrix SetMatrix(int row, int column, DualMatrix subMatrix)
		{
			if (row < 0 || row + subMatrix.Rows > Rows || column < 0 || column + subMatrix.Columns > Columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = subMatrix.Rows;
			int m = subMatrix.Columns;

			DualMatrix b = new DualMatrix(entries);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[row + i, column + j] = subMatrix[i, j];
				}
			}

			return b;
		}

		public DualMatrix GetMatrix(int row, int column, int rows, int columns)
		{
			if (rows < 0 || row < 0 || row + rows > Rows || columns < 0 || column < 0 || column + columns > Columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = rows;
			int m = columns;

			DualMatrix a = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					a[i, j] = this[row + i, column + j];
				}
			}

			return a;
		}

		public DualVector GetRow(int row)
		{
			int n = Columns;

			// Use conversion operator defined in DualVector.
			return (DualVector)GetMatrix(row, 0, 1, n);
		}

		public DualVector GetColumn(int column)
		{
			int n = Rows;

			// Use conversion operator defined in DualVector.
			return (DualVector)GetMatrix(0, column, n, 1);
		}

		public Matrix GetValues()
		{
			int n = Rows;
			int m = Columns;

			double[,] a = new double[n, m];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					a[i, j] = this[i, j].Value;
				}
			}

			return new Matrix(a);
		}

		public Matrix GetGradients(int index)
		{
			int n = Rows;
			int m = Columns;

			double[,] a = new double[n, m];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					Vector gradient = this[i, j].Gradient;
					if (gradient != null)
					{
						a[i, j] = gradient[index];
					}
				}
			}

			return new Matrix(a);
		}

		public Matrix GetHessians(int index1, int index2)
		{
			int n = Rows;
			int m = Columns;

			double[,] a = new double[n, m];
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					Matrix hessian = this[i, j].Hessian;
					if (hessian != null)
					{
						a[i, j] = hessian[index1, index2];
					}
				}
			}

			return new Matrix(a);
		}

		public DualNumber[,] ToArray()
		{
			return (DualNumber[,])entries.Clone();
		}

		public DualNumber[][] ToJaggedArray()
		{
			DualNumber[][] entries = new DualNumber[Rows][];
			for (int i = 0; i < Rows; i++)
			{
				entries[i] = new DualNumber[Columns];
				for (int j = 0; j < Columns; j++)
				{
					entries[i][j] = this[i, j];
				}
			}

			return entries;
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public string ToString(string format)
		{
			return GetValues().ToString(format);
		}

		public static DualMatrix Zero(int rows, int columns)
		{
			return new DualMatrix(rows, columns);
		}

		public static DualMatrix Identity(int rows, int columns)
		{
			if (rows < 0 || columns < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = Math.Min(rows, columns);

			DualMatrix a = new DualMatrix(rows, columns);
			for (int i = 0; i < n; i++)
			{
				a[i, i] = 1.0;
			}

			return a;
		}

		public static DualMatrix Diagonal(params DualNumber[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException();
			}

			int n = values.Length;

			DualMatrix a = new DualMatrix(n, n);
			for (int i = 0; i < n; i++)
			{
				a[i, i] = values[i];
			}

			return a;
		}

		public static DualMatrix Diagonal(IEnumerable<DualNumber> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException();
			}

			return Diagonal(values.ToArray());
		}

		public static DualMatrix Basis(int rows, int columns, int row, int column)
		{
			if (rows < 0 || row < 0 || row >= rows || columns < 0 || column < 0 || column >= columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			DualMatrix a = new DualMatrix(rows, columns);
			a[row, column] = 1.0;

			return a;
		}

		public static DualMatrix Transpose(DualMatrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException();
			}

			int n = matrix.Columns;
			int m = matrix.Rows;

			DualMatrix b = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = matrix[j, i];
				}
			}

			return b;
		}

		public static DualNumber Trace(DualMatrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException();
			}

			int n = matrix.Rows;

			if (matrix.Columns != n)
			{
				throw new ArgumentException("Non-square matrix.");
			}

			DualNumber s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += matrix[i, i];
			}

			return s;
		}

		public static DualMatrix Inverse(DualMatrix matrix)
		{
			// Use direct computation for low dimenstions; LU decomposition otherwise.
			return DualLUDecomposition.Inverse(matrix);
		}

		public static DualNumber Determinant(DualMatrix matrix)
		{
			// Use direct computation for low dimenstions; LU decomposition otherwise.
			return DualLUDecomposition.Determinant(matrix);
		}

		public static implicit operator DualMatrix(Matrix matrix)
		{
			return new DualMatrix(matrix);
		}

		public static DualMatrix operator +(DualMatrix a, DualMatrix b)
		{
			// hertil!!!!!!!!!!!!!

			int n = a.Rows;
			int m = a.Columns;

			if (b.Rows != n || b.Columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			DualMatrix c = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c[i, j] = a[i, j] + b[i, j];
				}
			}

			return c;
		}

		public static DualMatrix operator +(DualMatrix a, DualNumber t)
		{
			int n = a.Rows;
			int m = a.Columns;

			DualMatrix b = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j] + t;
				}
			}

			return b;
		}

		public static DualMatrix operator +(DualNumber t, DualMatrix a)
		{
			return a + t;
		}

		public static DualMatrix operator -(DualMatrix a)
		{
			return a * -1.0;
		}

		public static DualMatrix operator -(DualMatrix a, DualMatrix b)
		{
			int n = a.Rows;
			int m = a.Columns;

			if (b.Rows != n || b.Columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			DualMatrix c = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c[i, j] = a[i, j] - b[i, j];
				}
			}

			return c;
		}

		public static DualMatrix operator -(DualMatrix a, DualNumber t)
		{
			return a + (-t);
		}

		public static DualMatrix operator -(DualNumber t, DualMatrix a)
		{
			int n = a.Rows;
			int m = a.Columns;

			DualMatrix b = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = t - a[i, j];
				}
			}

			return b;
		}

		public static DualMatrix operator *(DualMatrix a, DualMatrix b)
		{
			int n = a.Rows;
			int m = b.Columns;
			int l = a.Columns;

			if (b.Rows != l)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			DualMatrix c = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					DualNumber s = 0.0;
					for (int k = 0; k < l; k++)
					{
						s += a[i, k] * b[k, j];
					}

					c[i, j] = s;
				}
			}

			return c;
		}

		public static DualMatrix operator *(DualMatrix a, DualNumber t)
		{
			int n = a.Rows;
			int m = a.Columns;

			DualMatrix b = new DualMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j] * t;
				}
			}

			return b;
		}

		public static DualMatrix operator *(DualNumber t, DualMatrix a)
		{
			return a * t;
		}

		public static DualMatrix operator /(DualMatrix a, DualNumber t)
		{
			return a * (1.0 / t);
		}

		public DualNumber this[int row, int column]
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
	}
}
