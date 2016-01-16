// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Base class for non-linear optimizers with non-linear constraints.
	/// </summary>
	[Serializable]
	public abstract class ConstrainedOptimizer : VariableConstrainedOptimizer
	{
		public ConstrainedOptimizer()
		{
			Constraints = new FunctionConstraintCollection();
		}

		/// <summary>
		/// Any function or variable may be added here to constrain the optimization. Functions are added using inequality operator overloads.
		/// </summary>
		public FunctionConstraintCollection Constraints
		{
			get;
			private set;
		}

		[Serializable]
		protected abstract class InnerConstrainedOptimizer : InnerVariableConstrainedOptimizer
		{
			public InnerConstrainedOptimizer(ConstrainedOptimizer optimizer)
				: base(optimizer)
			{
				FunctionConstraints = new List<FunctionConstraint>();

				foreach (FunctionConstraint constraint in optimizer.Constraints)
				{
					AddFunctionConstraint(constraint);
				}
			}

			public void AddFunctionConstraint(FunctionConstraint constraint)
			{
				if (constraint is VariableConstraint)
				{
					// Not really a function constraint (i.e. general non-linear constraint). Let the base class take care of it.
					AddVariableConstraint((VariableConstraint)constraint);
					return;
				}

				Function function = constraint.Function;

				if (function is Variable)
				{
					// Same as above.
					AddVariableConstraint(new VariableConstraint((Variable)function, constraint.MinValue, constraint.MaxValue));
					return;
				}

				FunctionConstraint constraint0 = FunctionConstraints.Find(delegate(FunctionConstraint c) { return c.Function == function; });
				if (constraint0 != null)
				{
					// Modify an existing constraint (i.e. regarding the same variable).
					double minValue = Math.Max(constraint0.MinValue, constraint.MinValue);
					double maxValue = Math.Min(constraint0.MaxValue, constraint.MaxValue);

					if (minValue <= maxValue)
					{
						// Remove the existing constraint and add a new with a smaller interval.
						FunctionConstraints.Remove(constraint0);
						FunctionConstraints.Add(new FunctionConstraint(function, minValue, maxValue));
					}
					else
					{
						// The two intervals are disjoint.
						throw new InconsistentConstraintException("Inconsistency found in constraints.", constraint);
					}
				}
				else
				{
					FunctionConstraints.Add(constraint);
				}
			}

			public override void CheckConstraints(IPoint point)
			{
				base.CheckConstraints(point);

				CheckFunctionConstraints(point);
			}

			public virtual void CheckFunctionConstraints(IPoint point)
			{
				foreach (FunctionConstraint constraint in FunctionConstraints)
				{
					double value = constraint.Function.Value(point);
					if (value < constraint.MinValue || value > constraint.MaxValue)
					{
						throw new ConstraintViolationException("A constraint isn't satisfied.", constraint);
					}
				}
			}

			public List<FunctionConstraint> FunctionConstraints
			{
				get;
				private set;
			}
		}
	}
}
