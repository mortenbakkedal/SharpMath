using System;
using System.Collections.Generic;

namespace SharpMath.Samples.Optimization
{
	[Serializable]
	public sealed class Vector //: IEnumerable<double>
	{
		private double[] entries;

		private Vector()
		{
		}

		public Vector(params double[] entries)
		{
		}

		public Vector(IEnumerable<double> entries)
		{
		}

		public static implicit operator Matrix(Vector a)
		{
			throw new NotImplementedException();
		}

		public static explicit operator Vector(Matrix a)
		{
			throw new NotImplementedException();
		}

		/*public static Vector operator +(Vector a, Vector b)
		{
			int rows, columns;
			Vector r = new Vector();
			Matrix.Add(a.Length, 1, a.entries, b.Length, 1, b.entries, out rows, out columns, out r.entries);

			return r;
		}*/

		public double this[int index]
		{
			get
			{
				// No explicit bounds checking required.
				return entries[index];
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
