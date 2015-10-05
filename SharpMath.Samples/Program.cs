// Copyright (c) 2015 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.Samples.Optimization;

namespace SharpMath.Samples
{
	public class Program
	{
		public static void Main(string[] args)
		{
			ImportIpopt.Import();
			/*OptimizerSamples.Sample2();

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
			}*/

			/*string s = System.IO.File.ReadAllText(@"D:\Udvikling\SharpMath\SharpMath.Optimization.Ipopt\Ipopt39_ORG.dll", System.Text.Encoding.GetEncoding(850));
			s = s.Replace("IpOptFSS39", "IpOptFSS64");
			System.IO.File.WriteAllText(@"D:\Udvikling\SharpMath\SharpMath.Optimization.Ipopt\Ipopt64.dll", s, System.Text.Encoding.GetEncoding(850));*/


			Console.WriteLine("Done");
			Console.ReadKey();
		}
	}
}
