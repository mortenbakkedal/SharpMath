// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Linq;

using SharpMath.LinearAlgebra;

namespace SharpMath
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class Matrix : IEquatable<Matrix>
	{
		private double[,] entries;
		private int hashCode;

		public Matrix(double[,] entries)
		{
			this.entries = (double[,])entries.Clone();
		}

		public Matrix(params double[][] entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException();
			}

			int n = entries.Length;

			if (n == 0)
			{
				this.entries = new double[0, 0];
			}
			else
			{
				if (entries[0] == null)
				{
					throw new ArgumentNullException();
				}

				int m = entries[0].Length;

				this.entries = new double[n, m];
				for (int i = 0; i < n; i++)
				{
					if (entries[i] == null)
					{
						throw new ArgumentNullException();
					}

					if (entries[i].Length != m)
					{
						throw new ArgumentException("Non-matching array dimensions.");
					}

					for (int j = 0; j < m; j++)
					{
						this.entries[i, j] = entries[i][j];
					}
				}
			}
		}

		public Matrix(IEnumerable<IEnumerable<double>> entries)
			: this(entries.Select(r => r.ToArray()).ToArray())
		{
		}

		public Matrix(int rows, int columns)
		{
			if (rows < 0 || columns < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			entries = new double[rows, columns];
		}

		public Matrix(int rows, int columns, double value)
			: this(rows, columns)
		{
			for (int i = 0; i < rows; i++)
			{
				for (int j = 0; j < columns; j++)
				{
					this[i, j] = value;
				}
			}
		}

		public Matrix SetEntry(int row, int column, double value)
		{
			if (row < 0 || row >= Rows || column < 0 || column >= Columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			Matrix a = new Matrix(entries);
			a[row, column] = value;

			return a;
		}

		public Matrix SetMatrix(int row, int column, Matrix subMatrix)
		{
			if (row < 0 || row + subMatrix.Rows > Rows || column < 0 || column + subMatrix.Columns > Columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = subMatrix.Rows;
			int m = subMatrix.Columns;

			Matrix b = new Matrix(entries);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[row + i, column + j] = subMatrix[i, j];
				}
			}

			return b;
		}

		public Matrix GetMatrix(int row, int column, int rows, int columns)
		{
			if (rows < 0 || row < 0 || row + rows > Rows || columns < 0 || column < 0 || column + columns > Columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = rows;
			int m = columns;

			Matrix a = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					a[i, j] = this[row + i, column + j];
				}
			}

			return a;
		}

		public Vector GetRow(int row)
		{
			int n = Columns;

			// Use conversion operator defined in Vector.
			return (Vector)GetMatrix(row, 0, 1, n);
		}

		public Vector GetColumn(int column)
		{
			int n = Rows;

			// Use conversion operator defined in Vector.
			return (Vector)GetMatrix(0, column, n, 1);
		}

		public bool Equals(Matrix other)
		{
			if (object.ReferenceEquals(other, null))
			{
				return false;
			}

			if (object.ReferenceEquals(this, other))
			{
				return true;
			}

			if (GetType() != other.GetType())
			{
				return false;
			}

			if (hashCode != 0 && other.hashCode != 0 && hashCode != other.hashCode)
			{
				// Can't be equal if they have different hash codes.
				return false;
			}

			if (other.Rows != Rows || other.Columns != Columns)
			{
				return false;
			}

			for (int i = 0; i < Rows; i++)
			{
				for (int j = 0; j < Columns; j++)
				{
					if (other[i, j] != this[i, j])
					{
						return false;
					}
				}
			}

			// No differences found.
			return true;
		}

		public override bool Equals(object other)
		{
			return Equals(other as Matrix);
		}

		public override int GetHashCode()
		{
			if (hashCode == 0)
			{
				hashCode = 23;
				unchecked
				{
					for (int i = 0; i < Rows; i++)
					{
						for (int j = 0; j < Columns; j++)
						{
							hashCode = hashCode * 31 + this[i, j].GetHashCode();
						}
					}
				}
			}

			return hashCode;
		}

		public double[,] ToArray()
		{
			return (double[,])entries.Clone();
		}

		public double[][] ToJaggedArray()
		{
			double[][] entries = new double[Rows][];
			for (int i = 0; i < Rows; i++)
			{
				entries[i] = new double[Columns];
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
			StringBuilder sb = new StringBuilder();

			sb.Append("{");
			for (int i = 0; i < Rows; i++)
			{
				if (i > 0)
				{
					sb.Append(", ");
				}
				sb.Append("{");
				for (int j = 0; j < Columns; j++)
				{
					if (j > 0)
					{
						sb.Append(", ");
					}
					sb.Append(this[i, j].ToString(format, CultureInfo.InvariantCulture));
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
			if (rows < 0 || columns < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = Math.Min(rows, columns);

			Matrix a = new Matrix(rows, columns);
			for (int i = 0; i < n; i++)
			{
				a[i, i] = 1.0;
			}

			return a;
		}

		public static Matrix Diagonal(params double[] values)
		{
			if (values == null)
			{
				throw new ArgumentNullException();
			}

			int n = values.Length;

			Matrix a = new Matrix(n, n);
			for (int i = 0; i < n; i++)
			{
				a[i, i] = values[i];
			}

			return a;
		}

		public static Matrix Diagonal(IEnumerable<double> values)
		{
			if (values == null)
			{
				throw new ArgumentNullException();
			}

			return Diagonal(values.ToArray());
		}

		public static Matrix Basis(int rows, int columns, int row, int column)
		{
			if (rows < 0 || row < 0 || row >= rows || columns < 0 || column < 0 || column >= columns)
			{
				throw new ArgumentOutOfRangeException();
			}

			Matrix a = new Matrix(rows, columns);
			a[row, column] = 1.0;

			return a;
		}

		public static Matrix Transpose(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException();
			}

			int n = matrix.Columns;
			int m = matrix.Rows;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = matrix[j, i];
				}
			}

			return b;
		}

		public static double Trace(Matrix matrix)
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

			double s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += matrix[i, i];
			}

			return s;
		}

		/// <summary>
		/// General-purpose matrix inversion through LU decomposition. No exceptions are thrown if the matrix isn't invertible.
		/// </summary>
		public static Matrix Inverse(Matrix matrix)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException();
			}

			int n = matrix.Rows;
			if (matrix.Columns != n)
			{
				throw new ArgumentException("Non-square matrix can't be inverted.");
			}

			Matrix a;
			if (!LUDecomposition.TryInverse(matrix, out a))
			{
				// Return a matrix with NaN entries. This is consistent with the static methods in the Math class.
				return new Matrix(n, n, double.NaN);
			}

			return a;
		}

		/// <summary>
		/// General-purpose matrix determinant through LU decomposition.
		/// </summary>
		public static double Determinant(Matrix matrix)
		{
			return LUDecomposition.Determinant(matrix);
		}

		public static implicit operator Matrix(Vector a)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Length;

			Matrix b = new Matrix(n, 1);
			for (int i = 0; i < n; i++)
			{
				b[i, 0] = a[i];
			}

			return b;
		}

		public static Matrix operator +(Matrix a, Matrix b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = a.Columns;

			if (b.Rows != n || b.Columns != m)
			{
				throw new ArgumentException("Non-matching matrix dimensions.");
			}

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c[i, j] = a[i, j] + b[i, j];
				}
			}

			return c;
		}

		public static Matrix operator +(Matrix a, double t)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = a.Columns;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j] + t;
				}
			}

			return b;
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
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = a.Columns;

			if (b.Rows != n || b.Columns != m)
			{
				throw new ArgumentException("Non-matching matrix dimensions.");
			}

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					c[i, j] = a[i, j] - b[i, j];
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
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = a.Columns;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = t - a[i, j];
				}
			}

			return b;
		}

		public static Matrix operator *(Matrix a, Matrix b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = b.Columns;
			int l = a.Columns;

			if (b.Rows != l)
			{
				throw new ArgumentException("Non-matching matrix dimensions.");
			}

			Matrix c = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					double s = 0.0;
					for (int k = 0; k < l; k++)
					{
						s += a[i, k] * b[k, j];
					}

					c[i, j] = s;
				}
			}

			return c;
		}

		public static Matrix operator *(Matrix a, double t)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = a.Columns;

			Matrix b = new Matrix(n, m);
			for (int i = 0; i < n; i++)
			{
				for (int j = 0; j < m; j++)
				{
					b[i, j] = a[i, j] * t;
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

		public static bool operator ==(Matrix a, Matrix b)
		{
			if (object.ReferenceEquals(a, null))
			{
				if (object.ReferenceEquals(b, null))
				{
					return true;
				}

				return false;
			}

			return a.Equals(b);
		}

		public static bool operator !=(Matrix a, Matrix b)
		{
			return !(a == b);
		}

		public double this[int row, int column]
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
