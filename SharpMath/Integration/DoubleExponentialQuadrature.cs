using System;

namespace SharpMath.Integration
{
	public static class DoubleExponentialQuadrature
	{
		public static void Test()
		{
			//Console.WriteLine(UnboundedIntegrate(x => Math.Exp(-1.0 - x) / (1.0 + x), 0.0, 1e-15));
			//Console.WriteLine(UnboundedIntegrate(x => Math.Exp(-1.0 - x) / (1.0 + x), 0.0, 150, 150, 0.025));
			Console.WriteLine(UnboundedIntegrate(x => Math.Exp(-1.0 - x) / (1.0 + x), 0.0, 100, 10, 0.14));
			Console.WriteLine(Integrate(x => Math.Exp(-1.0 - x) / (1.0 + x), 1.0, 20.0, 1e-8));


			//Console.WriteLine(UnboundedOscillatoryIntegrate(x => 1.0 / Math.Sqrt(x), 0.0, 1.0, Math.PI, 1e-18));
			//Console.WriteLine(Math.Sqrt(Math.PI / 2));
			//Console.WriteLine();
			//i = Integrate(x => Math.Cos(Math.PI * x) / Math.Sqrt(1 - x), 0, 0.5, 1e-9);
			//Console.WriteLine(i);
			//Console.WriteLine(err);
		}

		/// <summary>
		/// Computes the integral $\int_a^b f(x)dx$ using the Double Exponential quadrature. Fixed step size version.
		/// </summary>
		public static double Integrate(Func<double, double> f, double a, double b, int n, int h)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Computes the integral $\int_a^b f(x)dx$ using the Double Exponential quadrature. Adaptive version.
		/// </summary>
		public static double Integrate(Func<double, double> f, double a, double b, double eps)
		{
			double err;
			return Integrate(f, a, b, eps, out err);
		}

		/// <summary>
		/// Computes the integral $\int_a^b f(x)dx$ using the Double Exponential quadrature. Adaptive version with error estimate.
		/// </summary>
		public static double Integrate(Func<double, double> f, double a, double b, double eps, out double err)
		{
			// Following Takuya Ooura's implementation:
			// http://www.kurims.kyoto-u.ac.jp/~ooura/intde.html
			//
			// See also:
			// H.Takahasi and M.Mori, Double exponential formulas for numerical integration, Pub. RIMS Kyoto Univ. 9, 1974, 721-741 

			double i;

			// Adjustable parameters.
			int mmax = 256;
			double efs = 0.1;
			double hoff = 8.5;

			int m;
			double pi2, epsln, epsh, h0, ehp, ehm, epst, ba, ir, h, iback, irback, t, ep, em, xw, xa, wg, fa, fb, errt, errh, errd;

			pi2 = 2.0 * Math.Atan(1.0);
			epsln = 1.0 - Math.Log(efs * eps);
			epsh = Math.Sqrt(efs * eps);
			h0 = hoff / epsln;
			ehp = Math.Exp(h0);
			ehm = 1.0 / ehp;
			epst = Math.Exp(-ehm * epsln);
			ba = b - a;
			ir = f((a + b) * 0.5) * (ba * 0.25);
			i = ir * (2.0 * pi2);
			err = Math.Abs(i) * epst;
			h = 2.0 * h0;
			m = 1;
			errh = 0.0;
			do
			{
				iback = i;
				irback = ir;
				t = h * 0.5;
				do
				{
					em = Math.Exp(t);
					ep = pi2 * em;
					em = pi2 / em;
					do
					{
						xw = 1.0 / (1.0 + Math.Exp(ep - em));
						xa = ba * xw;
						wg = xa * (1.0 - xw);
						fa = f(a + xa) * wg;
						fb = f(b - xa) * wg;
						ir += fa + fb;
						i += (fa + fb) * (ep + em);
						errt = (Math.Abs(fa) + Math.Abs(fb)) * (ep + em);
						if (m == 1)
						{
							err += errt * epst;
						}
						ep *= ehp;
						em *= ehm;
					} while (errt > err || xw > epsh);
					t += h;
				} while (t < h0);
				if (m == 1)
				{
					errh = (err / epst) * epsh * h0;
					errd = 1.0 + 2.0 * errh;
				}
				else
				{
					errd = h * (Math.Abs(i - 2.0 * iback) + 4.0 * Math.Abs(ir - 2.0 * irback));
				}
				h *= 0.5;
				m *= 2;
			} while (errd > errh && m < mmax);
			i *= h;
			if (errd > errh)
			{
				err = -errd * m;
			}
			else
			{
				err = errh * epsh * m / (2.0 * efs);
			}

			if (err < 0.0)
			{
				throw new ArithmeticException();
			}

			return i;
		}

