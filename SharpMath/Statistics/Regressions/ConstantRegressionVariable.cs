// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Statistics.Regressions
{
	[Serializable]
	[DebuggerDisplay("(Const)")]
	public class ConstantRegressionVariable : IRegressionVariable
	{
		public ConstantRegressionVariable(bool required = false)
		{
			Required = required;
		}

		public ConstantRegressionVariable Require(bool required = true)
		{
			return new ConstantRegressionVariable(required);
		}

		public IEnumerable<double> Values
		{
			get
			{
				while (true)
				{
					yield return 1.0;
				}
			}
		}

		public bool Required
		{
			get;
			private set;
		}
	}

	[Serializable]
	[DebuggerDisplay("(Const)")]
	public class ConstantRegressionVariable<T> : ConstantRegressionVariable, IRegressionVariable<T>
	{
		public ConstantRegressionVariable(bool required = false)
			: base(required)
		{
		}

		public double Transform(T value)
		{
			return 1.0;
		}

		public new ConstantRegressionVariable<T> Require(bool required = true)
		{
			return new ConstantRegressionVariable<T>(required);
		}
	}
}
