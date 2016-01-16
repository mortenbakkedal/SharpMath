using System;
using System.Diagnostics;
using System.IO;

namespace SharpMath
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public class TempFile : IDisposable
	{
		private FileInfo file;

		public TempFile()
		{
			// This is not race-condition safe, but probably good enough.
			do
			{
				file = new FileInfo(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));
			} while (file.Exists || Directory.Exists(file.FullName));

			//file = new FileInfo(Path.GetTempFileName());
		}

		~TempFile()
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
				file.Delete();
			}
			catch (IOException)
			{
				// Ignore exceptions in finalizer.
			}
		}

		public FileInfo File
		{
			get
			{
				return file;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private string DebuggerDisplay
		{
			get
			{
				return file.FullName;
			}
		}
	}
}
