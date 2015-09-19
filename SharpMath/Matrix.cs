// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;

using SharpMath.LinearAlgebra;

namespace SharpMath
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class Matrix
	{
		private int rows, columns;
		private double[] entries;

		public Matrix(double[,] values)
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

		private Matrix(int rows, int columns)
		{
			// This constructor is intentionally kept private. Use Zero instead.

			this.rows = rows;
			this.columns = columns;

			entries = new double[rows * columns];
		}

		private Matrix(int rows, int columns, double[] entries)
		{
			this.rows = rows;
			this.columns = columns;
			this.entries = (double[])entries.Clone();
		}

		private void SetEntryInternal(int row, int column, double t)
		{
			entries[row + column * rows] = t;
		}

		private double GetEntryInternal(int row, int column)
		{
			return entries[row + column * rows];
		}

		public Matrix SetEntry(int row, int column, double t)
		{
			if (row < 0 || row >= rows || column < 0 || column >= columns)
			{
				throw new IndexOutOfRangeException();
			}

			Matrix a = new Matrix(rows, columns, entries);
			a.SetEntryInternal(row, column, t);

			return a;
		}

		public Matrix SetMatrix(int row, int column, Matrix a)
		{
			if (row < 0 || row + a.rows > rows || column < 0 || column + a.columns > columns)
			{
				throw new IndexOutOfRangeException();
			}

			Matrix b = new Matrix(rows, columns, entries);
			for (int i = 0; i < a.rows; i++)
			{
				for (int j = 0; j < a.columns; j++)
				{
					b.SetEntryInternal(row + i, column + j, a.GetEntryInternal(i, j));
				}
			}

			return b;
		}

		public Matrix GetMatrix(int row, int column, int rows, int columns)
		{
			if (row < 0 || row + rows > this.rows || column < 0 || column + columns > this.columns)
			{
				throw new IndexOutOfRangeException();
			}

			int n = rows;
			int m = columns;

			Matrix a = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					a.SetEntryInternal(i, j, GetEntryInternal(row + i, column + j));
				}
			}

			return a;
		}

		public Matrix GetRow(int row)
		{
			int n = columns;

			return GetMatrix(row, 0, 1, n);
		}

		public Vector GetColumn(int column)
		{
			int n = rows;

			// Use conversion operator defined in Vector.
			return (Vector)(GetMatrix(0, column, n, 1));
		}

		public double[,] ToArray()
		{
			double[,] values = new double[rows, columns];
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					values[i, j] = GetEntryInternal(i, j);
				}
			}

			return values;
		}

		public double[] ToLinearArray()
		{
			return (double[])entries.Clone();
		}

		public double[][] ToJaggedArray()
		{
			double[][] values = new double[rows][];
			for (int i = 0; i < rows; i++)
			{
				values[i] = new double[columns];
				for (int j = 0; j < columns; j++)
				{
					values[i][j] = GetEntryInternal(i, j);
				}
			}
			return values;
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
					sb.Append(GetEntryInternal(i, j).ToString(format, CultureInfo.InvariantCulture));
				}
				sb.Append("}");
			}
			sb.Append("}");

			return sb.ToString();
		}

		public static Matrix Zero(int rows, int columns)
		{
			return new Matrix(rows, columns);
		}

		public static Matrix Identity(int rows, int columns)
		{
			int n = Math.Min(rows, columns);

			Matrix a = new Matrix(rows, columns);
			for (int i = 0; i < n; i++)
			{
				a.SetEntryInternal(i, i, 1.0);
			}

			return a;
		}

		public static Matrix Diagonal(double[] values)
		{
			int n = values.Length;

			Matrix a = new Matrix(n, n);
			for (int i = 0; i < n; i++)
			{
				a.SetEntryInternal(i, i, values[i]);
			}

			return a;
		}

		public static Matrix Basis(int rows, int columns, int row, int column)
		{
			if (row < 0 || row >= rows || column < 0 || column >= columns)
			{
				throw new IndexOutOfRangeException();
			}

			Matrix a = new Matrix(rows, columns);
			a.SetEntryInternal(row, column, 1.0);

			return a;
		}

		public static Matrix Transpose(Matrix a)
		{
			int n = a.columns;
			int m = a.rows;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, a.GetEntryInternal(j, i));
				}
			}

			return b;
		}

		public static double Trace(Matrix a)
		{
			int n = a.rows;

			if (a.columns != n)
			{
				throw new ArgumentException("The matrix isn't a square matrix.");
			}

			double s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += a[i, i];
			}

			return s;
		}

		/// <summary>
		/// General-purpose matrix inversion through LU decomposition. No exceptions are thrown if the matrix isn't invertible.
		/// </summary>
		public static Matrix Inverse(Matrix a)
		{
			int n = a.Rows;
			if (a.Columns != n)
			{
				throw new ArgumentException("The matrix is not a square matrix.");
			}

			Matrix b;
			if (!LUDecomposition.TryInverse(a, out b))
			{
				// Return a matrix with NaN entries.
				b = Matrix.Zero(n, n) + double.NaN;
			}

			return b;
		}

		/// <summary>
		/// General-purpose matrix determinant through LU decomposition.
		/// </summary>
		public static double Determinant(Matrix a)
		{
			return LUDecomposition.Determinant(a);
		}

		public static Matrix operator +(Matrix a, Matrix b)
		{
			int n = a.Rows;
			int m = a.Columns;

			if (b.rows != n || b.columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c.SetEntryInternal(i, j, a.GetEntryInternal(i, j) + b.GetEntryInternal(i, j));
				}
			}

			return c;
		}

		public static Matrix operator +(Matrix a, double t)
		{
			int n = a.Rows;
			int m = a.Columns;

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c.SetEntryInternal(i, j, a.GetEntryInternal(i, j) + t);
				}
			}

			return c;
		}

		public static Matrix operator +(double t, Matrix a)
		{
			return a + t;
		}

		public static Matrix operator -(Matrix a)
		{
			return a * -1.0;
		}

		public static Matrix operator -(Matrix a, Matrix b)
		{
			int n = a.rows;
			int m = a.columns;

			if (b.rows != n || b.columns != m)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c.SetEntryInternal(i, j, a.GetEntryInternal(i, j) - b.GetEntryInternal(i, j));
				}
			}

			return c;
		}

		public static Matrix operator -(Matrix a, double t)
		{
			return a + (-t);
		}

		public static Matrix operator -(double t, Matrix a)
		{
			int n = a.rows;
			int m = a.columns;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, t - a.GetEntryInternal(i, j));
				}
			}

			return b;
		}

		public static Matrix operator *(Matrix a, Matrix b)
		{
			int n = a.rows;
			int m = b.columns;
			int l = a.columns;

			if (b.rows != l)
			{
				throw new ArgumentException("The matrix dimensions don't match.");
			}

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					double s = 0.0;
					for (int k = 0; k < l; k++)
					{
						s += a.GetEntryInternal(i, k) * b.GetEntryInternal(k, j);
					}

					c.SetEntryInternal(i, j, s);
				}
			}

			return c;
		}

		public static Matrix operator *(Matrix a, double t)
		{
			int n = a.rows;
			int m = a.columns;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b.SetEntryInternal(i, j, a.GetEntryInternal(i, j) * t);
				}
			}

			return b;
		}

		public static Matrix operator *(double t, Matrix a)
		{
			return a * t;
		}

		public static Matrix operator /(Matrix a, double t)
		{
			return a * (1.0 / t);
		}

		public double this[int row, int column]
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
	}
}
