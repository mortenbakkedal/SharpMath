// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization.Ipopt
{
	[Serializable]
	public class IpoptOptionCollection : List<IpoptOption>
	{
		internal IpoptOptionCollection()
		{
		}

		public void Add(string name, int value)
		{
			Add(new IpoptIntegerOption(name, value));
		}

		public void Add(string name, double value)
		{
			Add(new IpoptNumberOption(name, value));
		}

		public void Add(string name, string value)
		{
			Add(new IpoptStringOption(name, value));
		}

		public void Remove(string name)
		{
			RemoveAll(delegate(IpoptOption option) { return option.Name == name; });
		}

		public bool Contains(string name)
		{
			return Find(delegate(IpoptOption option) { return option.Name == name; }) != null;
		}

		public object this[string name]
		{
			get
			{
				return Find(delegate(IpoptOption option) { return option.Name == name; }).Value;
			}
			set
			{
				Find(delegate(IpoptOption option) { return option.Name == name; }).Value = value;
			}
		}
	}
}
