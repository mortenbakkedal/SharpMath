using System;

namespace SharpMath.Statistics.RandomNumbers
{
	public class RngStream : IUniformStreamGenerator
	{
		private double norm, a12, a13n, a21, a23n, two17, two53;
		private double[,] a1p0inv, a2p0inv, a1p0, a2p0, a1p76, a2p76, a1p127, a2p127;
		private double[] Cg, Bg, Ig;

		private const double m1 = 4294967087.0;
		private const double m2 = 4294944443.0;
		private const double two24inv = 5.9604644775390625e-8;

		private static int c_id = 1;
		private int id = c_id++;

		public RngStream()
			: this(new uint[] { 12345, 12345, 12345, 12345, 12345, 12345 })
		{
		}

		public RngStream(uint[] seed)
		{
			norm = 2.328306549295727688e-10;
			//m1 = 4294967087.0;
			//m2 = 4294944443.0;
			a12 = 1403580.0;
			a13n = 810728.0;
			a21 = 527612.0;
			a23n = 1370589.0;
			two17 = 131072.0;
			two53 = 9007199254740992.0;

			a1p0inv = new double[,] { { 184888585.0, 0.0, 1945170933.0 }, { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 } };
			a2p0inv = new double[,] { { 0.0, 360363334.0, 4225571728.0 }, { 1.0, 0.0, 0.0 }, { 0.0, 1.0, 0.0 } };

			a1p0 = new double[,] { { 0.0, 1.0, 0.0 }, { 0.0, 0.0, 1.0 }, { -810728.0, 1403580.0, 0.0 } };
			a2p0 = new double[,] { { 0.0, 1.0, 0.0 }, { 0.0, 0.0, 1.0 }, { -1370589.0, 0.0, 527612.0 } };

			a1p76 = new double[,] { { 82758667.0, 1871391091.0, 4127413238.0 }, { 3672831523.0, 69195019.0, 1871391091.0 }, { 3672091415.0, 3528743235.0, 69195019.0 } };
			a2p76 = new double[,] { { 1511326704.0, 3759209742.0, 1610795712.0 }, { 4292754251.0, 1511326704.0, 3889917532.0 }, { 3859662829.0, 4292754251.0, 3708466080.0 } };

			a1p127 = new double[,] { { 2427906178.0, 3580155704.0, 949770784.0 }, { 226153695.0, 1230515664.0, 3580155704.0 }, { 1988835001.0, 986791581.0, 1230515664.0 } };
			a2p127 = new double[,] { { 1464411153.0, 277697599.0, 1610723613.0 }, { 32183930.0, 1464411153.0, 1022607788.0 }, { 2824425944.0, 32183930.0, 2093834863.0 } };

			// This is the state variables.

			Cg = new double[6];
			Bg = new double[6];
			Ig = new double[6];

			if (!CheckSeed(seed))
				throw new ArgumentException("Invalid seed specified.", "seed");

			for (int i = 0; i < 6; i++)
				Bg[i] = Cg[i] = Ig[i] = seed[i];
		}

		public RngStream(int n)
			: this()
		{
			NextStream(n);
		}

		public RngStream(uint[] seed, int n)
			: this(seed)
		{
			NextStream(n);
		}

		private bool CheckSeed(uint[] seed)
		{
			if (seed == null || seed.Length != 6)
				return false;

			for (int i = 0; i < 3; i++)
				if (seed[i] >= m1 || seed[i] < 0)
					return false;

			for (int i = 3; i < 6; i++)
				if (seed[i] >= m2 || seed[i] < 0)
					return false;

			if (seed[0] == 0 && seed[1] == 0 && seed[2] == 0)
				return false;

			if (seed[3] == 0 && seed[4] == 0 && seed[5] == 0)
				return false;

			return true;
		}

		public void ResetStream()
		{
			for (int i = 0; i < 6; i++)
				Cg[i] = Bg[i] = Ig[i];
		}

