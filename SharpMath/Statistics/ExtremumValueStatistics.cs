// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Statistics
{
	[Serializable]
	public class ExtremumValueStatistics<T>
	{
		public ExtremumValueStatistics()
		{
			MinValue = double.PositiveInfinity;
			MaxValue = double.NegativeInfinity;
			MinItem = default(T);
			MaxItem = default(T);
		}

		public ExtremumValueStatistics(IEnumerable<Tuple<double, T>> values)
			: this()
		{
			foreach (Tuple<double, T> value in values)
			{
				Add(value.Item1, value.Item2);
			}
		}

		public void Add(double value, T item)
		{
			if (value < MinValue)
			{
				MinValue = value;
				MinItem = item;
			}

			if (value > MaxValue)
			{
				MaxValue = value;
				MaxItem = item;
			}
		}

		public double MinValue
		{
			get;
			private set;
		}

		public double MaxValue
		{
			get;
			private set;
		}

		public T MinItem
		{
			get;
			private set;
		}

		public T MaxItem
		{
			get;
			private set;
		}
	}

	[Serializable]
	public class ExtremumValueStatistics
	{
		public ExtremumValueStatistics()
		{
			MinValue = double.PositiveInfinity;
			MaxValue = double.NegativeInfinity;
		}

		public ExtremumValueStatistics(IEnumerable<double> values)
			: this()
		{
			foreach (double value in values)
			{
				Add(value);
			}
		}

		public void Add(double value)
		{
			if (value < MinValue)
			{
				MinValue = value;
			}

			if (value > MaxValue)
			{
				MaxValue = value;
			}
		}

		public double MinValue
		{
			get;
			private set;
		}

		public double MaxValue
		{
			get;
			private set;
		}
	}
}
