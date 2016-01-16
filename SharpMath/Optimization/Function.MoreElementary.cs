// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	public partial class Function
	{
		/// <summary>
		/// Positive part function g(x)=f(x)^+=max(0,f(x)).
		/// </summary>
		public static Function Positive(Function f)
		{
			return new PositiveFunction(f);
		}

		/// <summary>
		/// Heaviside step function.
		/// </summary>
		public static Function Step(Function f)
		{
			return new StepFunction(f);
		}

		/// <summary>
		/// Absolute value function.
		/// </summary>
		public static Function Abs(Function f)
		{
			return new AbsFunction(f);
		}

		[Serializable]
		private class PositiveFunction : Function
		{
			private Function f;

			public PositiveFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				double x = f.Value(evaluator);
				return x > 0.0 ? x : 0.0;
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return new StepFunction(f);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Positive(f.PartialValue(evaluator));
			}
		}

		[Serializable]
		private class StepFunction : Function
		{
			private Function f;

			public StepFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				double x = f.Value(evaluator);
				if (x < 0.0)
				{
					return 0.0;
				}
				else if (x > 0.0)
				{
					return 1.0;
				}
				else
				{
					// Not defined at zero.
					return double.NaN;
				}
			}

			protected override Function ComputeDerivative(Variable variable)
			{
				return new StepDerivativeFunction(f);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Step(f.PartialValue(evaluator));
			}
		}

		[Serializable]
		private class StepDerivativeFunction : Function
		{
			private Function f;

			public StepDerivativeFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return f.Value(evaluator) != 0.0 ? 0.0 : double.NaN;
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				// As far as not being defined at zero, all derivatives are the same.
				return this;
			}
		}

		[Serializable]
		private class AbsFunction : Function
		{
			private Function f;

			public AbsFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				return Math.Abs(f.Value(evaluator));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				return new AbsDerivativeFunction(f);
			}

			protected override Function ComputePartialValue(IPartialEvaluator evaluator)
			{
				return Function.Abs(f.PartialValue(evaluator));
			}
		}

		[Serializable]
		private class AbsDerivativeFunction : Function
		{
			private Function f;

			public AbsDerivativeFunction(Function f)
			{
				this.f = f;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				double x = f.Value(evaluator);
				if (x < 0.0)
				{
					return -1.0;
				}
				else if (x > 0.0)
				{
					return 1.0;
				}
				else
				{
					// Not defined at zero.
					return double.NaN;
				}
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				// Same derivative as that of the step function.
				return new StepDerivativeFunction(f);
			}
		}
	}
}
