// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.LinearAlgebra
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{DebuggerDisplay}")]
	public sealed class ComplexVector
	{
		private ComplexMatrix inner;

		internal ComplexVector(ComplexMatrix inner)
		{
			if (inner.Columns != 1)
			{
				throw new ArgumentException();
			}

			this.inner = inner;
		}

		public ComplexVector(Complex[] entries)
		{
			int n = entries.Length;

			Complex[,] a = new Complex[n, 1];
			for (int i = 0; i < n; i++)
			{
				a[i, 0] = entries[i];
			}

			inner = new ComplexMatrix(a);
		}

		public ComplexVector SetValue(int index, Complex t)
		{
			return new ComplexVector(inner.SetEntry(index, 0, t));
		}

		public ComplexVector SetVector(int index, ComplexVector a)
		{
			return new ComplexVector(inner.SetMatrix(index, 0, a.inner));
		}

		public ComplexVector GetVector(int index, int length)
		{
			return new ComplexVector(inner.GetMatrix(index, 0, length, 1));
		}

		public Complex[] ToArray()
		{
			return inner.ToLinearArray();
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public string ToString(string format)
		{
			//string s = ComplexMatrix.Transpose(inner).ToString(format);
			//return s.Substring(1, s.Length - 2);
			return inner.ToString(format);
		}

		public static ComplexVector Zero(int length)
		{
			return new ComplexVector(ComplexMatrix.Zero(length, 1));
		}

		public static ComplexVector Basis(int length, int index)
		{
			return new ComplexVector(ComplexMatrix.Basis(length, 1, index, 0));
		}

		public static Complex Dot(ComplexVector a, ComplexVector b)
		{
			int n = a.Length;
			if (n != b.Length)
			{
				throw new ArgumentException("Dot product undefined. Size mismatch.");
			}

			Complex s = 0.0;
			for (int i = 0; i < n; i++)
			{
				s += a[i] * b[i];
			}

			return s;
		}

		public static implicit operator ComplexMatrix(ComplexVector a)
		{
			return a.inner;
		}

		public static explicit operator ComplexVector(ComplexMatrix a)
		{
			if (a.Columns == 1)
			{
				return new ComplexVector(a);
			}

			if (a.Rows == 1)
			{
				// Some copying overhead here.
				return new ComplexVector(ComplexMatrix.Transpose(a));
			}

			throw new InvalidCastException("The matrix has no vector representation.");
		}

		public static implicit operator ComplexVector(Vector vector)
		{
			return new ComplexVector((ComplexMatrix)(Matrix)vector);
		}

		public static ComplexVector operator +(ComplexVector a, ComplexVector b)
		{
			return new ComplexVector(a.inner + b.inner);
		}

		public static ComplexVector operator +(ComplexVector a, Complex t)
		{
			return new ComplexVector(a.inner + t);
		}

		public static ComplexVector operator +(Complex t, ComplexVector a)
		{
			return a + t;
		}

		public static ComplexVector operator -(ComplexVector a)
		{
			return a * -1.0;
		}

		public static ComplexVector operator -(ComplexVector a, ComplexVector b)
		{
			return new ComplexVector(a.inner - b.inner);
		}

		public static ComplexVector operator -(ComplexVector a, Complex t)
		{
			return a + (-t);
		}

		public static ComplexVector operator -(Complex t, ComplexVector a)
		{
			return new ComplexVector(t - a.inner);
		}

		public static ComplexVector operator *(ComplexMatrix a, ComplexVector b)
		{
			return new ComplexVector(a * b.inner);
		}

		public static ComplexVector operator *(Matrix a, ComplexVector b)
		{
			return new ComplexVector(a * b.inner);
		}

		public static ComplexVector operator *(ComplexVector a, Complex t)
		{
			return new ComplexVector(a.inner * t);
		}

		public static ComplexVector operator *(Complex t, ComplexVector a)
		{
			return a * t;
		}

		public static ComplexVector operator /(ComplexVector a, Complex t)
		{
			return a * (1.0 / t);
		}

		public Complex this[int index]
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
