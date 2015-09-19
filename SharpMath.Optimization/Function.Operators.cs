// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	public partial class Function
	{
		private static Function zero;

		static Function()
		{
			// Used very often (e.g. in matrix initialization). It's also important to keep this special value referring to the same object.
			zero = new ConstantFunction(0.0);
		}

		[DebuggerStepThrough]
		public static implicit operator Function(double a)
		{
			if (a == 0.0)
			{
				return zero;
			}

			return new ConstantFunction(a);
		}

		[DebuggerStepThrough]
		public static Function operator +(Function f, Function g)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant + g;
			}

			ConstantFunction g0 = g as ConstantFunction;
			if (g0 != null)
			{
				return f + g0.Constant;
			}

			return new AddFunction(f, g);
		}

		[DebuggerStepThrough]
		public static Function operator +(Function f, double a)
		{
			if (a == 0.0)
			{
				return f;
			}

			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant + a;
			}

			// FIXME AddConstantFunction?
			return new AddFunction(f, a);
		}

		[DebuggerStepThrough]
		public static Function operator +(double a, Function f)
		{
			return f + a;
		}

		[DebuggerStepThrough]
		public static Function operator -(Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return -f0.Constant;
			}

			return new UnaryMinus(f);
		}

		[DebuggerStepThrough]
		public static Function operator -(Function f, Function g)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant - g;
			}

			ConstantFunction g0 = g as ConstantFunction;
			if (g0 != null)
			{
				return f - g0.Constant;
			}

			return new SubtractFunction(f, g);
		}

		[DebuggerStepThrough]
		public static Function operator -(Function f, double a)
		{
			if (a == 0.0)
			{
				return f;
			}

			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant - a;
			}

			// FIXME
			return new SubtractFunction(f, a);
		}

		[DebuggerStepThrough]
		public static Function operator -(double a, Function f)
		{
			if (a == 0.0)
			{
				return -f;
			}

			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return a - f0.Constant;
			}

			// FIXME SubtractConstantFunction?
			return new SubtractFunction(a, f);
		}

		[DebuggerStepThrough]
		public static Function operator *(Function f, Function g)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant * g;
			}

			ConstantFunction g0 = g as ConstantFunction;
			if (g0 != null)
			{
				return f * g0.Constant;
			}

			return new MultiplyFunction(f, g);
		}

		[DebuggerStepThrough]
		public static Function operator *(Function f, double a)
		{
			if (a == 0.0)
			{
				return 0.0;
			}

			if (a == 1.0)
			{
				return f;
			}

			if (a == -1.0)
			{
				return -f;
			}

			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant * a;
			}

			return new MultiplyConstantFunction(f, a);
		}

		[DebuggerStepThrough]
		public static Function operator *(double a, Function f)
		{
			return f * a;
		}

		[DebuggerStepThrough]
		public static Function operator /(Function f, Function g)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return f0.Constant / g;
			}

			ConstantFunction g0 = g as ConstantFunction;
			if (g0 != null)
			{
				return f / g0.Constant;
			}

			return new DivideFunction(f, g);
		}

		[DebuggerStepThrough]
		public static Function operator /(Function f, double a)
		{
			return f * (1.0 / a);
		}

		[DebuggerStepThrough]
		public static Function operator /(double a, Function f)
		{
			ConstantFunction f0 = f as ConstantFunction;
			if (f0 != null)
			{
				return a / f0.Constant;
			}

			return new ReciprocalFunction(a, f);
		}

		public static FunctionConstraint operator <=(Function f, Function g)
		{
			return new FunctionConstraint(f - g, double.NegativeInfinity, 0.0);
		}

		public static FunctionConstraint operator >=(Function f, Function g)
		{
			return g <= f;
		}

		public static FunctionConstraint operator <=(Function f, double a)
		{
			return new FunctionConstraint(f, double.NegativeInfinity, a);
		}

		public static FunctionConstraint operator >=(Function f, double a)
		{
			return new FunctionConstraint(f, a, double.PositiveInfinity);
		}

		public static FunctionConstraint operator <=(double a, Function f)
		{
			return f >= a;
		}

		public static FunctionConstraint operator >=(double a, Function f)
		{
			return f <= a;
		}

		public static FunctionEqualityConstraint operator ==(Function function, double value)
		{
			return new FunctionEqualityConstraint(function, value);
		}

		public static FunctionEqualityConstraint operator !=(Function function, double value)
		{
			// This operator makes no sence in this context, but is required by C#.
			throw new InvalidOperationException();
		}

		public static FunctionEqualityConstraint operator ==(double value, Function variable)
		{
			return variable == value;
		}

		public static FunctionEqualityConstraint operator !=(double value, Function variable)
		{
			// This operator makes no sence in this context, but is required by C#.
			throw new InvalidOperationException();
		}

		public override int GetHashCode()
		{
			// The hash code from Object is fine, but want to get rid of a warning.
			return base.GetHashCode();
		}

		public override bool Equals(object other)
		{
			// The equality from Object is fine, but want to get rid of a warning.
			return base.Equals(other);
		}

		/// <summary>
		/// Computes the sum of one or more functions. Faster than adding many functions using the + operator overload.
		/// </summary>
		public static Function Sum(params Function[] functions)
		{
			Function result;
			if (!TrySumConstantFunction(functions, out result))
			{
				result = new SumFunction(functions);
			}

			return result;
		}

		private static bool TrySumConstantFunction(Function[] functions, out Function result)
		{
			double a = 0.0;
			foreach (Function function in functions)
			{
				if (!(function is ConstantFunction))
				{
					result = null;
					return false;
				}

				a += ((ConstantFunction)function).Constant;
			}

			result = a;
			return true;
		}
		
		/// <summary>
		/// Computes the product of one or more functions. Faster than multiplying many functions using the * operator overload.
		/// </summary>
		public static Function Product(params Function[] functions)
		{
			// Implement like for Sum.
			throw new NotImplementedException();
		}

		[Serializable]
		private class AddFunction : Function
		{
			private Function f, g;

			public AddFunction(Function f, Function g)
			{
				this.f = f;
				this.g = g;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return f.Value(evaluator) + g.Value(evaluator);
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return f.Derivative(variable) + g.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return f.PartialValue(evaluator) + g.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class SubtractFunction : Function
		{
			private Function f, g;

			public SubtractFunction(Function f, Function g)
			{
				this.f = f;
				this.g = g;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return f.Value(evaluator) - g.Value(evaluator);
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return f.Derivative(variable) - g.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return f.PartialValue(evaluator) - g.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class UnaryMinus : Function
		{
			private Function f;

			public UnaryMinus(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return -f.Value(evaluator);
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return -f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return -f.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class MultiplyFunction : Function
		{
			private Function f, g;

			public MultiplyFunction(Function f, Function g)
			{
				this.f = f;
				this.g = g;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return f.Value(evaluator) * g.Value(evaluator);
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return f.Derivative(variable) * g + f * g.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return f.PartialValue(evaluator) * g.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class MultiplyConstantFunction : Function
		{
			private Function f;
			private double a;

			public MultiplyConstantFunction(Function f, double a)
			{
				this.f = f;
				this.a = a;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return a * f.Value(evaluator);
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return a * f.Derivative(variable);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return a * f.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class DivideFunction : Function
		{
			private Function f, g;

			public DivideFunction(Function f, Function g)
			{
				this.f = f;
				this.g = g;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return f.Value(evaluator) / g.Value(evaluator);
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				//return (f.Derivative(variable) * g - f * g.Derivative(variable)) / Function.Sqr(g);
				return f.Derivative(variable) / g - f * g.Derivative(variable) / Function.Sqr(g);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return f.PartialValue(evaluator) / g.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class ReciprocalFunction : Function
		{
			private double a;
			private Function f;

			public ReciprocalFunction(double a, Function f)
			{
				this.a = a;
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				// Only really the reciprocal function when a=1.
				return a / f.Value(evaluator);
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return a * -f.Derivative(variable) / Function.Sqr(f);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return a / f.PartialValue(evaluator);
			}
		}

		[Serializable]
		private class SumFunction : Function
		{
			private Function[] functions;

			public SumFunction(params Function[] functions)
			{
				this.functions = (Function[])functions.Clone();
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				double value = 0.0;
				foreach (Function function in functions)
				{
					value += function.Value(evaluator);
				}

				return value;
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				List<Function> derivatives = new List<Function>();
				foreach (Function function in functions)
				{
					derivatives.Add(function.Derivative(variable));
				}

				return new SumFunction(derivatives.ToArray());
			}
		}
	}
}
