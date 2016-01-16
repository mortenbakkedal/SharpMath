// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Base class for preparing optimizers. Allows the optimizer to copy parameters to an internal storage to avoid changes during
	/// the optimization and to perform pre-analysis of the problem, e.g. eliminating variables being fixed by some constraints or
	/// determining the derivative structure. The actual optimization may be implemented by overriding this class.
	/// </summary>
	[Serializable]
	public abstract class PreparedOptimizer : IOptimizer
	{
		protected PreparedOptimizer(Optimizer optimizer)
		{
			Optimizer = optimizer;
		}

		/// <summary>
		/// Run the prepared optimizer with the specified initial values.
		/// </summary>
		public abstract IOptimizerResult Run(IPoint initialPoint);

		/// <summary>
		/// Run the prepared optimizer with the specified initial values.
		/// </summary>
		public IOptimizerResult Run(params VariableAssignment[] initialAssignments)
		{
			return Run(new Point(initialAssignments));
		}

		/// <summary>
		/// The original optimizer instance.
		/// </summary>
		public Optimizer Optimizer
		{
			get;
			private set;
		}
	}
}
