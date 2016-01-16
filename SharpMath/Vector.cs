// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

namespace SharpMath
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{ToString(),nq}")]
	public sealed class Vector : IEnumerable<double>, IEquatable<Vector>
	{
		private double[] entries;
		private int hashCode;

		public Vector(params double[] entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException();
			}

			this.entries = (double[])entries.Clone();
		}

		public Vector(IEnumerable<double> entries)
		{
			if (entries == null)
			{
				throw new ArgumentNullException();
			}

			this.entries = entries.ToArray();
		}

		public Vector(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException();
			}

			entries = new double[length];
		}

		public Vector(int length, double t)
			: this(length)
		{
			for (int i = 0; i < length; i++)
			{
				this[i] = t;
			}
		}

		public Vector SetEntry(int index, double t)
		{
			if (index < 0 || index >= Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			Vector a = new Vector(entries);
			a[index] = t;

			return a;
		}

		public Vector SetVector(int index, Vector a)
		{
			if (index < 0 || index + a.Length > Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			Vector b = new Vector(entries);
			for (int i = 0; i < a.Length; i++)
			{
				b[index + i] = a[i];
			}

			return b;
		}

		public Vector GetVector(int index, int length)
		{
			if (length < 0 || index < 0 || index + length > Length)
			{
				throw new ArgumentOutOfRangeException();
			}

			int n = length;

			Vector a = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				a[i] = this[index + i];
			}

			return a;
		}

		public bool Equals(Vector other)
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

			if (other.Length != Length)
			{
				return false;
			}

			for (int i = 0; i < Length; i++)
			{
				if (other[i] != this[i])
				{
					return false;
				}
			}

			// No differences found.
			return true;
		}

		public override bool Equals(object other)
		{
			return Equals(other as Vector);
		}

		public override int GetHashCode()
		{
			if (hashCode == 0)
			{
				hashCode = 23;
				unchecked
				{
					for (int i = 0; i < Length; i++)
					{
						hashCode = hashCode * 31 + this[i].GetHashCode();
					}
				}
			}

			return hashCode;
		}

		public IEnumerator<double> GetEnumerator()
		{
			return ((IEnumerable<double>)entries).GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
        }

		public double[] ToArray()
		{
			return (double[])entries.Clone();
		}

		public override string ToString()
		{
			return ToString(null);
		}

		public string ToString(string format)
		{
			return "{ " + string.Join(", ", entries.Select(e => e.ToString(format, CultureInfo.InvariantCulture))) + " }";
		}

		public static Vector Zero(int length)
		{
			return new Vector(length);
		}

		public static Vector Basis(int length, int index)
		{
			if (length < 0 || index < 0 || index >= length)
			{
				throw new ArgumentOutOfRangeException();
			}

			Vector a = new Vector(length);
			a[index] = 1.0;

			return a;
		}

		public static explicit operator Vector(Matrix a)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			if (a.Columns == 1)
			{
				int n = a.Rows;

				Vector b = new Vector(n);
				for (int i = 0; i < n; i++)
				{
					b[i] = a[i, 0];
				}

				return b;
			}

			if (a.Rows == 1)
			{
				int n = a.Columns;

				Vector b = new Vector(n);
				for (int i = 0; i < n; i++)
				{
					b[i] = a[0, i];
				}

				return b;
			}

			throw new InvalidCastException("No vector representation of the matrix.");
		}

		public static Vector operator +(Vector a, Vector b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Length;
			if (b.Length != n)
			{
				throw new ArgumentException("Non-matching vector lengths.");
			}

			Vector c = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				c[i] = a[i] + b[i];
			}

			return c;
		}

		public static Vector operator +(Vector a, double t)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Length;

			Vector b = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				b[i] = a[i] + t;
			}

			return b;
		}

		public static Vector operator +(double t, Vector a)
		{
			return a + t;
		}

		public static Vector operator -(Vector a)
		{
			return a * -1.0;
		}

		public static Vector operator -(Vector a, Vector b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Length;

			if (b.Length != n)
			{
				throw new ArgumentException("Non-matching vector lengths.");
			}

			Vector c = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				c[i] = a[i] - b[i];
			}

			return c;

		}

		public static Vector operator -(Vector a, double t)
		{
			return a + (-t);
		}

		public static Vector operator -(double t, Vector a)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Length;

			Vector b = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				b[i] = t - a[i];
			}

			return b;

		}

		public static Vector operator *(Matrix a, Vector b)
		{
			if (a == null || b == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Rows;
			int m = a.Columns;

			if (b.Length != m)
			{
				throw new ArgumentException("Non-matching matrix dimensions and vector length.");
			}

			Vector c = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				double s = 0.0;
				for (int j = 0; j < m; j++)
				{
					s += a[i, j] * b[j];
				}

				c[i] = s;
			}

			return c;
		}

		public static Vector operator *(Vector a, double t)
		{
			if (a == null)
			{
				throw new ArgumentNullException();
			}

			int n = a.Length;

			Vector b = new Vector(n);
			for (int i = 0; i < n; i++)
			{
				b[i] = a[i] * t;
			}

			return b;
		}

		public static Vector operator *(double t, Vector a)
		{
			return a * t;
		}

		public static Vector operator /(Vector a, double t)
		{
			return a * (1.0 / t);
		}

		public static bool operator ==(Vector a, Vector b)
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

		public static bool operator !=(Vector a, Vector b)
		{
			return !(a == b);
		}

		public double this[int index]
		{
			get
			{
				return entries[index];
			}
			private set
			{
				// This property is kept private. The class is immutable by design.
				entries[index] = value;
			}
		}

		public int Length
		{
			get
			{
				return entries.Length;
			}
		}
	}
}
