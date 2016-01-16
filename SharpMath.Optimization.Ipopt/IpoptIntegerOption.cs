// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.Optimization.Ipopt.Cureos;

namespace SharpMath.Optimization.Ipopt
{
	[Serializable]
	public class IpoptIntegerOption : IpoptOption
	{
		private int value;

		public IpoptIntegerOption(string name, int value)
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
			return new IpoptIntegerOption(Name, value);
		}

		public override object Value
		{
			get
			{
				return value;
			}
			set
			{
				this.value = (int)value;
			}
		}
	}
}
