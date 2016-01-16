using System;

namespace SharpMath.Statistics.RandomNumbers
{
	public interface IPoissonGenerator
	{
		int NextPoisson(double lambda);
	}
}
