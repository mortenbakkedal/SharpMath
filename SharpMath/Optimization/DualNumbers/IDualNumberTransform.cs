// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Optimization.DualNumbers
{
	/// <summary>
	/// The <see cref="DualNumber" /> representation of each <see cref="Variable" /> the function depends on.
	/// </summary>
	public interface IDualNumberTransform
	{
		/// <summary>
		/// The <see cref="DualNumber" /> representation of this variable.
		/// </summary>
		DualNumber this[Variable variable]
		{
			get;
		}

		/// <summary>
		/// The original variables.
		/// </summary>
		VariableCollection Variables
		{
			get;
		}
	}
}
