// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Represents a mathematical function. Any function must derive from this class. Also includes a range
	/// of static method for performing operations on functions.
	/// </summary>
	[Serializable]
	public abstract partial class Function
	{
		private Dictionary<Variable, Function> derivatives;

		/// <summary>
		/// Evaluates the function by assigning values to the variables.
		/// </summary>
		public double Value(IPoint point)
		{
			// Evaluate using the cached Evaluate method by default (usually much faster, but uses more memory).
			return Evaluator.Evaluate(point, this);
		}

		/// <summary>
		/// Evaluates the function by assigning values to the variables.
		/// </summary>
		public double Value(params VariableAssignment[] assignments)
		{
			return Value(new Point(assignments));
		}

		/// <summary>
		/// Evaluates the function by assigning values to the variables.
		/// </summary>
		public double Value(IEvaluator evaluator)
		{
			// Consider removing this extra call?
			return evaluator.Evaluate(this);
		}
				
		/// <summary>
		/// Evaluates the function by assigning values to the variables. Override this to implement.
		/// </summary>
		protected abstract double ComputeValue(IEvaluator evaluator);

		/// <summary>
		/// Evaluates the function by assigning values to the variables.
		/// </summary>
		public static double ComputeValue(Function function, IEvaluator evaluator)
		{
			return function.ComputeValue(evaluator);
		}

		/// <summary>
		/// Computes the partial derivative with respect to a variable.
		/// </summary>
		public Function Derivative(Variable variable)
		{
			// It's has proved very important for efficient computation that the same object is returned
			// if called repeatably. This is especially true if the function is compiled.

			if (derivatives == null)
			{
				derivatives = new Dictionary<Variable, Function>();
			}

			Function derivative;
			if (!derivatives.TryGetValue(variable, out derivative))
			{
				derivative = ComputeDerivative(variable);
				if (!derivative.IsZero)
				{
					// Only use memory for saving non-zero derivatives.
					derivatives.Add(variable, derivative);
				}
			}

			return derivative;
		}

		/// <summary>
		/// Computes the higher order partial derivative with respect to a variable.
		/// </summary>
		public Function Derivative(Variable variable, int order)
		{
			if (order < 0)
			{
				throw new ArgumentOutOfRangeException("order");
			}

			Function f = this;
			for (int i = 0; i < order; i++)
			{
				f = f.Derivative(variable);
			}
			return f;
		}

		/// <summary>
		/// Computes the higher order partial derivative with respect to a number of variables.
		/// </summary>
		public Function Derivative(params Variable[] variables)
		{
			Function f = this;
			for (int i = 0; i < variables.Length; i++)
			{
				f = f.Derivative(variables[i]);
			}
			return f;
		}

		/// <summary>
		/// Computes the partial derivative with respect to a variable. Override this to implement.
		/// </summary>
		protected abstract Function ComputeDerivative(Variable variable);

		/// <summary>
		/// Replaces a number of variables by fixed values. Returns a function of the remaining variables.
		/// </summary>
		public Function PartialValue(IPoint point)
		{
			return Evaluator.PartialEvaluate(point, this);
		}

		/// <summary>
		/// Replaces a number of variables by fixed values. Returns a function of the remaining variables.
		/// </summary>
		public Function PartialValue(params VariableAssignment[] assignments)
		{
			return PartialValue(new Point(assignments));
		}

		/// <summary>
		/// Replaces a number of variables by fixed values. Returns a function of the remaining variables.
		/// </summary>
		public Function PartialValue(IPartialEvaluator evaluator)
		{
			return evaluator.Evaluate(this);
		}

		/// <summary>
		/// Substitutes a number of variables by function values. The chain rule is applied. Returns a function of the remaining variables and the variables introduced implicitly through the substitution.
		/// </summary>
		public Function Substitute(params VariableFunctionAssignment[] assignments)
		{
			throw new NotImplementedException();
			//return PartialValue(new Point(assignments));
		}

		/// <summary>
		/// Substitutes a number of variables by function values. The chain rule is applied. Returns a function of the remaining variables and the variables introduced implicitly through the substitution.
		/// </summary>
		public Function Substitute(IPartialEvaluator evaluator)
		{
			throw new NotImplementedException();
			//return evaluator.Evaluate(this);
		}

		/// <summary>
		/// Replaces a number of variables by fixed values. Override this to implement.
		/// </summary>
		protected virtual Function ComputePartialValue(IPartialEvaluator evaluator)
		{
			// Use slow and inefficient implementation using PartialValueFunction by default. Override to provide a more efficient implementation.
			return new PartialValueFunction(this, evaluator);
		}

		/// <summary>
		/// Replaces a number of variables by fixed values.
		/// </summary>
		public static Function ComputePartialValue(Function function, IPartialEvaluator evaluator)
		{
			return function.ComputePartialValue(evaluator);
		}

		/// <summary>
		/// Tests if this function is known to be identically zero. Override this to express knowledge about zeroness.
		/// </summary>
		public virtual bool IsZero
		{
			get
			{
				ConstantFunction f0 = this as ConstantFunction;
				return f0 != null && f0.Constant == 0.0;
			}
		}

		[Serializable]
		private class PartialValueFunction : Function
		{
			private Function innerFunction;
			private IPartialEvaluator partialEvaluator;

			public PartialValueFunction(Function innerFunction, IPartialEvaluator partialEvaluator)
			{
				this.innerFunction = innerFunction;
				this.partialEvaluator = partialEvaluator;
			}

			protected override double ComputeValue(IEvaluator evaluator)
			{
				Dictionary<Variable, double> assignments = new Dictionary<Variable, double>();

				// Add values corresponding to the variable not being assignmed by the partial evaluation.
				foreach (VariableAssignment assignment in evaluator.Point)
				{
					assignments[assignment.Variable] = assignment.Value;
				}

				// Overwrite or extend with values defined for the partial value.
				foreach (VariableAssignment assignment in partialEvaluator.Point)
				{
					assignments[assignment.Variable] = assignment.Value;
				}

				return innerFunction.Value(new Point(VariableAssignment.Create(assignments)));
			}
			
			protected override Function ComputeDerivative(Variable variable)
			{
				if (partialEvaluator.Point.ContainsVariable(variable))
				{
					// This variable is replaced by a constant.
					return 0.0;
				}

				return innerFunction.Derivative(variable);
			}
		}
	}
}
