// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.Functions
{
	[Serializable]
	public class CubicSplineInterpolationFunction : IPlotFunction
	{
		// http://en.wikipedia.org/wiki/Spline_interpolation
		// http://www.cis.uab.edu/courses/cs470/fall2007/Interpolation/java/SplInt.java
		// http://apps.nrbook.com/c/index.html, Section 3.3
		// http://www.cse.unsw.edu.au/~lambert/splines/NatCubic.java

		private double[] xa, ya, y2a;
		private int n;

		private CubicSplineInterpolationFunction(double[] xa, double[] ya, double[] y2a, int n)
		{
			this.xa = xa;
			this.ya = ya;
			this.y2a = y2a;
			this.n = n;
		}

		public static CubicSplineInterpolationFunction CreateCubicSpline(double[] x, double[] y, double yp1, double ypn)
		{
			int n = x.Length;
			
			if (y.Length != n)
			{
				throw new ArgumentException();
			}

			for (int i = 0; i < n - 1; i++)
			{
				if (x[i] >= x[i + 1])
				{
					throw new ArgumentException();
				}
			}

			double[] y2 = new double[n];
			Spline(x, y, n, yp1, ypn, y2);

			return new CubicSplineInterpolationFunction((double[])x.Clone(), (double[])y.Clone(), y2, n);
		}

		public static CubicSplineInterpolationFunction CreateNaturalCubicSpline(double[] x, double[] y)
		{
			return CreateCubicSpline(x, y, NATURAL, NATURAL);
		}

		public double Value(double x)
		{
			double y;
			SplInt(xa, ya, y2a, n, x, out y);

			return y;
		}

		private static double NATURAL = 1.0e30;

		/// <summary>
		/// Given arrays X and Y of length N containing a tabulated function, i.e.
		/// Y[I] = f(X[I]), with X[0]<X[1]<...<X[N-1], and given values YP1 and YPN,
		/// for the first derivative of the interpolating function at points 0 and N-1,
		/// respectively, this routine returns an array Y2 of length N which contains
		/// the second derivatives of the interpolating function at the tabulated points
		/// X[I].  If YP1 and/or YPN are equal to 1x10^30 or larger, the routine is
		/// signalled to set the corresponding boundary condition for a natural spline,
		/// with zero second derivative on that boundary.
		/// </summary>
		private static void Spline(double[] X, double[] Y, int N, double YP1, double YPN, double[] Y2)
		{
			double[] U = new double[N];
			double SIG, P, UN, QN;
			int I, K;

			if (YP1 >= NATURAL)
			{
				Y2[0] = 0.0;
				U[0] = 0.0;
			}
			else
			{
				Y2[0] = -0.5;
				U[0] = (3.0 / (X[1] - X[0])) * ((Y[1] - Y[0]) / (X[1] - X[0]) - YP1);
			}

			if (YPN >= NATURAL)
			{
				QN = 0.0;
				UN = 0.0;
			}
			else
			{
				QN = 0.5;
				UN = (3.0 / (X[N - 1] - X[N - 2])) * (YPN - (Y[N - 1] - Y[N - 2]) / (X[N - 1] - X[N - 2]));
			}
   
			// The decomposition loop of the tridiagonal algorithm.  Y2 and U
	        // are used to store the decomposed factors.
			for (I=1; I < (N-1); I++)
			{
				SIG = (X[I] - X[I - 1]) / (X[I + 1] - X[I - 1]);
				P = SIG * Y2[I - 1] + 2.0;
				Y2[I] = (SIG - 1.0) / P;
				U[I] = (6.0 * ((Y[I + 1] - Y[I]) / (X[I + 1] - X[I])
					- (Y[I] - Y[I - 1]) / (X[I] - X[I - 1]))
					/ (X[I + 1] - X[I - 1])
					- SIG * U[I - 1]) / P; 
			}
  
			Y2[N - 1] = (UN - QN * U[N - 2]) / (QN * Y2[N - 2] + 1.0);

			// Backsubstitution loop of the tridiagonal algorithm.
			for (K=N-2; K>=0; K--)
			{
				Y2[K] = Y2[K] * Y2[K + 1] + U[K];
			}  
		}

		/// <summary>
		/// Given the arrays XA and YA of length N, which tabulate a function
		/// (with the XA's in increasing order), and given the array Y2A, which 
		/// is the output from Spline above, and given a value of X,
		/// this routine returns a cubic-spline interpolated value Y.
		/// </summary>
		private static void SplInt(double[] XA, double[] YA, double[] Y2A, int N, double X, out double Y)
		{
			int KLO, KHI, K;
			double H, A, B;

			/* use bisection to find the right interval */
			for (KLO=0,KHI=N-1; (KHI-KLO)>1;)
			{
				K = (KHI + KLO) / 2;
				if (XA[K] > X)
					KHI = K;
				else
					KLO = K;
			} 
			H = XA[KHI] - XA[KLO];
			if (0.0 == H)
			{
				throw new Exception("SplInt: bad XA");
			}
			A = (XA[KHI] - X) / H;
			B = (X - XA[KLO]) / H;
			Y = A * YA[KLO] + B * YA[KHI]
				+ ((A * A * A - A) * Y2A[KLO] + (B * B * B - B) * Y2A[KHI]) * (H * H) / 6.0;
		}
	}
}