		/*public void NextStream()
		{
		  MatVecModM(a1p127, nextSeed, nextSeed, m1);
		  double[] temp = new double[3];
		  for (int i = 0; i < 3; i++)
			temp[i] = nextSeed[i + 3];
		  MatVecModM(a2p127, temp, temp, m2);
		  for (int i = 0; i < 3; i++)
			nextSeed[i + 3] = temp[i];
		}*/

		public void NextStream()
		{
			NextStream(0, 0);
		}

		public void NextStream(int n)
		{
			if (n != 0)
			{
				//int e, c;
				if (n > 0)
				{
					//e = Log2(n);
					//c = n - Pow2(e);
				}
				int e = (int)(Math.Log(n) / Math.Log(2.0));
				int c;
				if (e == 0)
					c = n - (int)Math.Pow(2.0, e);
				else
					c = n;
				NextStream(e, c);
			}
			// ResetStream();
			// FIXME: ellers lav en reset
		}

		private int Log2(uint x)
		{
			// See http://en.wikipedia.org/wiki/Logarithm#Computers.

			int r = 0;
			while (x >> r != 0)
				r++;

			// Returns -1 for x == 0, floor(log2(x)) otherwise.

			return r - 1;
		}

		private int Pow2(int x)
		{
			return 1 << x;
		}

		private void NextStream(int e, int c)
		{
			double[,] b1 = new double[3, 3];
			double[,] c1 = new double[3, 3];
			double[,] b2 = new double[3, 3];
			double[,] c2 = new double[3, 3];

			if (e > 0)
			{
				MatTwoPowModM(a1p0, b1, m1, e);
				MatTwoPowModM(a2p0, b2, m2, e);
			}
			else if (e < 0)
			{
				MatTwoPowModM(a1p0inv, b1, m1, -e);
				MatTwoPowModM(a2p0inv, b2, m2, -e);
			}

			if (c >= 0)
			{
				MatPowModM(a1p0, c1, m1, c);
				MatPowModM(a2p0, c2, m2, c);
			}
			else if (c < 0)
			{
				MatPowModM(a1p0inv, c1, m1, -c);
				MatPowModM(a2p0inv, c2, m2, -c);
			}

			if (e != 0)
			{
				MatMatModM(b1, c1, c1, m1);
				MatMatModM(b2, c2, c2, m2);
			}

			MatVecModM(c1, Cg, Cg, m1);
			double[] cg3 = new double[3];
			for (int i = 0; i < 3; i++)
				cg3[i] = Cg[i + 3];
			MatVecModM(c2, cg3, cg3, m2);
			for (int i = 0; i < 3; i++)
				Cg[i + 3] = cg3[i];
		}

		public void ResetSubstream()
		{
			for (int i = 0; i < 6; i++)
				Cg[i] = Bg[i];
		}

		public void NextSubstream()
		{
			MatVecModM(a1p76, Bg, Bg, m1);
			double[] temp = new double[3];
			for (int i = 0; i < 3; i++)
			{
				temp[i] = Bg[i + 3];
			}

			MatVecModM(a2p76, temp, temp, m2);
			for (int i = 0; i < 3; i++)
			{
				Bg[i + 3] = temp[i];
			}

			for (int i = 0; i < 6; i++)
			{
				Cg[i] = Bg[i];
			}
		}

		public void NextSubstream(int skip)
		{
			// TODO: Optimize!
			for (int i = 0; i < skip; i++)
			{
				NextSubstream();
			}
		}

		private double MultModM(double a, double s, double c, double m)
		{
			// Compute (a*s + c) MOD m ; m must be < 2^35
			// Works also for s, c < 0.

			double v;
			int a1;
			v = a * s + c;
			if (v >= two53 || v <= -two53)
			{
				a1 = (int)(a / two17);
				a -= a1 * two17;
				v = a1 * s;
				a1 = (int)(v / m);
				v -= a1 * m;
				v = v * two17 + a * s + c;
			}
			a1 = (int)(v / m);
			if ((v -= a1 * m) < 0.0)
				return v += m;
			else
				return v;
		}

