using System;
using System.Diagnostics;
using System.IO;

namespace SharpMath
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class TempDirectory : IDisposable
	{
		private DirectoryInfo directory;

		public TempDirectory()
		{
			// This is not race-condition safe, but probably good enough.
			do
			{
				directory = new DirectoryInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
			} while (directory.Exists || File.Exists(directory.FullName));

			directory.Create();
		}

		~TempDirectory()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			// Following this dispose pattern: http://stackoverflow.com/a/898867

			try
			{
				directory.Delete(true);
			}
			catch (IOException)
			{
				// Ignore IO exceptions in finalizer.
			}
		}

		public DirectoryInfo Directory
		{
			get
			{
				return directory;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				return directory.FullName;
			}
		}
	}
}
