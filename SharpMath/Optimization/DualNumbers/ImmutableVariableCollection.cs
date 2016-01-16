// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Represents a immutable collection of unique variables, i.e. each variable is only allowed to be present once in the collection.
	/// </summary>
	[Serializable]
	public class ImmutableVariableCollection : IEnumerable<Variable> // Or IImmutableList<Variable> in .NET 4.5
	{
		private List<Variable> variables;
		private Dictionary<Variable, int> indices;

		private ImmutableVariableCollection()
		{
			variables = new List<Variable>();
			indices = new Dictionary<Variable, int>();
		}

		private ImmutableVariableCollection(IEnumerable<Variable> variables)
			: this()
		{
			foreach (Variable variable in variables)
			{
				Add(variable);
			}
		}

		/// <summary>
		/// Creates a new collection of variables. Duplicated variables are ignored.
		/// </summary>
		public static ImmutableVariableCollection Create(IEnumerable<Variable> variables)
		{
			return new ImmutableVariableCollection(variables);
		}

		/// <summary>
		/// Creates a new collection of variables. Duplicated variables are ignored.
		/// </summary>
		public static ImmutableVariableCollection Create(params Variable[] variables)
		{
			return Create((IEnumerable<Variable>)variables);
		}

		private void Add(Variable variable)
		{
			// Intentionally kept private. The class is immutable by design.

			if (variable == null)
			{
				throw new NullReferenceException();
			}

			if (!Contains(variable))
			{
				int index = variables.Count;
				variables.Add(variable);
				indices.Add(variable, index);
			}
		}

		public bool Contains(Variable variable)
		{
			return indices.ContainsKey(variable);
		}

		public int IndexOf(Variable variable)
		{
			int index;
			if (!indices.TryGetValue(variable, out index))
			{
				index = -1;
			}

			return index;
		}

		public IEnumerator<Variable> GetEnumerator()
		{
			return variables.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return (IEnumerator)GetEnumerator();
		}

		public Variable this[int index]
		{
			get
			{
				return variables[index];
			}
		}

		public int Count
		{
			get
			{
				return variables.Count;
			}
		}
	}
}