		/// <summary>
		/// Computes the integral $\int_a^\infty f(x)dx$ using the Double Exponential quadrature. Fixed step size version.
		/// </summary>
		public static double UnboundedIntegrate(Func<double, double> f, double a, int n1, int n2, double h)
		{
			if (a != 0.0)
			{
				throw new NotImplementedException();
			}

			// Implemented in the specialized TanhSinhQuadrature class.
			return ExpSinhQuadrature.Integrate(f, n1, n2, h);
		}

		/// <summary>
		/// Computes the integral $\int_a^\infty f(x)dx$ using the Double Exponential quadrature. Adaptive version.
		/// </summary>
		public static double UnboundedIntegrate(Func<double, double> f, double a, double eps)
		{
			double err;
			return UnboundedIntegrate(f, a, eps, out err);
		}

		/// <summary>
		/// Computes the integral $\int_a^\infty f(x)dx$ using the Double Exponential quadrature. Adaptive version with error estimate.
		/// </summary>
		public static double UnboundedIntegrate(Func<double, double> f, double a, double eps, out double err)
		{
			double i;

			// Adjustable parameters.
			int mmax = 256;
			double efs = 0.1;
			double hoff = 11.0;

			int m;
			double pi4, epsln, epsh, h0, ehp, ehm, epst, ir, h, iback, irback, t, ep, em, xp, xm, fp, fm, errt, errh, errd;

			pi4 = Math.Atan(1.0);
			epsln = 1.0 - Math.Log(efs * eps);
			epsh = Math.Sqrt(efs * eps);
			h0 = hoff / epsln;
			ehp = Math.Exp(h0);
			ehm = 1.0 / ehp;
			epst = Math.Exp(-ehm * epsln);
			ir = f(a + 1.0);
			i = ir * (2.0 * pi4);
			err = Math.Abs(i) * epst;
			h = 2.0 * h0;
			m = 1;
			errh = 0.0;
			do
			{
				iback = i;
				irback = ir;
				t = h * 0.5;
				do
				{
					em = Math.Exp(t);
					ep = pi4 * em;
					em = pi4 / em;
					do
					{
						xp = Math.Exp(ep - em);
						xm = 1.0 / xp;
						fp = f(a + xp) * xp;
						fm = f(a + xm) * xm;
						ir += fp + fm;
						i += (fp + fm) * (ep + em);
						errt = (Math.Abs(fp) + Math.Abs(fm)) * (ep + em);
						if (m == 1)
						{
							err += errt * epst;
						}
						ep *= ehp;
						em *= ehm;
					} while (errt > err || xm > epsh);
					t += h;
				} while (t < h0);
				if (m == 1)
				{
					errh = (err / epst) * epsh * h0;
					errd = 1.0 + 2.0 * errh;
				}
				else
				{
					errd = h * (Math.Abs(i - 2.0 * iback) + 4.0 * Math.Abs(ir - 2.0 * irback));
				}
				h *= 0.5;
				m *= 2;
			} while (errd > errh && m < mmax);
			i *= h;
			if (errd > errh)
			{
				err = -errd * m;
			}
			else
			{
				err = errh * epsh * m / (2.0 * efs);
			}

			if (err < 0.0)
			{
				throw new ArithmeticException();
			}

			return i;
		}

		/// <summary>
		/// Computes the integral $\int_a^\infty f(x)\sin(\omega x+\theta)dx$ using the Double Exponential quadrature.  Fixed step size version.
		/// </summary>
		public static double UnboundedOscillatoryIntegrate(Func<double, double> f, double a, double omega, double theta, int n1, int n2, double h)
		{
			if (a != 0.0 || theta != 0.0)
			{
				throw new NotImplementedException();
			}

			// Use complex-valued DoubleExponentialOscillatoryQuadrature until implemented here.
			return Complex.Im(DoubleExponentialOscillatoryQuadrature.Integrate(f, omega, n1, n2, h));
		}

		/// <summary>
		/// Computes the integral $\int_a^\infty f(x)\sin(\omega x+\theta)dx$ using the Double Exponential quadrature. Adaptive version.
		/// </summary>
		public static double UnboundedOscillatoryIntegrate(Func<double, double> f, double a, double omega, double theta, double eps)
		{
			double err;
			return UnboundedOscillatoryIntegrate(f, a, omega, theta, eps, out err);
		}

