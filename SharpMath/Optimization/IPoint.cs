// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Represents a set of variables with some values assigned.
	/// </summary>
	public interface IPoint : IEnumerable<VariableAssignment>, IEquatable<IPoint>//interface here
	{
		/// <summary>
		/// Tests if this point contains this variable, i.e. if a value is assigned to it.
		/// </summary>
		bool ContainsVariable(Variable variable);//FIXME slet

		/// <summary>
		/// Gets the value assigned to a variable.
		/// </summary>
		double this[Variable variable]//IVariable here
		{
			get;
		}

		/// <summary>
		/// Number of variables.
		/// </summary>
		int Count//FIXME slet
		{
			get;
		}

		IEnumerable<IVariableAssignment> Assignments
		{
			get;
		}
    }
}
