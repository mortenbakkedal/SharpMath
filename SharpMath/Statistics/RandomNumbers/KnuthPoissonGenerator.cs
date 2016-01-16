using System;

namespace SharpMath.Statistics.RandomNumbers
{
	public class KnuthPoissonGenerator : IPoissonGenerator
	{
		private IRandomGenerator uniformGenerator;

		public KnuthPoissonGenerator(IRandomGenerator uniformGenerator)
		{
			this.uniformGenerator = uniformGenerator;
		}

		public int NextPoisson(double lambda)
		{
			// FIXME Rejection sampling for large lambda?

			if (lambda < 0.0)
			{
				throw new ArgumentException("Poisson intensity must be non-negative.", "lambda");
			}

			// Using Knuth's algorithm:
			// http://en.wikipedia.org/w/index.php?title=Poisson_distribution&oldid=412436229#Generating_Poisson-distributed_random_variables

			double h = Math.Exp(-lambda);
			int k = 0;
			double p = 1.0;

			do
			{
				k++;
				p *= uniformGenerator.NextUniform();
			} while (p > h);

			return k - 1;
		}
	}
}