		/// <summary>
		/// Computes the integral $\int_a^\infty f(x)\sin(\omega x+\theta)dx$ using the Double Exponential quadrature. Adaptive version with error estimate.
		/// </summary>
		public static double UnboundedOscillatoryIntegrate(Func<double, double> f, double a, double omega, double theta, double eps, out double err)
		{
			Func<double, double> g = x => f(x) * Math.Sin(omega * x + theta);
			double i;

			// Adjustable parameters.
			int mmax = 256;
			int lmax = 5;
			double efs = 0.1;
			double enoff = 0.40;
			double pqoff = 2.9;
			double ppoff = -0.72;

			int n, m, l, k;
			double pi4, epsln, epsh, frq4, per2, pp, pq, ehp, ehm, ir, h, iback, irback, t, ep, em, tk, xw, wg, xa, fp, fm, errh, tn, errd;

			pi4 = Math.Atan(1.0);
			epsln = 1.0 - Math.Log(efs * eps);
			epsh = Math.Sqrt(efs * eps);
			n = (int)(enoff * epsln);
			frq4 = Math.Abs(omega) / (2.0 * pi4);
			per2 = 4.0 * pi4 / Math.Abs(omega);
			pq = pqoff / epsln;
			pp = ppoff - Math.Log(pq * pq * frq4);
			ehp = Math.Exp(2.0 * pq);
			ehm = 1.0 / ehp;
			xw = Math.Exp(pp - 2.0 * pi4);
			i = g(a + Math.Sqrt(xw * (per2 * 0.5)));
			ir = i * xw;
			i *= per2 * 0.5;
			err = Math.Abs(i);
			h = 2.0;
			m = 1;
			errh = 0.0;
			do
			{
				iback = i;
				irback = ir;
				t = h * 0.5;
				do
				{
					em = Math.Exp(2.0 * pq * t);
					ep = pi4 * em;
					em = pi4 / em;
					tk = t;
					do
					{
						xw = Math.Exp(pp - ep - em);
						wg = Math.Sqrt(frq4 * xw + tk * tk);
						xa = xw / (tk + wg);
						wg = (pq * xw * (ep - em) + xa) / wg;
						fm = g(a + xa);
						fp = g(a + xa + per2 * tk);
						ir += (fp + fm) * xw;
						fm *= wg;
						fp *= per2 - wg;
						i += fp + fm;
						if (m == 1)
						{
							err += Math.Abs(fp) + Math.Abs(fm);
						}
						ep *= ehp;
						em *= ehm;
						tk += 1.0;
					} while (ep < epsln);
					if (m == 1)
					{
						errh = err * epsh;
						err *= eps;
					}
					tn = tk;
					while (Math.Abs(fm) > err)
					{
						xw = Math.Exp(pp - ep - em);
						xa = xw / tk * 0.5;
						wg = xa * (1.0 / tk + 2.0 * pq * (ep - em));
						fm = g(a + xa);
						ir += fm * xw;
						fm *= wg;
						i += fm;
						ep *= ehp;
						em *= ehm;
						tk += 1.0;
					}
					fm = g(a + per2 * tn);
					em = per2 * fm;
					i += em;
					if (Math.Abs(fp) > err || Math.Abs(em) > err)
					{
						l = 0;
						for (; ; )
						{
							l++;
							tn += n;
							em = fm;
							fm = g(a + per2 * tn);
							xa = fm;
							ep = fm;
							em += fm;
							xw = 1.0;
							wg = 1.0;
							for (k = 1; k <= n - 1; k++)
							{
								xw = xw * (n + 1 - k) / k;
								wg += xw;
								fp = g(a + per2 * (tn - k));
								xa += fp;
								ep += fp * wg;
								em += fp * xw;
							}
							wg = per2 * n / (wg * n + xw);
							em = wg * Math.Abs(em);
							if (em <= err || l >= lmax)
							{
								break;
							}
							i += per2 * xa;
						}
						i += wg * ep;
						if (em > err)
						{
							err = em;
						}
					}
					t += h;
				} while (t < 1.0);
				if (m == 1)
				{
					errd = 1.0 + 2.0 * errh;
				}
				else
				{
					errd = h * (Math.Abs(i - 2.0 * iback) + pq * Math.Abs(ir - 2.0 * irback));
				}
				h *= 0.5;
				m *= 2;
			} while (errd > errh && m < mmax);
			i *= h;
			if (errd > errh)
			{
				err = -errd;
			}
			else
			{
				err *= m * 0.5;
			}

			if (err < 0.0)
			{
				throw new ArithmeticException();
			}

			return i;
		}
	}
}
