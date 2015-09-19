// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath
{
	[Serializable]
	[DebuggerStepThrough]
	public sealed class Vector
	{
		private Matrix inner;

		private Vector(Matrix inner)
		{
			if (inner.Columns != 1)
			{
				throw new ArgumentException();
			}

			this.inner = inner;
		}

		public Vector(double[] entries)
		{
			int n = entries.Length;

			double[,] a = new double[n, 1];
			for (int i = 0; i < n; i++)
			{
				a[i, 0] = entries[i];
			}

			inner = new Matrix(a);
		}

		public Vector SetEntry(int index, double t)
		{
			return new Vector(inner.SetEntry(index, 0, t));
		}

		public Vector SetVector(int index, Vector a)
		{
			return new Vector(inner.SetMatrix(index, 0, a.inner));
		}

		public Vector GetVector(int index, int length)
		{
			return new Vector(inner.GetMatrix(index, 0, length, 1));
		}

		public double[] ToArray()
		{
			return inner.ToLinearArray();
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public string ToString(string format)
		{
			//string s = Matrix.Transpose(inner).ToString(format);
			//return s.Substring(1, s.Length - 2);
			return inner.ToString(format);
		}

		public static Vector Zero(int length)
		{
			return new Vector(Matrix.Zero(length, 1));
		}

		public static Vector Basis(int length, int index)
		{
			return new Vector(Matrix.Basis(length, 1, index, 0));
		}

		public static implicit operator Matrix(Vector a)
		{
			return a.inner;
		}

		public static explicit operator Vector(Matrix a)
		{
			if (a.Columns == 1)
			{
				return new Vector(a);
			}

			if (a.Rows == 1)
			{
				// Some copying overhead here.
				return new Vector(Matrix.Transpose(a));
			}

			throw new InvalidCastException("The matrix has no vector representation.");
		}

		public static Vector operator +(Vector a, Vector b)
		{
			return new Vector(a.inner + b.inner);
		}

		public static Vector operator +(Vector a, double t)
		{
			return new Vector(a.inner + t);
		}

		public static Vector operator +(double t, Vector a)
		{
			return new Vector(t + a.inner);
		}

		public static Vector operator -(Vector a)
		{
			return new Vector(-a.inner);
		}

		public static Vector operator -(Vector a, Vector b)
		{
			return new Vector(a.inner - b.inner);
		}

		public static Vector operator -(Vector a, double t)
		{
			return new Vector(a.inner - t);
		}

		public static Vector operator -(double t, Vector a)
		{
			return new Vector(t - a.inner);
		}

		public static Vector operator *(Matrix a, Vector b)
		{
			return new Vector(a * b.inner);
		}

		public static Vector operator *(Vector a, double t)
		{
			return new Vector(a.inner * t);
		}

		public static Vector operator *(double t, Vector a)
		{
			return new Vector(t * a.inner);
		}

		public static Vector operator /(Vector a, double t)
		{
			return new Vector(a.inner / t);
		}

		public double this[int index]
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
