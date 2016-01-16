using System;
using System.Collections.Generic;

namespace SharpMath.Collections
{
	[Serializable]
	public class CyclicReferenceException<T> : Exception
	{
		public CyclicReferenceException(string message, IEnumerable<T> cycle)
			: base(message)
		{
			if (cycle != null)
			{
				Cycle = new ImmutableList<T>(cycle);
			}
		}

		public ImmutableList<T> Cycle
		{
			get;
			private set;
		}
	}
}
