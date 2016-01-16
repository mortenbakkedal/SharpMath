using System;

namespace SharpMath.Statistics.RandomNumbers
{
	/// <summary>
	/// Represents a stream of uniform random numbers.
	/// </summary>
	public interface IUniformStreamGenerator : IRandomGenerator
	{
		/// <summary>
		/// Skip to next substream for next iteration.
		/// </summary>
		void NextSubstream();

		void NextSubstream(int skip);

		void SetSeed(uint seed);
	}
}
