// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Represents a variable and the identity function.
	/// </summary>
	[Serializable]
	[DebuggerDisplay("{DebuggerDisplay}")]
	public partial class Variable : Function, IVariable
	{
		/// <summary>
		/// Creates a new variable.
		/// </summary>
		public Variable()
			: this(null)
		{
		}

		/// <summary>
		/// Creates a new named variable.
		/// </summary>
		public Variable(string name)
		{
			Name = name;
		}

		protected override double ComputeValue(IEvaluator evaluator)
		{
			// Should throw an exception if not assigned.
			return evaluator.Point[this];
		}

		protected override Function ComputeDerivative(Variable variable)
		{
			return variable == this ? 1.0 : 0.0;
		}

		protected override Function ComputePartialValue(IPartialEvaluator evaluator)
		{
			IPoint point = evaluator.Point;

			if (point.ContainsVariable(this))
			{
				// Replace this variable by the specified value.
				return point[this];
			}

			// This variable isn't replaced by a value. Just keep it.
			return this;
		}

		/// <summary>
		/// The name of the variable.
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				// Using this pattern: http://blogs.msdn.com/b/jaredpar/archive/2011/03/18/debuggerdisplay-attribute-best-practices.aspx
				return Name ?? "{" + GetType().FullName + "}";
			}
		}
	}
}
