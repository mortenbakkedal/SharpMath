// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath.Optimization
{
	/// <summary>
	/// Generic thread-safe class for caching any type depending on <see cref="IPoint" />.
	/// </summary>
	/// <typeparam name="T">The type to cache, e.g. double or Matrix.</typeparam>
	public class PointCache<T>
	{
		private static int defaultSize;
		private int size;
		private Dictionary<IPoint, T> values;
		private LinkedList<IPoint> points;
		private object cacheLock;

		static PointCache()
		{
			// Read from configuration file instead?
			defaultSize = 100;
		}

		/// <summary>
		/// Creates a new instance of <see cref="PointCache" /> using default cache size as specified by <see cref="DefaultSize" />.
		/// </summary>
		public PointCache()
			: this(defaultSize)
		{
		}

		/// <summary>
		/// Creates a new instance of <see cref="PointCache" /> with a custom cache size.
		/// </summary>
		/// <param name="size">The size of the rolling cache.</param>
		public PointCache(int size)
		{
			this.size = size;

			values = new Dictionary<IPoint, T>(size);
			points = new LinkedList<IPoint>();
			cacheLock = new object();
		}

		public void Add(IPoint point, T value)
		{
			if (size == 0)
			{
				// No caching; avoid locking.
				return;
			}

			lock (cacheLock)
			{
				if (values.ContainsKey(point))
				{
					// Put last in queue for removal.
					points.Remove(point);
					points.AddLast(point);

					// Overwrite if changed for some reason.
					values[point] = value;
				}
				else
				{
					if (points.Count >= size)
					{
						values.Remove(points.First.Value);
						points.RemoveFirst();
					}

					values.Add(point, value);
					points.AddLast(point);
				}
			}
		}

		public bool TryGetValue(IPoint point, out T value)
		{
			if (size == 0)
			{
				value = default(T);
				return false;
			}

			lock (cacheLock)
			{
				return values.TryGetValue(point, out value);
			}
		}

		public static int DefaultSize
		{
			get
			{
				return defaultSize;
			}
			set
			{
				defaultSize = value;
			}
		}
	}
}
