// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;
using System.Text;

namespace SharpMath.LinearAlgebra
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{DebuggerDisplay}")]
	public sealed class ComplexMatrix
	{
		private int rows, columns;
		private Complex[] entries;

		public ComplexMatrix(Complex[,] values)
			: this(values.GetLength(0), values.GetLength(1))
		{
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					SetEntryInternal(i, j, values[i, j]);
				}
			}
		}

		private ComplexMatrix(int rows, int columns)
		{
			// This constructor is intentionally kept private. Use Zero instead.

			this.rows = rows;
			this.columns = columns;

			entries = new Complex[rows * columns];
		}

		private ComplexMatrix(int rows, int columns, Complex[] entries)
		{
			this.rows = rows;
			this.columns = columns;
			this.entries = (Complex[])entries.Clone();
		}

		private void SetEntryInternal(int row, int column, Complex t)
		{
			entries[row + column * rows] = t;
		}

		private Complex GetEntryInternal(int row, int column)
		{
			return entries[row + column * rows];
		}

		public ComplexMatrix SetEntry(int row, int column, Complex t)
		{
			if (row < 0 || row >= rows || column < 0 || column >= columns)
			{
				throw new IndexOutOfRangeException();
			}

			ComplexMatrix a = new ComplexMatrix(rows, columns, entries);
			a.SetEntryInternal(row, column, t);
			return a;
		}

		public ComplexMatrix SetMatrix(int row, int column, ComplexMatrix a)
		{
			if (row < 0 || row + a.rows > rows || column < 0 || column + a.columns > columns)
			{
				throw new IndexOutOfRangeException();
			}

			ComplexMatrix b = new ComplexMatrix(rows, columns, entries);
			for (int i = 0; i < a.rows; i++)
			{
				for (int j = 0; j < a.columns; j++)
				{
					b.SetEntryInternal(row + i, column + j, a.GetEntryInternal(i, j));
				}
			}

			return b;
		}

		public ComplexMatrix GetMatrix(int row, int column, int rows, int columns)
		{
			if (row < 0 || row + rows > this.rows || column < 0 || column + columns > this.columns)
			{
				throw new IndexOutOfRangeException();
			}

			int n = rows;
			int m = columns;

			ComplexMatrix a = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					a.SetEntryInternal(i, j, GetEntryInternal(row + i, column + j));
				}
			}

			return a;
		}

		public ComplexMatrix GetRow(int row)
		{
			int n = columns;

			return GetMatrix(row, 0, 1, n);
		}

		public ComplexVector GetColumn(int column)
		{
			int n = rows;

			// Use conversion operator defined in Vector.
			return (ComplexVector)(GetMatrix(0, column, n, 1));
		}

		public Complex[,] ToArray()
		{
			Complex[,] values = new Complex[rows, columns];
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					values[i, j] = GetEntryInternal(i, j);
				}
			}

			return values;
		}

		public Complex[] ToLinearArray()
		{
			return (Complex[])entries.Clone();
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public string ToString(string format)
		{
			StringBuilder sb = new StringBuilder();

			sb.Append("{");
			for (int i = 0; i < rows; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				sb.Append("{");
				for (int j = 0; j < columns; j++)
				{
					if (j > 0)
					{
						sb.Append(", ");
					}
					sb.Append(GetEntryInternal(i, j).ToString());
				}
				sb.Append("}");
			}
			sb.Append("}");

			return sb.ToString();
		}

		public static ComplexMatrix Zero(int rows, int columns)
		{
			return new ComplexMatrix(rows, columns);
		}

		public static ComplexMatrix Identity(int rows, int columns)
		{
			int n = Math.Min(rows, columns);

			ComplexMatrix a = new ComplexMatrix(rows, columns);
			for (int i = 0; i < n; i++)
			{
				a.SetEntryInternal(i, i, 1.0);
			}

			return a;
		}

		public static ComplexMatrix Diagonal(Complex[] values)
		{
			int n = values.Length;

			ComplexMatrix a = new ComplexMatrix(n, n);
			for (int i = 0; i < n; i++)
			{
				a.SetEntryInternal(i, i, values[i]);
			}

			return a;
		}

		public static ComplexMatrix Basis(int rows, int columns, int row, int column)
		{
			if (row < 0 || row >= rows || column < 0 || column >= columns)
			{
				throw new IndexOutOfRangeException();
			}

			ComplexMatrix a = new ComplexMatrix(rows, columns);
			a.SetEntryInternal(row, column, 1.0);
			return a;
		}

		public static ComplexMatrix Transpose(ComplexMatrix a)
		{
			int n = a.columns;
			int m = a.rows;

			ComplexMatrix b = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, a.GetEntryInternal(j, i));
				}
			}

			return b;
		}

		public static Complex Trace(ComplexMatrix a)
		{
			int n = Math.Min(a.rows, a.columns);

			Complex s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += a[i, i];
			}

			return s;
		}

		public static implicit operator ComplexMatrix(Matrix a)
		{
			int n = a.Rows;
			int m = a.Columns;

			ComplexMatrix b = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, a[i, j]);
				}
			}

			return b;
		}

		public static ComplexMatrix operator +(ComplexMatrix a, ComplexMatrix b)
		{
			int n = a.Rows;
			int m = a.Columns;

			if (b.rows != n || b.columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			ComplexMatrix c = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c.SetEntryInternal(i, j, a.GetEntryInternal(i, j) + b.GetEntryInternal(i, j));
				}
			}

			return c;
		}

		public static ComplexMatrix operator +(ComplexMatrix a, Matrix b)
		{
			return a + (ComplexMatrix)b;
		}

		public static ComplexMatrix operator +(Matrix a, ComplexMatrix b)
		{
			return (ComplexMatrix)a + b;
		}

		public static ComplexMatrix operator +(ComplexMatrix a, Complex t)
		{
			int n = a.Rows;
			int m = a.Columns;

			ComplexMatrix c = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c.SetEntryInternal(i, j, a.GetEntryInternal(i, j) + t);
				}
			}

			return c;
		}

		public static ComplexMatrix operator +(Complex t, ComplexMatrix a)
		{
			return a + t;
		}

		public static ComplexMatrix operator -(ComplexMatrix a)
		{
			return a * -1.0;
		}

		public static ComplexMatrix operator -(ComplexMatrix a, ComplexMatrix b)
		{
			int n = a.rows;
			int m = a.columns;

			if (b.rows != n || b.columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			ComplexMatrix c = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c.SetEntryInternal(i, j, a.GetEntryInternal(i, j) - b.GetEntryInternal(i, j));
				}
			}

			return c;
		}

		public static ComplexMatrix operator -(ComplexMatrix a, Complex t)
		{
			return a + (-t);
		}

		public static ComplexMatrix operator -(Complex t, ComplexMatrix a)
		{
			int n = a.rows;
			int m = a.columns;

			ComplexMatrix b = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, t - a.GetEntryInternal(i, j));
				}
			}

			return b;
		}

		public static ComplexMatrix operator *(ComplexMatrix a, ComplexMatrix b)
		{
			int n = a.rows;
			int m = b.columns;
			int l = a.columns;

			if (b.rows != l)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			ComplexMatrix c = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					Complex s = 0.0;
					for (int k = 0; k < l; k++)
					{
						s += a.GetEntryInternal(i, k) * b.GetEntryInternal(k, j);
					}

					c.SetEntryInternal(i, j, s);
				}
			}

			return c;
		}

		public static ComplexMatrix operator *(Matrix a, ComplexMatrix b)
		{
			return (ComplexMatrix)a * b;
		}

		public static ComplexMatrix operator *(ComplexMatrix a, Complex t)
		{
			int n = a.rows;
			int m = a.columns;

			ComplexMatrix b = new ComplexMatrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, a.GetEntryInternal(i, j) * t);
				}
			}

			return b;
		}

		public static ComplexMatrix operator *(Complex t, ComplexMatrix a)
		{
			return a * t;
		}

		public static ComplexMatrix operator /(ComplexMatrix a, Complex t)
		{
			return a * (1.0 / t);
		}

		public Complex this[int row, int column]
		{
			get
			{
				if (row < 0 || row >= rows || column < 0 || column >= columns)
				{
					throw new IndexOutOfRangeException();
				}

				return GetEntryInternal(row, column);
			}
		}

		public int Rows
		{
			get
			{
				return rows;
			}
		}

		public int Columns
		{
			get
			{
				return columns;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				return ToString();
			}
		}
	}
}
