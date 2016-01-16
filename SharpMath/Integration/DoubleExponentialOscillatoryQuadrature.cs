using System;

namespace SharpMath.Integration
{
	/// <summary>
	/// Fast and accurate computation of the Fourier transform and Fourier type integrals using Takuya Ooura's double exponential formula.
	/// </summary>
	public sealed class DoubleExponentialOscillatoryQuadrature
	{
		private double omega0;
		private int n, n1, n2;
		private double[] phi, phi1;

		public DoubleExponentialOscillatoryQuadrature(double omega0, int n1, int n2, double h)
			: this(omega0, h)
		{
			if (n1 < 0 || n2 < 0)
			{
				throw new ArgumentException("Invalid number of mesh points.");
			}

			this.n1 = n1;
			this.n2 = n2;

			n = 1 + n1 + n2;

			double m = Math.PI / (Math.Abs(omega0) * h);
			double beta = 0.25;
			double alpha = beta / Math.Sqrt(1.0 + m * Math.Log(1.0 + m) / (4.0 * Math.PI));

			ComputePhi(m, h, alpha, beta);
		}

		public DoubleExponentialOscillatoryQuadrature(double omega0, double eps1, double eps2, double h)
			: this(omega0, h)
		{
			if (eps1 <= 0.0 || eps2 <= 0.0)
			{
				throw new ArgumentException("Invalid target precision.");
			}

			double m = Math.PI / (Math.Abs(omega0) * h);
			double beta = 0.25;
			double alpha = beta / Math.Sqrt(1.0 + m * Math.Log(1.0 + m) / (4.0 * Math.PI));

			double t1 = Math.Log(-Math.Log(eps1 / m) / alpha + 1.0);
			t1 = Math.Log((2.0 * t1 - Math.Log(eps1 / m / t1)) / alpha + 1.0);
			t1 = Math.Log((2.0 * t1 - Math.Log(eps1 / m / t1)) / alpha + 1.0);
			n1 = 1 + (int)(t1 / h);

			double t2 = Math.Log(-Math.Log(eps2 / m) / beta + 1.0);
			t2 = Math.Log((2.0 * t2 - Math.Log(eps2 / m / t2)) / beta + 1.0);
			t2 = Math.Log((2.0 * t2 - Math.Log(eps2 / m / t2)) / beta + 1.0);
			n2 = 1 + (int)(t2 / h);

			n = 1 + n1 + n2;

			ComputePhi(m, h, alpha, beta);
		}

		private DoubleExponentialOscillatoryQuadrature(double omega0, double h)
		{
			if (omega0 == 0.0)
			{
				throw new ArgumentException("Invalid transform central point.");
			}

			if (h <= 0.0)
			{
				throw new ArgumentException("Invalid integral step size.");
			}

			this.omega0 = omega0;
		}

		private void ComputePhi(double m, double h, double alpha, double beta)
		{
			phi = new double[n];
			phi1 = new double[n];
			for (int i = 0; i < n; i++)
			{
				double t = h * (i - n1);
				if (t != 0.0)
				{
					double e1 = alpha * (1.0 - Math.Exp(-t));
					double e2 = beta * (Math.Exp(t) - 1.0);
					double u = 1.0 / (1.0 - Math.Exp(-2.0 * t - e1 - e2));
					phi[i] = m * t * u; // M*phi(n*h)
					phi1[i] = m * h * (1.0 - t * (2.0 + alpha - e1 + beta + e2) * (u - 1.0)) * u; // M*h*phi'(n*h)
				}
				else
				{
					double u = 1.0 / (2.0 + alpha + beta);
					phi[i] = m * u; // M*phi(0)
					phi1[i] = m * h * (1.0 + (alpha - beta) * u * u) / 2.0; // M*h*phi'(0)
				}
			}
		}

