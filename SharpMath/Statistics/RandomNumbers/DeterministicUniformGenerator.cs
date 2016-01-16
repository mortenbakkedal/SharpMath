using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpMath.Statistics.RandomNumbers
{
	[Serializable]
	public class DeterministicUniformGenerator : IRandomGenerator
	{
		private double[] values;
		private int index;

		public DeterministicUniformGenerator(IEnumerable<double> values)
		{
			this.values = values.ToArray();

			index = 0;
		}

		public static DeterministicUniformGenerator[] Load(string fileName)
		{
			throw new NotImplementedException();
		}

		//public static void ParseCsv()
	
		public double NextUniform()
		{
			return values[index++];
		}
	}
}
