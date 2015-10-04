// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Base class for non-linear optimizers.
	/// </summary>
	[Serializable]
	public abstract class Optimizer : IOptimizer
	{
		public Optimizer()
		{
			ObjectiveFunctionScaling = 1.0;
			Variables = new VariableCollection();
			VariableEqualityConstraints = new VariableEqualityConstraintCollection();
		}

		/// <summary>
		/// Run the optimizer with the specified initial values.
		/// </summary>
		public IOptimizerResult Run(IPoint initialPoint)
		{
			// It's easier to use the prepared optimizer object though we're only using it once in this case.
			return Prepare().Run(initialPoint);
		}

		/// <summary>
		/// Run the optimizer with the specified initial values.
		/// </summary>
		public IOptimizerResult Run(params VariableAssignment[] initialAssignments)
		{
			return Run(new Point(initialAssignments));
		}

		/// <summary>
		/// Use the returned object for multiple runs with different initial values.
		/// </summary>
		public abstract PreparedOptimizer Prepare();

		/// <summary>
		/// Tests whether all constraints are satisfied. This method isn't needed initially for some optimizers (e.g. Ipopt),
		/// and it may also fail slightly for some after optimization (again Ipopt).
		/// </summary>
		public bool CheckConstraints(IPoint point)
		{
			try
			{
				// Use the prepared object like in the Run method.
				((InnerOptimizer)Prepare()).CheckConstraints(point);
			}
			catch (ConstraintViolationException)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// The IEvaluator object used to compute the function value before each iteration. Override this to change the default behavior,
		/// i.e. using a different IEvaluator object than Evaluator.InnerEvaluation.
		/// </summary>
		protected virtual IEvaluator CreateEvaluator(IDictionary<Variable, double> assignments)
		{
			return Evaluator.Create(new Point(VariableAssignment.Create(assignments)));
		}

		/// <summary>
		/// The IPoint object returned as the final optimized point in IOptimizerResult. Override this to change the default behavior, i.e.
		/// using a different IPoint object than Point.
		/// </summary>
		protected virtual IPoint CreateOptimalPoint(IDictionary<Variable, double> assignments)
		{
			return new Point(VariableAssignment.Create(assignments));
		}

		/// <summary>
		/// The function to optimize.
		/// </summary>
		public Function ObjectiveFunction
		{
			get;
			set;
		}

		/// <summary>
		/// Use a positive number to perform minimization (default) or a negative number to perform maximization.
		/// </summary>
		public double ObjectiveFunctionScaling
		{
			get;
			set;
		}

		/// <summary>
		/// The unknown variables to optimize.
		/// </summary>
		public VariableCollection Variables
		{
			get;
			private set;
		}

		/// <summary>
		/// Variables added here using the == operator overload are removed from the optimization and replaced by the values specified.
		/// </summary>
		public VariableEqualityConstraintCollection VariableEqualityConstraints
		{
			get;
			private set;
		}

		[Serializable]
		protected abstract class InnerOptimizer : PreparedOptimizer
		{
			public InnerOptimizer(Optimizer optimizer)
				: base(optimizer)
			{
				ObjectiveFunction = optimizer.ObjectiveFunction;
				ObjectiveFunctionScaling = optimizer.ObjectiveFunctionScaling;

				if (ObjectiveFunction == null)
				{
					throw new OptimizerException("No objective function is specified.");
				}

				Variables = new List<Variable>();

				foreach (Variable variable in optimizer.Variables)
				{
					// Avoid duplicates. Not an error, just ignore them.
					if (!Variables.Contains(variable))
					{
						Variables.Add(variable);
					}
				}

				VariableEqualityConstraints = new Dictionary<Variable, double>();

				foreach (VariableEqualityConstraint constraint in optimizer.VariableEqualityConstraints)
				{
					AddVariableEqualityConstraint(constraint);
				}
			}

			public void AddVariableEqualityConstraint(VariableEqualityConstraint constraint)
			{
				Variable variable = constraint.Variable;
				double value = constraint.Value;

				if (VariableEqualityConstraints.ContainsKey(variable))
				{
					// Previously defined. Check that it's set to the same value.
					if (VariableEqualityConstraints[variable] != value)
					{
						throw new InconsistentConstraintException("Inconsistency found in variable equality constraints.", constraint);
					}
				}
				else if (Variables.Contains(variable))
				{
					// Remove the variable from the optimization.
					Variables.Remove(variable);
					VariableEqualityConstraints.Add(variable, value);
				}
				else
				{
					throw new InconsistentConstraintException("A variable equality constraint is specified on a variable that's not included in the optimization.", constraint);
				}
			}

			public virtual void CheckConstraints(IPoint point)
			{
				CheckVariableEqualityConstraints(point);
			}

			public virtual void CheckVariableEqualityConstraints(IPoint point)
			{
				foreach (KeyValuePair<Variable, double> constraint in VariableEqualityConstraints)
				{
					Variable variable = constraint.Key;
					if (point.ContainsVariable(variable) && point[variable] != constraint.Value)
					{
						throw new ConstraintViolationException("A variable equality constraint is violated.");
					}
				}
			}

			public IEvaluator CreateEvaluator(double[] values)
			{
				return Optimizer.CreateEvaluator(CreateDictionary(values));
			}

			public IPoint CreateOptimalPoint(double[] values)
			{
				return Optimizer.CreateOptimalPoint(CreateDictionary(values));
			}

			private IDictionary<Variable, double> CreateDictionary(double[] values)
			{
				int n = Variables.Count;

				if (values.Length != n)
				{
					throw new OptimizerException();
				}

				Dictionary<Variable, double> assignments = new Dictionary<Variable, double>(VariableEqualityConstraints);
				for (int i = 0; i < n; i++)
				{
					assignments[Variables[i]] = values[i];
				}

				return assignments;
			}

			public Function ObjectiveFunction
			{
				get;
				private set;
			}

			public double ObjectiveFunctionScaling
			{
				get;
				private set;
			}

			public List<Variable> Variables
			{
				get;
				private set;
			}

			public Dictionary<Variable, double> VariableEqualityConstraints
			{
				get;
				private set;
			}
		}
	}
}