		/// <summary>
		/// Computes the transform $g(\omega)=\int_0^\infty e^{i\omega x}f(x)dx$ for a range of omegas.
		/// </summary>
		public void Transform(Func<double, double> f, int k, out double[] omega, out Complex[] g)
		{
			if (k < 1)
			{
				throw new ArgumentException("Invalid number of transform points.");
			}

			// See also:
			// http://www.kurims.kyoto-u.ac.jp/~ooura/profile.html
			// http://www.kurims.kyoto-u.ac.jp/~ooura/papers/DE-FT-gen.pdf
			// http://www.kurims.kyoto-u.ac.jp/~ooura/papers/deft_new.c

			int k2 = 2 * k - 1;

			double[] g_re = new double[k2];
			double[] g_im = new double[k2];

			double delta = Math.Abs(omega0) / k;
			for (int i = 0; i < n; i++)
			{
				double x = phi[i];
				double y = 0.5 * (x * Math.Abs(omega0) - Math.PI * (i - n1));

				double w_re = Math.Cos(y);
				double w_im = Math.Sin(y);

				double s_re = Math.Cos(delta * x);
				double s_im = Math.Sin(delta * x);

				// Prepare left end-point of transform interval.
				double t0_re = 2.0 * phi1[i] * w_im * f(x);
				double t0_im = 0.0;

				for (int j = 0; j < k2; j++)
				{
					// Compute first part of the exponential for this transform point.
					double t_re = t0_re * s_re - t0_im * s_im;
					double t_im = t0_im * s_re + t0_re * s_im;
					t0_re = t_re;
					t0_im = t_im;

					// Full contribution to the sum.
					g_re[j] -= t0_im * w_re - t0_re * w_im;
					g_im[j] += t0_re * w_re + t0_im * w_im;
				}
			}

			omega = new double[k2];
			g = new Complex[k2];
			for (int i = 0; i < k2; i++)
			{
				// Perform a trivial transformation of the integral when omega0 is negative. Holds since f is real-valued.
				omega[i] = Math.Sign(omega0) * delta * (i + 1);
				g[i] = new Complex(g_re[i], Math.Sign(omega0) * g_im[i]);
			}
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{i\omega x}f(x)dx$.
		/// </summary>
		public Complex Integrate(Func<double, double> f)
		{
			double[] omega;
			Complex[] g;
			Transform(f, 1, out omega, out g);
			return g[0];
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{i\omega x}f(x)dx$.
		/// </summary>
		public Complex Integrate(Func<double, Complex> f)
		{
			return Integrate(x => Complex.Re(f(x))) + Complex.I * Integrate(x => Complex.Im(f(x)));
		}

		/// <summary>
		/// Computes the transform $g(\omega)=\int_0^\infty e^{i\omega x}f(x)dx$ for a range of omegas.
		/// </summary>
		public static void Transform(Func<double, double> f, double omega0, double eps1, double eps2, double h, int k, out double[] omega, out Complex[] g)
		{
			new DoubleExponentialOscillatoryQuadrature(omega0, eps1, eps2, h).Transform(f, k, out omega, out g);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{i\omega x}f(x)dx$.
		/// </summary>
		public static Complex Integrate(Func<double, double> f, double omega, int n1, int n2, double h)
		{
			return new DoubleExponentialOscillatoryQuadrature(omega, n1, n2, h).Integrate(f);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{i\omega x}f(x)dx$.
		/// </summary>
		public static Complex Integrate(Func<double, double> f, double omega, double eps1, double eps2, double h)
		{
			return new DoubleExponentialOscillatoryQuadrature(omega, eps1, eps2, h).Integrate(f);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{i\omega x}f(x)dx$.
		/// </summary>
		public static Complex Integrate(Func<double, Complex> f, double omega, int n1, int n2, double h)
		{
			return new DoubleExponentialOscillatoryQuadrature(omega, n1, n2, h).Integrate(f);
		}

		/// <summary>
		/// Computes the integral $\int_0^\infty e^{i\omega x}f(x)dx$.
		/// </summary>
		public static Complex Integrate(Func<double, Complex> f, double omega, double eps1, double eps2, double h)
		{
			return new DoubleExponentialOscillatoryQuadrature(omega, eps1, eps2, h).Integrate(f);
		}

		/*public static void Test1()
		{
			Func<double, double> f1 = x => Math.Log(x) / Math.Sqrt(x);
			Func<double, double> F1r = o => Math.Sqrt(Math.PI / 2 / o) * (Math.Log(4 * o) + 0.57721566490153286060 + Math.PI / 2);
			Func<double, double> F1i = o => Math.Sqrt(Math.PI / 2 / o) * (Math.Log(4 * o) + 0.57721566490153286060 - Math.PI / 2);
			
			int k = 128;
			int k2 = 2 * k - 1;
			double h = 0.075;
			double omega0 = 1.0;

			DoubleExponentialOscillatory de = new DoubleExponentialOscillatory(omega0, 1.0e-24, 1.0e-12, h);

			double[] omega;
			Complex[] g;
			de.Transform(f1, k, out omega, out g);

			for (int i = 0; i < k2; i++)
			{
				double o = omega0 * (i + 1) / k;
				double er = Complex.Re(g[i]) + F1r(o);
				double ei = Complex.Im(g[i]) + F1i(o);
				double err = Math.Sqrt(er * er + ei * ei);
				Console.WriteLine("omega = {0}, error = {1}", o, err);
			}
			Console.WriteLine("N_-= {0}, N_+= {1}", de.n1, de.n2);
		}

		public static void Test2()
		{
			Func<double, double> g_im = o => -(Math.Sqrt(Math.PI / 2 / o) * (Math.Log(4 * o) + 0.57721566490153286060 - Math.PI / 2));
			Console.WriteLine(Complex.Im(DoubleExponentialOscillatory.Integrate(x => Math.Log(x) / Math.Sqrt(x), 1.0, 1.0e-24, 1.0e-12, 0.075)));
			Console.WriteLine(g_im(1.0));
			Console.WriteLine();

			Console.WriteLine(Complex.Im(DoubleExponentialOscillatory.Integrate(x => 1.0 / Math.Sqrt(x), 1.0, 1.0e-24, 1.0e-12, 0.075)));
			Console.WriteLine(Math.Sqrt(Math.PI / 2));
			Console.WriteLine();

			Console.WriteLine(Complex.Re(DoubleExponentialOscillatory.Integrate(x => 1.0 / Math.Sqrt(x), 1.0, 1.0e-24, 1.0e-12, 0.075)));
			Console.WriteLine(Math.Sqrt(Math.PI / 2));
			Console.WriteLine();

			Console.WriteLine(Complex.Im(DoubleExponentialOscillatory.Integrate(x => Math.Log(x), 1.0, 1.0e-24, 1.0e-12, 0.075)));
			Console.WriteLine(-0.57721566490153286060);
			Console.WriteLine();
		}*/
	}
}

