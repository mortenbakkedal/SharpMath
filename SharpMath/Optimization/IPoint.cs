// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Represents a set of variables with some values assigned.
	/// </summary>
	public interface IPoint : IEnumerable<VariableAssignment>
	{
		/// <summary>
		/// Tests if this point contains this variable, i.e. if a value is assigned to it.
		/// </summary>
		bool ContainsVariable(Variable variable);

		/// <summary>
		/// The value assigned to a variable. Throws <see cref="VariableNotAssignedException" /> if not assigned.
		/// </summary>
		double this[Variable variable]
		{
			get;
		}

		/// <summary>
		/// Number of variables.
		/// </summary>
		int Count
		{
			get;
		}
	}
}
