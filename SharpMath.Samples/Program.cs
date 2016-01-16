// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.Samples.Optimization;

namespace SharpMath.Samples
{
	public class Program
	{
		public static void Main(string[] args)
		{
			OptimizerSamples.Sample2();

			if (IntPtr.Size == 4)
			{
				Console.WriteLine("32-bit");
			}
			else if (IntPtr.Size == 8)
			{
				Console.WriteLine("64-bit");
			}
			else
			{
				// The future is now!
			}

			Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
