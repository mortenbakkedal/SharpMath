// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Optimization
{
	[Serializable]
	[DebuggerDisplay("{Constant}")]
	public class ConstantFunction : Function
	{
		public ConstantFunction(double constant)
		{
			Constant = constant;
		}

		protected override double ComputeValue(IEvaluator evaluator)
		{
			return Constant;
		}

		protected override Function ComputeDerivative(Variable variable)
		{
			return 0.0;
		}

		protected override Function ComputePartialValue(IPartialEvaluator evaluator)
		{
			return this;
		}
						
		public double Constant
		{
			get;
			private set;
		}
	}
}
