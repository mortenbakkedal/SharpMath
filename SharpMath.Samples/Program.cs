// SharpMath - C# Mathematical Library
// Copyright (c) 2015 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SharpMath.Samples.Optimization;

namespace SharpMath.Samples
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//Scan(new DirectoryInfo(@"D:\Udvikling\SharpMath\SharpMath")); return;

			//ImportIpopt.Import();
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

		private static void Scan(DirectoryInfo d0)
		{
			foreach (DirectoryInfo d in d0.GetDirectories())
			{
				Scan(d);
			}

			foreach (FileInfo f in d0.GetFiles().Where(f2 => f2.Name.EndsWith(".cs")))
			{
				if (!f.FullName.EndsWith(".cs"))
				{
					continue;
				}
				List<string> lines = File.ReadAllLines(f.FullName).ToList();
				if (lines.Count == 0)
				{
					continue;
				}

				if (lines[0].StartsWith("using "))
				{
					lines.Insert(0, "");
					lines.Insert(0, "// This code is published under the MIT License.");
					lines.Insert(0, "// Copyright (c) 2016 Morten Bakkedal");
				}
				if (!lines[0].Contains("SharpMath - C#"))
				{
					lines.Insert(0, "");
				}
				else
				{
				}

				lines[0] = "// SharpMath - C# Mathematical Library";

				File.WriteAllLines(f.FullName, lines.ToArray());
			}
		}
	}
}
