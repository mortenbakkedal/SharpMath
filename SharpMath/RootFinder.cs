// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath
{
	public static class RootFinder
	{
		public static double Bisection(Func<double, double> f, double a, double b)
		{
			double fa = f(a);
			if (fa * f(b) >= 0.0)
			{
				throw new ArithmeticException();
			}

			// http://en.wikipedia.org/w/index.php?title=Bisection_method&oldid=422656767#Practical_considerations

			// Assumption: One of f(a) and f(b) is >= 0 and the other is <= 0.
			double lo, hi;
			if (fa <= 0)
			{
				lo = a;
				hi = b;
			}
			else
			{
				lo = b;
				hi = a;
			}

			int i = 0;
			double mid = lo + (hi - lo) / 2.0;
			while (mid != lo && mid != hi)
			{
				i++;
				if (f(mid) <= 0.0)
				{
					lo = mid;
				}
				else
				{
					hi = mid;
				}
				mid = lo + (hi - lo) / 2.0;
			}

			return mid;
		}
	}
}
