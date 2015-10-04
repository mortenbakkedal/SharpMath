using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpMath.Samples.Optimization
{
	[Serializable]
	public sealed class Matrix
	{
		private double[] entries;

		private Matrix()
		{
		}

		/*public static void Add(int rows1, int columns1, double[] entries1, int rows2, int colums2, double[] entries2, out int rows, out int columns, out double[] entriesResult)
		{
			//if (...)
			throw new ArgumentException("Dimension mismatch.");

			rows = 123;
			columns = 123;
			entriesResult = null;
			entriesResult[0] = 12313;
		}
		public static void Multiply(int rows1, int columns1, double[] entries1, int rows2, int colums2, double[] entries2, double[] entriesResult)
		{
			//if (...)
			throw new ArgumentException("Dimension mismatch.");

			int rows = 123;
			int columns = 123;
			entriesResult[0] = 12313;
		}


		public static Vector operator *(Matrix a, Vector b)
		{
			int rows, columns;
			Matrix r = new Matrix();
			Matrix.Add(a.Length, 1, a.entries, b.Length, 1, b.entries, out rows, out columns, out r.entries);

			return r;
		}*/
	}
}
