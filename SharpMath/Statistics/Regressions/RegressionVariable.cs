// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SharpMath.LinearAlgebra;

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	[DebuggerDisplay("{Name}")]
	public class RegressionVariable : IRegressionVariable
	{
		public RegressionVariable(Vector values, bool required = false)
			: this(null, values, required)
		{
		}

		public RegressionVariable(IEnumerable<double> values, bool required = false)
			: this(null, values, required)
		{
		}

		public RegressionVariable(string name, Vector values, bool required = false)
		{
			Name = name;
			Values = values;
			Required = false;
		}

		public RegressionVariable(string name, IEnumerable<double> values, bool required = false)
			: this(name, new Vector(values), required)
		{
		}

		public RegressionVariable Require(bool required = true)
		{
			return new RegressionVariable(Name, Values, required);
		}

		public Vector Values
		{
			get;
			private set;
		}

		IEnumerable<double> IRegressionVariable.Values
		{
			get
			{
				return Values.ToArray(); // FIXME
			}
		}

		public string Name
		{
			get;
			private set;
		}

		public bool Required
		{
			get;
			private set;
		}
	}

	[Serializable]
	[DebuggerDisplay("{Name}")]
	public class RegressionVariable<T> : RegressionVariable, IRegressionVariable<T>
	{
		private Func<T, double> transform;

		private RegressionVariable(Vector values, Func<T, double> transform, bool required)
			: base(values, required)
		{
			this.transform = transform;
		}

		public RegressionVariable(IEnumerable<T> values, Func<T, double> transform, bool required = false)
			: this(null, values, transform, required)
		{
		}

		public RegressionVariable(string name, IEnumerable<T> values, Func<T, double> transform, bool required = false)
			: base(name, values.Select(v => transform(v)), required)
		{
			this.transform = transform;
		}

		public double Transform(T value)
		{
			return transform(value);
		}

		public new RegressionVariable<T> Require(bool required = true)
		{
			return new RegressionVariable<T>(Values, transform, required);
		}
	}
}
