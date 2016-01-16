using System;
using System.Collections.Generic;

namespace SharpMath.Collections
{
	public static class CollectionExtensions
	{
		public static IEnumerable<T> TopologicalSort<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle = true)
		{
			LinkedList<T> sorted = new LinkedList<T>();
			HashSet<T> sortedHashSet = new HashSet<T>();
			HashSet<T> visited = new HashSet<T>();

			foreach (var item in source)
			{
				Stack<T> cycle = new Stack<T>();
				Visit(item, visited, sorted, sortedHashSet, dependencies, throwOnCycle, cycle);
			}

			return sorted;
		}

		private static void Visit<T>(T item, HashSet<T> visited, LinkedList<T> sorted, HashSet<T> sortedHashSet, Func<T, IEnumerable<T>> dependencies, bool throwOnCycle, Stack<T> cycle)
		{
			cycle.Push(item);

			if (!visited.Contains(item))
			{
				visited.Add(item);

				foreach (var dependency in dependencies(item))
				{
					Visit(dependency, visited, sorted, sortedHashSet, dependencies, throwOnCycle, cycle);
				}

				sorted.AddLast(item);
				sortedHashSet.Add(item);
			}
			else
			{
				if (throwOnCycle && !sortedHashSet.Contains(item))
				{
					throw new CyclicReferenceException<T>("Cyclic dependency found.", cycle); // FIXME item -> x1 -> x2 -> ... -> item
				}
			}

			cycle.Pop();
		}
	}
}
