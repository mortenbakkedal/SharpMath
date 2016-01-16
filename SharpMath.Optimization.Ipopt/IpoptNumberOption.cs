// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.Optimization.Ipopt.Cureos;

namespace SharpMath.Optimization.Ipopt
{
	[Serializable]
	public class IpoptNumberOption : IpoptOption
	{
		private double value;

		public IpoptNumberOption(string name, double value)
			: base(name)
		{
			this.value = value;
		}

		internal override void Prepare(IpoptProblem ipopt)
		{
			ipopt.AddOption(Name, value);
		}

		internal override IpoptOption Clone()
		{
			return new IpoptNumberOption(Name, value);
		}

		public override object Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = (double)value;
			}
		}
	}
}
