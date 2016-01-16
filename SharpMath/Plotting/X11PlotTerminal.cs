// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Diagnostics;

namespace SharpMath.Plotting
{
	[Serializable]
	public class X11PlotTerminal : IPlotTerminal
	{
		public void Generate(Gnuplot plot)
		{
			if (SupportsMouseClose())
			{
				// wxt terminal no longer supported in Debian Jessie. Using gnuplot-qt instead.
				// https://launchpad.net/debian/jessie/+source/gnuplot/+changelog
				// [4d67729] Disable wxt-terminal. (Closes: #750045)
				// [7342432] Increase priority of gnuplot-qt over gnuplot-x11.
				plot.Add("set terminal qt");
			}

			// Allow dashed lines, but remember to keep default solid lines.
			plot.Add("set termoption dashed");
		}

		public void FinalizeTerminal()
		{
		}

		private static bool SupportsMouseClose()
		{
			using (Process p = new Process())
			{
				p.StartInfo.FileName = "gnuplot";
				p.StartInfo.Arguments = "--version";
				p.StartInfo.UseShellExecute = false;
				p.StartInfo.RedirectStandardOutput = true;
				p.StartInfo.RedirectStandardError = true;

				p.Start();
				p.WaitForExit();

				if (p.ExitCode != 0)
				{
					return false;
				}

				// Bug not fixed in this version.
				// http://sourceforge.net/p/gnuplot/bugs/1489/
				if (p.StandardOutput.ReadToEnd().Trim() == "gnuplot 4.6 patchlevel 0")
				{
					return false;
				}
			}

			return true;
		}
	}
}
