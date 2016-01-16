// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	public static class Evaluator
	{
		public static IEvaluator Create(IPoint point)
		{
			return new InnerEvaluator(point);
		}

		public static IEvaluator CreateCompact(IPoint point)
		{
			return new InnerCompactEvaluator(point);
		}

		public static IPartialEvaluator CreatePartial(IPoint point)
		{
			// It's important for efficiency that the same objects are reused whenever possible. Thus no compact memory
			// implementation of IPartialEvaluator (like InnerCompactEvaluator; but in that case only primitive types
			// are returned, i.e. doubles).
			return new InnerPartialEvaluator(point);
		}

		public static double Evaluate(IPoint point, Function function)
		{
			return Evaluate(point, new Function[] { function })[0];
		}

		public static double[] Evaluate(IPoint point, Function[] functions)
		{
			return Evaluate(Create(point), functions);
		}

		public static double EvaluateCompact(IPoint point, Function function)
		{
			return EvaluateCompact(point, new Function[] { function })[0];
		}

		public static double[] EvaluateCompact(IPoint point, Function[] functions)
		{
			return Evaluate(CreateCompact(point), functions);
		}

		public static Function PartialEvaluate(IPoint point, Function function)
		{
			return CreatePartial(point).Evaluate(function);
		}

		private static double[] Evaluate(IEvaluator evaluator, Function[] functions)
		{
			List<double> values = new List<double>();
			foreach (Function function in functions)
			{
				values.Add(evaluator.Evaluate(function));
			}

			return values.ToArray();
		}

		private class InnerEvaluator : IEvaluator
		{
			private Dictionary<Function, double> values;

			public InnerEvaluator(IPoint point)
			{
				Point = point;

				values = new Dictionary<Function, double>();
			}

			public double Evaluate(Function function)
			{
				double value;
				if (!values.TryGetValue(function, out value))
				{
					value = Function.ComputeValue(function, this);
					values.Add(function, value);
				}

				return value;
			}

			public IPoint Point
			{
				get;
				private set;
			}
		}

		private class InnerCompactEvaluator : IEvaluator
		{
			public InnerCompactEvaluator(IPoint point)
			{
				Point = point;
			}

			public double Evaluate(Function function)
			{
				// Don't store function values.
				return Function.ComputeValue(function, this);
			}

			public IPoint Point
			{
				get;
				private set;
			}
		}

		private class InnerPartialEvaluator : IPartialEvaluator
		{
			private Dictionary<Function, Function> values;

			public InnerPartialEvaluator(IPoint point)
			{
				Point = point;

				values = new Dictionary<Function, Function>();
			}

			public Function Evaluate(Function function)
			{
				Function value;
				if (!values.TryGetValue(function, out value))
				{
					value = Function.ComputePartialValue(function, this);
					values.Add(function, value);
				}

				return value;
			}

			public IPoint Point
			{
				get;
				private set;
			}
		}
	}
}
