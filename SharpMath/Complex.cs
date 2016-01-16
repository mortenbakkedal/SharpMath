// Copyright (c) 2012 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;
using System.Globalization;

namespace SharpMath
{
	[Serializable]
	[DebuggerStepThrough]
	[DebuggerDisplay("{ToString(),nq}")]
	public struct Complex : IEquatable<Complex>
	{
		private double re, im;

		static Complex()
		{
			I = new Complex(0.0, 1.0);
		}

		public Complex(double re, double im)
		{
			this.re = re;
			this.im = im;
		}

		public override string ToString()
		{
			if (im == 0.0)
			{
				return re.ToString(CultureInfo.InvariantCulture);
			}
			else if (im > 0.0)
			{
				return re.ToString(CultureInfo.InvariantCulture) + "+i" + im.ToString(CultureInfo.InvariantCulture);
			}
			else
			{
				return re.ToString(CultureInfo.InvariantCulture) + "-i" + (-im).ToString(CultureInfo.InvariantCulture);
			}
		}

		public override int GetHashCode()
		{
			unchecked
			{
				// Add the hash code of the pair as here:
				// http://stackoverflow.com/questions/892618/create-a-hashcode-of-two-numbers
				return (23 * 31 + re.GetHashCode()) * 31 + im.GetHashCode();
			}
		}

		public bool Equals(Complex other)
		{
			return this == (Complex)other;
		}

		public override bool Equals(object other)
		{
			if (!(other is Complex))
			{
				return false;
			}

			return Equals((Complex)other);
		}

		public static implicit operator Complex(double a)
		{
			return new Complex(a, 0.0);
		}

		public static explicit operator double(Complex z)
		{
			return z.re;
		}

		public static Complex operator +(Complex z1, Complex z2)
		{
			return new Complex(z1.re + z2.re, z1.im + z2.im);
		}

		public static Complex operator +(Complex z, double a)
		{
			return new Complex(z.re + a, z.im);
		}

		public static Complex operator +(double a, Complex z)
		{
			return new Complex(a + z.re, z.im);
		}

		public static Complex operator -(Complex z)
		{
			return new Complex(-z.re, -z.im);
		}

		public static Complex operator -(Complex z1, Complex z2)
		{
			return new Complex(z1.re - z2.re, z1.im - z2.im);
		}

		public static Complex operator -(Complex z, double a)
		{
			return new Complex(z.re - a, z.im);
		}

		public static Complex operator -(double a, Complex z)
		{
			return new Complex(a - z.re, -z.im);
		}

		public static Complex operator *(Complex z1, Complex z2)
		{
			return new Complex(z1.re * z2.re - z1.im * z2.im, z1.im * z2.re + z1.re * z2.im);
		}

		public static Complex operator *(Complex z, double a)
		{
			return new Complex(z.re * a, z.im * a);
		}

		public static Complex operator *(double a, Complex z)
		{
			return new Complex(a * z.re, a * z.im);
		}

		public static Complex operator /(Complex z1, Complex z2)
		{
			double c = z2.re * z2.re + z2.im * z2.im;
			return new Complex((z1.re * z2.re + z1.im * z2.im) / c, (z1.im * z2.re - z1.re * z2.im) / c);
		}

		public static Complex operator /(Complex z, double a)
		{
			return new Complex(z.re / a, z.im / a);
		}

		public static Complex operator /(double a, Complex z)
		{
			double c = z.re * z.re + z.im * z.im;
			return new Complex(a * z.re / c, -a * z.im / c);
		}

		public static bool operator ==(Complex z1, Complex z2)
		{
			return z1.re == z2.re && z1.im == z2.im;
		}

		public static bool operator !=(Complex z1, Complex z2)
		{
			return !(z1 == z2);
		}

		public static double Re(Complex z)
		{
			return z.re;
		}

		public static double Im(Complex z)
		{
			return z.im;
		}

		public static Complex Polar(double r, double theta)
		{
			return new Complex(r * Math.Cos(theta), r * Math.Sin(theta));
		}

		public static Complex Conjugate(Complex z)
		{
			return new Complex(z.re, -z.im);
		}

		public static double Abs(Complex z)
		{
			return Math.Sqrt(z.re * z.re + z.im * z.im);
		}

		public static Complex Exp(Complex z)
		{
			double c = Math.Exp(z.re);
			if (z.im == 0.0)
			{
				return new Complex(c, 0.0);
			}
			else
			{
				return new Complex(c * Math.Cos(z.im), c * Math.Sin(z.im));
			}
		}

		public static Complex Sqr(Complex z)
		{
			return new Complex(z.re * z.re - z.im * z.im, 2.0 * z.re * z.im);
		}

		/// <summary>
		/// Computes the principal square root. Discontinuity at the negative real axis.
		/// </summary>
		public static Complex Sqrt(Complex z)
		{
			// Using this identity:
			// http://en.wikipedia.org/w/index.php?title=Square_root&oldid=500359963#Algebraic_formula

			if (z.im != 0.0)
			{
				double c = Complex.Abs(z) + z.re;
				return new Complex(Math.Sqrt(0.5 * c), z.im / Math.Sqrt(2.0 * c));
			}
			else if (z.re >= 0.0)
			{
				return new Complex(Math.Sqrt(z.re), 0.0);
			}
			else
			{
				return new Complex(0.0, Math.Sqrt(-z.re));
			}
		}

		public static double Arg(Complex z)
		{
			if (z.re > 0.0 || z.im != 0.0)
			{
				return 2.0 * Math.Atan2(z.im, Abs(z) + z.re);
			}
			else if (z.re < 0.0 && z.im == 0.0)
			{
				return Math.PI;
			}
			else
			{
				return double.NaN;
			}
		}

		public static Complex Log(Complex z)
		{
			return new Complex(0.5 * Math.Log(z.re * z.re + z.im * z.im), Arg(z));
		}

		public static Complex I
		{
			get;
			private set;
		}
	}
}
