// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace SharpMath.Collections
{
	[Serializable]
	[DebuggerDisplay("Count = {Count}")]
	public class ImmutableList<T> : IEnumerable<T>
	{
		private List<T> list;

		public ImmutableList(IEnumerable<T> items)
		{
			if (items == null)
			{
				throw new ArgumentNullException();
			}

			list = new List<T>(items);
		}

		public ImmutableList(params T[] items)
			: this((IEnumerable<T>)items)
		{
		}

		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			list.CopyTo(array, arrayIndex);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T this[int index]
		{
			get
			{
				return list[index];
			}
		}

		public int Count
		{
			get
			{
				return list.Count;
			}
		}
	}
}
