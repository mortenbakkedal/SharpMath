// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	public partial class Function
	{
		[DebuggerStepThrough]
		public static Function Exp(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Exp(f0.Constant);
			}

			return new ExpFunction(f);
		}

		[DebuggerStepThrough]
		public static Function Log(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Log(f0.Constant);
			}

			return new LogFunction(f);
		}

		[DebuggerStepThrough]
		public static Function Sqr(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant * f0.Constant;
			}

			return new SqrFunction(f);
		}

		[DebuggerStepThrough]
		public static Function Sqrt(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Sqrt(f0.Constant);
			}

			return new SqrtFunction(f);
		}

		[DebuggerStepThrough]
		public static Function Pow(Function f, Function g)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Pow(f0.Constant, g);
			}

			ConstantFunction g0 = g as ConstantFunction;
			if (g0 != null)
			{
				return Pow(f, g0.Constant);
			}

			return new PowFunction(f, g);
		}

		[DebuggerStepThrough]
		public static Function Pow(Function f, double a)
		{
			// Allow special handling of a few special cases (while not guaranteed to be numerically identical).

			if (a == 1.0)
			{
				return f;
			}

			if (a == 0.5)
			{
				return Sqrt(f);
			}

			if (a == 2.0)
			{
				return Sqr(f);
			}

			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Pow(f0.Constant, a);
			}

			return new PowConstantFunction(f, a);
		}

		[DebuggerStepThrough]
		public static Function Pow(double a, Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Pow(a, f0.Constant);
			}

			// No special handling of this case (currently).
			return new PowFunction(a, f);
		}

		[DebuggerStepThrough]
		public static Function Cos(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Cos(f0.Constant);
			}

			return new CosFunction(f);
		}

		[DebuggerStepThrough]
		public static Function Sin(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return Math.Sin(f0.Constant);
			}

			return new SinFunction(f);
		}

		[Serializable]
		private class ExpFunction : Function
		{
			private Function f;

			public ExpFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Exp(f.Value(evaluator));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				// Reuse the same object here.
				return this * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Exp(f.Substitute(evaluator));
			}
		}

		[Serializable]
		private class LogFunction : Function
		{
			private Function f;

			public LogFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Log(f.Value(evaluator));
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return 1.0 / f * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Log(f.Substitute(evaluator));
			}
		}

		[Serializable]
		private class SqrFunction : Function
		{
			private Function f;

			public SqrFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				double x = f.Value(evaluator);
				return x * x;
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return 2.0 * f * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Sqr(f.Substitute(evaluator));
			}
		}

		[Serializable]
		private class SqrtFunction : Function
		{
			private Function f;

			public SqrtFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Sqrt(f.Value(evaluator));
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				// Reuse the same object here.
				return 0.5 / this * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Sqrt(f.Substitute(evaluator));
			}
		}

		[Serializable]
		private class PowFunction : Function
		{
			private Function f, g;

			public PowFunction(Function f, Function g)
			{
				this.f = f;
				this.g = g;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Pow(f.Value(evaluator), g.Value(evaluator));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return g * Function.Pow(f, g - 1.0) * f.Derivative(variable) + Function.Log(f) * this * g.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Pow(f.Substitute(evaluator), g.Substitute(evaluator));
			}
		}

		[Serializable]
		private class PowConstantFunction : Function
		{
			private Function f;
			private double a;

			public PowConstantFunction(Function f, double a)
			{
				this.f = f;
				this.a = a;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Pow(f.Value(evaluator), a);
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return a * Function.Pow(f, a - 1.0) * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Pow(f.Substitute(evaluator), a);
			}
		}

		[Serializable]
		private class CosFunction : Function
		{
			private Function f;
			private SinFunction sin;

			public CosFunction(Function f)
				: this(f, null)
			{
			}

			public CosFunction(Function f, SinFunction sin)
			{
				this.f = f;
				this.sin = sin;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Cos(f.Value(evaluator));
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				if (sin == null)
				{
					sin = new SinFunction(f, this);
				}

				return -sin * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Cos(f.Substitute(evaluator));
			}
		}

		[Serializable]
		private class SinFunction : Function
		{
			private Function f;
			private CosFunction cos;

			public SinFunction(Function f)
				: this(f, null)
			{
			}

			public SinFunction(Function f, CosFunction cos)
			{
				this.f = f;
				this.cos = cos;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Sin(f.Value(evaluator));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				if (cos == null)
				{
					cos = new CosFunction(f, this);
				}

				return cos * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Sin(f.Substitute(evaluator));
			}
		}
	}
}
