// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

using FuncLib.Mathematics;

namespace SharpMath.Optimization
{
	[Serializable]
	public sealed class ComplexFunction
	{
		private Function re, im;

		static ComplexFunction()
		{
			I = new ComplexFunction(0.0, 1.0);
		}

		public ComplexFunction(Function re, Function im)
		{
			this.re = re;
			this.im = im;
		}

		public Complex Value(IPoint point)
		{
			return new Complex(re.Value(point), im.Value(point));
		}

		public Complex Value(params VariableAssignment[] assignments)
		{
			return Value(new Point(assignments));
		}

		public Complex Value(IEvaluator evaluator)
		{
			return new Complex(re.Value(evaluator), im.Value(evaluator));
		}
		
		public ComplexFunction Derivative(Variable variable)
		{
			// Currently no support for anything like a ComplexVariable. How to implement this?
			return new ComplexFunction(re.Derivative(variable), im.Derivative(variable));
		}

		public ComplexFunction Derivative(Variable variable, int order)
		{
			return new ComplexFunction(re.Derivative(variable, order), im.Derivative(variable, order));
		}

		public ComplexFunction Derivative(params Variable[] variables)
		{
			return new ComplexFunction(re.Derivative(variables), im.Derivative(variables));
		}

		public static ComplexFunction operator +(ComplexFunction z1, ComplexFunction z2)
		{
			return new ComplexFunction(z1.re + z2.re, z1.im + z2.im);
		}

		public static ComplexFunction operator -(ComplexFunction z)
		{
			return new ComplexFunction(-z.re, -z.im);
		}

		public static ComplexFunction operator -(ComplexFunction z1, ComplexFunction z2)
		{
			return new ComplexFunction(z1.re - z2.re, z1.im - z2.im);
		}

		public static ComplexFunction operator *(ComplexFunction z1, ComplexFunction z2)
		{
			return new ComplexFunction(z1.re * z2.re - z1.im * z2.im, z1.im * z2.re + z1.re * z2.im);
		}

		public static ComplexFunction operator /(ComplexFunction z1, ComplexFunction z2)
		{
			Function c = z2.re * z2.re + z2.im * z2.im;
			return new ComplexFunction((z1.re * z2.re + z1.im * z2.im) / c, (z1.im * z2.re - z1.re * z2.im) / c);
		}

		public static implicit operator ComplexFunction(Function a)
		{
			return new ComplexFunction(a, 0.0);
		}

		public static implicit operator ComplexFunction(double a)
		{
			return new ComplexFunction(a, 0.0);
		}

		public static implicit operator ComplexFunction(Complex z)
		{
			return new ComplexFunction(Complex.Re(z), Complex.Im(z));
		}

		public static Function Re(ComplexFunction z)
		{
			return z.re;
		}

		public static Function Im(ComplexFunction z)
		{
			return z.im;
		}

		public static ComplexFunction Conjugate(ComplexFunction z)
		{
			return new ComplexFunction(z.re, -z.im);
		}

		public static Function Abs(ComplexFunction z)
		{
			return Function.Sqrt(z.re * z.re + z.im * z.im);
		}

		public static ComplexFunction Exp(ComplexFunction z)
		{
			Function c = Function.Exp(z.re);
			return new ComplexFunction(c * Function.Cos(z.im), c * Function.Sin(z.im));
		}

		public static ComplexFunction Sqr(ComplexFunction z)
		{
			return new ComplexFunction(z.re * z.re - z.im * z.im, 2.0 * z.re * z.im);
		}

		public static ComplexFunction Sqrt(ComplexFunction z)
		{
			ReSqrtFunction re = new ReSqrtFunction(z);
			ImSqrtFunction im = new ImSqrtFunction(z);
			ComplexFunction z05 = new ComplexFunction(re, im);
			re.Update(z05);
			im.Update(z05);
			return z05;
		}

		public static Function Arg(ComplexFunction z)
		{
			// Use Complex.Arg through the inner class defined below.
			return new ArgFunction(z);
		}

		public static ComplexFunction Log(ComplexFunction z)
		{
			return new ComplexFunction(0.5 * Function.Log(z.re * z.re + z.im * z.im), Arg(z));
		}

		public static ComplexFunction I
		{
			get;
			private set;
		}

		[Serializable]
		private class SqrtHelper
		{
		}

		[Serializable]
		private class ReSqrtFunction : Function
		{
			private ComplexFunction z, z05;

			public ReSqrtFunction(ComplexFunction z)
			{
				this.z = z;
			}

			public void Update(ComplexFunction z05)
			{
				this.z05 = z05;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				// FIXME Implement using a common helper class.
				return Complex.Re(Complex.Sqrt(z.Value(evaluator)));
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return ComplexFunction.Re(0.5 * new ComplexFunction(ComplexFunction.Re(z).Derivative(variable), ComplexFunction.Im(z).Derivative(variable)) / z05);
			}
		}

		[Serializable]
		private class ImSqrtFunction : Function
		{
			private ComplexFunction z, z05;

			public ImSqrtFunction(ComplexFunction z)
			{
				this.z = z;
			}

			public void Update(ComplexFunction z05)
			{
				this.z05 = z05;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Complex.Im(Complex.Sqrt(z.Value(evaluator)));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return ComplexFunction.Im(0.5 * new ComplexFunction(ComplexFunction.Re(z).Derivative(variable), ComplexFunction.Im(z).Derivative(variable)) / z05);
			}
		}

		[Serializable]
		private class ArgFunction : Function
		{
			private ComplexFunction z;

			public ArgFunction(ComplexFunction z)
			{
				this.z = z;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Complex.Arg(z.Value(evaluator));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return (Function.Sqr(z.re) * z.im.Derivative(variable) - z.im * z.re.Derivative(variable)) / (Function.Sqr(z.re) + Function.Sqr(z.im));
			}
		}
	}
}
