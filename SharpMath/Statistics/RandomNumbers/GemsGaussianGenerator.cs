using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace SharpMath.Statistics.RandomNumbers
{
	[Serializable]
	public class GemsGaussianGenerator
	{
		private Dictionary<Tuple<string, int>, List<double>> values;

		private GemsGaussianGenerator(Dictionary<Tuple<string, int>, List<double>> values)
		{
			this.values = values;
		}

		public static GemsGaussianGenerator Load()
		{
			return Load(@"H:\Lorenza\CIR reproduction_Morten_GemsWorkbook2.csv");
		}

		public static GemsGaussianGenerator Load(string fileName)
		{
			string[] lines = File.ReadLines(fileName).ToArray();
			string[] labels = lines.First().Split(',').Skip(4).ToArray();

			int n = labels.Length;
			Dictionary<Tuple<string, int>, List<double>> values = new Dictionary<Tuple<string, int>, List<double>>();

			int previousPath = -1;
			foreach (string line in lines.Skip(2))
			{
				string[] columns = line.Split(',');
				if (columns[1] == "2015")
				{
					continue;
				}
				int path = int.Parse(columns[0]);
				if (path != previousPath)
				{
					previousPath = path;
					for (int i = 0; i < n; i++)
					{
						values.Add(Tuple.Create<string, int>(labels[i], path), new List<double>());
					}
				}

				for (int i = 0; i < n; i++)
				{
					values[Tuple.Create<string, int>(labels[i], path)].Add(double.Parse(columns[i + 4], CultureInfo.InvariantCulture));
				}
			}

			//return labels.Select(s => new GemsGaussianGenerator(values[s])).ToArray();
			return new GemsGaussianGenerator(values);
		}

		public IGaussianRandomGenerator Prepare(string label, int path)
		{
			return new InnerGenerator(values[Tuple.Create<string, int>(label, path)]);
		}
		
		[Serializable]
		private class InnerGenerator : IGaussianRandomGenerator
		{
			private List<double> values;
			private int index;

			public InnerGenerator(List<double> values)
			{
				this.values = values;

				index = 0;
			}

			public double NextGaussian()
			{
				return values[index++];
			}
		}
	}
}
