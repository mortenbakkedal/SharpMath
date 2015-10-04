// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	public class VariableCollection : List<Variable>
	{
		public void Add(IEnumerable<Variable> variables)
		{
			AddRange(variables);
		}

		public void Add(params Variable[] variables)
		{
			Add((IEnumerable<Variable>)variables);
		}
	}
}