		private void MatVecModM(double[,] a, double[] s, double[] v, double m)
		{
			// Returns v = A*s MOD m.  Assumes that -m < s[i] < m.
			// Works even if v = s.

			double[] x = new double[3];
			for (int i = 0; i < 3; i++)
			{
				x[i] = MultModM(a[i, 0], s[0], 0.0, m);
				x[i] = MultModM(a[i, 1], s[1], x[i], m);
				x[i] = MultModM(a[i, 2], s[2], x[i], m);
			}

			for (int i = 0; i < 3; i++)
			{
				v[i] = x[i];
			}
		}

		private void MatMatModM(double[,] A, double[,] B, double[,] C, double m)
		{
			// Returns C = A*B MOD m
			// Note: work even if A = C or B = C or A = B = C.

			double[] V = new double[3];
			double[,] W = new double[3, 3];
			for (int i = 0; i < 3; i++)
			{
				for (int j = 0; j < 3; j++)
					V[j] = B[j, i];
				MatVecModM(A, V, V, m);
				for (int j = 0; j < 3; ++j)
					W[j, i] = V[j];
			}
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
					C[i, j] = W[i, j];
		}

		private void MatTwoPowModM(double[,] A, double[,] B, double m, int e)
		{
			// Compute matrix B = (A^(2^e) Mod m);  works even if A = B

			// initialize: B = A
			if (A != B)
			{
				for (int i = 0; i < 3; i++)
					for (int j = 0; j < 3; j++)
						B[i, j] = A[i, j];
			}

			// Compute B = A^{2^e}
			for (int i = 0; i < e; i++)
				MatMatModM(B, B, B, m);
		}

		private void MatPowModM(double[,] A, double[,] B, double m, int c)
		{
			// Compute matrix D = A^c Mod m ;  works even if A = B
			int n = c;
			double[,] W = new double[3, 3];

			// initialize: W = A; B = I
			for (int i = 0; i < 3; i++)
				for (int j = 0; j < 3; j++)
				{
					W[i, j] = A[i, j];
					B[i, j] = 0.0;
				}
			for (int j = 0; j < 3; j++)
				B[j, j] = 1.0;

			// Compute B = A^c mod m using the binary decomp. of c
			while (n > 0)
			{
				if ((n % 2) == 1)
					MatMatModM(W, B, B, m);
				MatMatModM(W, W, W, m);
				n /= 2;
			}
		}

		public int NextInt(int minValue, int maxValue)
		{
			return (minValue + (int)(NextUniform() * (maxValue - minValue + 1.0)));
		}

		public double NextUniform()
		{
			// The first component.
			double p1 = a12 * Cg[1] - a13n * Cg[0];
			int k1 = (int)(p1 / m1);
			p1 -= k1 * m1;
			if (p1 < 0.0)
				p1 += m1;
			Cg[0] = Cg[1];
			Cg[1] = Cg[2];
			Cg[2] = p1;

			// The second component.
			double p2 = a21 * Cg[5] - a23n * Cg[3];
			int k2 = (int)(p2 / m2);
			p2 -= k2 * m2;
			if (p2 < 0.0)
				p2 += m2;
			Cg[3] = Cg[4];
			Cg[4] = Cg[5];
			Cg[5] = p2;

			// Combination of the two.
			if (p1 > p2)
				return (p1 - p2) * norm;
			else
				return (p1 - p2 + m1) * norm;
		}

		public double NextDouble53()
		{
			double u = NextUniform();
			u += NextUniform() * two24inv;
			if (u < 1.0)
				return u;
			else
				return u - 1.0;
		}

		public void SetSeed(uint seed)
		{
			SetSeed(new uint[] { seed, seed, seed, seed, seed, seed });
		}

		public void SetSeed(uint[] seed)
		{
			if (!CheckSeed(seed))
				throw new ArgumentException("Invalid seed specified.", "seed");

			for (int i = 0; i < 6; i++)
			{
				Bg[i] = Cg[i] = Ig[i] = seed[i];
			}
		}
	}
}
