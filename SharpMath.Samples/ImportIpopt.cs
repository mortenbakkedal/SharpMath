using System;
using System.IO;
using System.Text;

namespace SharpMath.Samples
{
	public static class ImportIpopt
	{
		public static void Import()
		{
			// Rename 32/64-bit files so they can be present in the same directory. However, the Ipopt*.dll file
			// references IpOptFSS39.dll. Replace the reference in the binary file.
			File.Copy(@"D:\Udvikling\SharpMath\Ipopt\Ipopt-3.9.2-win32-win64-dll\Ipopt-3.9.2-win32-win64-dll\lib\win32\release\IpOptFSS39.dll", @"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\IpOptFSS32.dll", true);
			File.Copy(@"D:\Udvikling\SharpMath\Ipopt\Ipopt-3.9.2-win32-win64-dll\Ipopt-3.9.2-win32-win64-dll\lib\x64\release\IpOptFSS39.dll", @"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\IpOptFSS64.dll", true);
			BinaryReplace(@"D:\Udvikling\SharpMath\Ipopt\Ipopt-3.9.2-win32-win64-dll\Ipopt-3.9.2-win32-win64-dll\lib\win32\release\Ipopt39.dll", @"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\Ipopt32.dll", "IpOptFSS39", "IpOptFSS32");
			BinaryReplace(@"D:\Udvikling\SharpMath\Ipopt\Ipopt-3.9.2-win32-win64-dll\Ipopt-3.9.2-win32-win64-dll\lib\x64\release\Ipopt39.dll", @"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\Ipopt64.dll", "IpOptFSS39", "IpOptFSS64");
			
			// Replace back again to verify.
			BinaryReplace(@"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\Ipopt32.dll", @"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\Ipopt32_verify.dll", "IpOptFSS32", "IpOptFSS39");
			BinaryReplace(@"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\Ipopt64.dll", @"D:\Udvikling\SharpMath\SharpMath\SharpMath.Optimization.Ipopt\Ipopt64_verify.dll", "IpOptFSS64", "IpOptFSS39");
		}

		private static void BinaryReplace(string sourceFileName, string destinationFileName, string oldValue, string newValue)
		{
			// Assumes that the 8-bit encodes keep the binary content.
			string s = File.ReadAllText(sourceFileName, Encoding.GetEncoding(850));
			s = s.Replace(oldValue, newValue);
            File.WriteAllText(destinationFileName, s, Encoding.GetEncoding(850));
		}
	}
}
