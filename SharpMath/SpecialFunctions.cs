// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath
{
	public static class SpecialFunctions
	{
		/*public static void Test()
		{
			Console.WriteLine(InverseNormal(Normal(-1.5)));
			Console.WriteLine(InverseNormal(Normal(-0.5)));
			Console.WriteLine(InverseNormal(Normal(0.5)));
			Console.WriteLine(InverseNormal(Normal(1.5)));
			Console.WriteLine(InverseNormal(Normal(2.5)));
			Console.WriteLine(InverseNormal(Normal(6.5)));
		}*/

		public static double Polynomial(double x, params double[] coefficients) // FIXME remove
		{
			double y = 0.0;
			for (int i = coefficients.Length - 1; i >= 0; i--)
				y = y * x + coefficients[i];
			return y;
		}

		/// <summary>
		/// Evaluates the cumulative distribution function (CDF) of the standard normal distribution,
		/// $\Phi(x)=\int_{-\infty}^xe^{-t^2/2}dt/\sqrt{2\pi}$.
		/// </summary>
		public static double Normal(double x)
		{
			return 0.5 + 0.5 * ErrorFunction(0.70710678118654746 * x);
		}

		/// <summary>
		/// Evaluates the inverse of the cumulative distribution function (CDF) of the standard normal distribution.
		/// </summary>
		public static double InverseNormal(double p)
		{
			if (p == 0.0)
			{
				return double.NegativeInfinity;
			}

			if (p == 1.0)
			{
				return double.PositiveInfinity;
			}

			if (p < 0.0 || p > 1.0)
			{
				return double.NaN;
			}

			// This is an implementation of Peter J. Acklam's algorithm. Relative error
			// has an absolute value less than 1.15e-9 in the entire region. See
			// http://home.online.no/~pjacklam/notes/invnorm/ for details.

			double z;
			if (p < 0.02425)
			{
				// Rational approximation for lower region.
				double q = Math.Sqrt(-2.0 * Math.Log(p));
				z = Polynomial(q, 2.938163982698783, 4.374664141464968, -2.549732539343734, -2.400758277161838, -3.223964580411365e-1, -7.784894002430293e-3) / Polynomial(q, 1.0, 3.754408661907416, 2.445134137142996, 3.224671290700398e-1, 7.784695709041462e-3);
			}
			else if (p > 0.97575)
			{
				// Rational approximation for upper region.
				double q = Math.Sqrt(-2.0 * Math.Log(1.0 - p));
				z = Polynomial(q, -2.938163982698783, -4.374664141464968, 2.549732539343734, 2.400758277161838, 3.223964580411365e-1, 7.784894002430293e-3) / Polynomial(q, 1.0, 3.754408661907416, 2.445134137142996, 3.224671290700398e-1, 7.784695709041462e-3);
			}
			else
			{
				// Rational approximation for central region.
				double q = p - 0.5;
				double q2 = q * q;
				z = q * Polynomial(q * q, 2.506628277459239, -3.066479806614716e1, 1.383577518672690e2, -2.759285104469687e2, 2.209460984245205e2, -3.969683028665376e1) / Polynomial(q * q, 1.0, -1.328068155288572e1, 6.680131188771972e1, -1.556989798598866e2, 1.615858368580409e2, -5.447609879822406e1);
			}

			// Refining algorithm is based on Halley rational method
			// for finding roots of equations as described at:
			// http://www.math.uio.no/~jacklam/notes/invnorm/index.html

			double e = 0.5 * ComplementaryErrorFunction(-z / 1.4142135623730951) - p;
			double u = e * Math.Sqrt(2.0 * Math.PI) * Math.Exp(z * z / 2.0);
			z = z - u / (1.0 + z * u / 2.0);

			return z;
		}

		/// <summary>
		/// Evaluates the density of the standard normal distribution at the point specified.
		/// </summary>
		public static double NormalDensity(double x)
		{
			return 0.3989422804014327 * Math.Exp(-0.5 * x * x);
		}

		public static double ErrorFunction(double x)
		{
			if (Math.Abs(x) > 1.0)
			{
				return 1.0 - ComplementaryErrorFunction(x);
			}
			else
			{
				//return x * Polynomial(x * x, 5.55923013010394962768e4, 7.00332514112805075473e3, 2.23200534594684319226e3, 9.00260197203842689217e1, 9.60497373987051638749) / Polynomial(x * x, 4.92673942608635921086e4, 2.26290000613890934246e4, 4.59432382970980127987e3, 5.21357949780152679795e2, 3.35617141647503099647e1, 1.0);
				double x2 = x * x;
				return x * ((((9.60497373987051638749 * x2 + 9.00260197203842689217e1) * x2 + 2.23200534594684319226e3) * x2 + 7.00332514112805075473e3) * x2 + 5.55923013010394962768e4) / (((((x2 + 3.35617141647503099647e1) * x2 + 5.21357949780152679795e2) * x2 + 4.59432382970980127987e3) * x2 + 2.26290000613890934246e4) * x2 + 4.92673942608635921086e4);
			}
		}

		public static double ComplementaryErrorFunction(double a)
		{
			// This is based on the Cephes Math Library.

			double x = Math.Abs(a);

			if (x < 1.0)
			{
				return 1.0 - ErrorFunction(a);
			}

			double z = -a * a;

			if (z < -7.09782712893383996732e2) // -MAXLOG
			{
				if (a < 0)
				{
					return 2.0;
				}
				else
				{
					return 0.0;
				}
			}

			z = Math.Exp(z);

			double p, q;
			if (x < 8.0)
			{
				p = Polynomial(x, 5.57535335369399327526e2, 1.02755188689515710272e3, 9.34528527171957607540e2, 5.26445194995477358631e2, 1.96520832956077098242e2, 4.86371970985681366614e1, 7.46321056442269912687, 5.64189564831068821977e-1, 2.46196981473530512524e-10);
				q = Polynomial(x, 5.57535340817727675546e2, 1.65666309194161350182e3, 2.24633760818710981792e3, 1.82390916687909736289e3, 9.75708501743205489753e2, 3.54937778887819891062e2, 8.67072140885989742329e1, 1.32281951154744992508e1, 1.0);
			}
			else
			{
				p = Polynomial(x, 2.97886665372100240670, 7.40974269950448939160, 6.16021097993053585195, 5.01905042251180477414, 1.27536670759978104416, 5.64189583547755073984e-1);
				q = Polynomial(x, 3.36907645100081516050, 9.60896809063285878198, 1.70814450747565897222e1, 1.20489539808096656605e1, 9.39603524938001434673, 2.26052863220117276590, 1.0);
			}

			double y = (z * p) / q;

			if (a < 0.0)
			{
				y = 2.0 - y;
			}

			if (y == 0.0)
			{
				if (a < 0.0)
				{
					return 2.0;
				}
				else
				{
					return 0.0;
				}
			}

			return y;
		}

		public static double InverseErrorFunction(double x)
		{
			return 0.70710678118654746 * InverseNormal(0.5 + 0.5 * x);
		}

		public static double Gamma(double x)
		{
			if (x <= 0.0 && Math.Round(x) == x)
			{
				// Not defined for negative integers.
				return double.NaN;
			}

			// See http://en.wikipedia.org/wiki/Lanczos_approximation. Exact to the 15th digit.
			if (x < 0.5)
			{
				return Math.PI / (Math.Sin(Math.PI * x) * Gamma(1.0 - x));
			}
			else
			{
				return Math.Sqrt(2.0 * Math.PI) * Math.Pow(6.5 + x, x - 0.5) * Math.Exp(-6.5 - x) * (0.99999999999980993 + 676.5203681218851 / x - 1259.1392167224028 / (x + 1.0) + 771.32342877765313 / (x + 2.0) - 176.61502916214059 / (x + 3.0) + 12.507343278686905 / (x + 4.0) - 0.13857109526572012 / (x + 5.0) + 9.9843695780195716e-6 / (x + 6.0) + 1.5056327351493116e-7 / (x + 7.0));
			}
		}

		public static double LogGamma(double x)
		{
			if (x <= 0.0)
			{
				// Not defined for negative values.
				return double.NaN;
			}

			if (x < 1.0)
			{
				return LogGamma(x + 1.0) - Math.Log(x);
			}
			else
			{
				return (x - 0.5) * Math.Log(x + 4.5) - x - 4.5 + Math.Log(2.50662827465 * (1.0 + 76.18009173 / x - 86.50532033 / (x + 1.0) + 24.01409822 / (x + 2.0) - 1.231739516 / (x + 3.0) + 0.120858003e-2 / (x + 4.0) - 0.536382e-5 / (x + 5.0)));
			}
		}
	}
}
