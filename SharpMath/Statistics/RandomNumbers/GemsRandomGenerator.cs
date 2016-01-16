using System;

namespace SharpMath.Statistics.RandomNumbers
{
	[Serializable]
	public class GemsRandomGenerator : IRandomGenerator
	{
		private int seed, base_, multiplier, summand;

		public GemsRandomGenerator(int seed)
		{
			this.seed = seed;

			base_ = 99989;
			multiplier = 3893;
			summand = 15;
		}

		public int Next()
		{
			seed = (multiplier * seed + summand) % base_;
			return seed;
		}

		public double NextUniform()
		{
			return 1.0 * Next() / base_;
		}
	}
}
