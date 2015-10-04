// Copyright (c) 2015 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.Optimization;
using SharpMath.Optimization.Ipopt;

namespace SharpMath.Samples.Optimization
{
	public class OptimizerSamples
	{
		public static void Sample1()
		{
			// Variables. The string names are added for convenience when printing out the optimal point with Point.ToString().
			Variable x = new Variable("x");
			Variable y = new Variable("y");

			// Rosenbrock function http://en.wikipedia.org/wiki/Rosenbrock_function.
			Function f = Function.Sqr(1.0 - x) + 100.0 * Function.Sqr(y - Function.Sqr(x));

			// Use the BFGS optimizer.
			BfgsOptimizer o = new BfgsOptimizer();

			// Specify variables and objective functions and add constraints. Derivatives are computed automatically.
			o.Variables.Add(x, y);
			o.VariableConstraints.Add(x >= 0.0, y >= 0.0);
			o.ObjectiveFunction = f;

			// Start optimizer from a random point.
			Random r = new Random(1);
			IOptimizerResult or = o.Run(x | r.NextDouble(), y | r.NextDouble());

			// Show convergence status and optimal point and value.
			Console.WriteLine(or.Status);
			Console.WriteLine(or.OptimalPoint);
			Console.WriteLine(or.OptimalValue);

			// The result object also contains BFGS convergence status.
			Console.WriteLine(((BfgsOptimizerResult)or).ConvergenceStatus);

			// We may use Prepare if we need to run the optimizer from multiple starting points.
			PreparedOptimizer o2 = o.Prepare();
			for (int i = 0; i < 10; i++)
			{
				IOptimizerResult or2 = o2.Run(x | r.NextDouble(), y | r.NextDouble());
				Console.WriteLine(or2.OptimalPoint);
			}
		}

		public static void Sample2()
		{
			// See IpoptHowTo.txt how to get Ipopt to work. It's quite easy and Ipopt is very powerful.

			// Variables.
			Variable x1 = new Variable();
			Variable x2 = new Variable();
			Variable x3 = new Variable();
			Variable x4 = new Variable();

			// Objective function and non-linear constraints.
			Function f = x1 * x4 * (x1 + x2 + x3) + x3;
			Function g1 = x1 * x2 * x3 * x4;
			Function g2 = Function.Sqr(x1) + Function.Sqr(x2) + Function.Sqr(x3) + Function.Sqr(x4);

			// Objective function and constrains may be compiled including first and second derivatives.
			//CompiledFunction[] c = Compiler.Compile(new Function[] { f, g1, g2 }, new Variable[] { x1, x2, x3, x4 }, 2);
			//f = c[0];
			//g1 = c[1];
			//g2 = c[2];

			// Prepare the optimizer.
			IpoptOptimizer o = new IpoptOptimizer();
			o.Variables.Add(x1, x2, x3, x4);
			o.ObjectiveFunction = f;
			o.Constraints.Add(g1 >= 25.0);
			o.Constraints.Add(g2 == 40.0);
			o.Constraints.Add(1.0 <= x1, x1 <= 5.0);
			o.Constraints.Add(1.0 <= x2, x2 <= 5.0);
			o.Constraints.Add(1.0 <= x3, x3 <= 5.0);
			o.Constraints.Add(1.0 <= x4, x4 <= 5.0);

			// Verbose mode. Show Ipopt convergence.
			o.PrintLevel = 5;

			// Run optimization. Initial point doesn't have to satisfy the constraints.
			IpoptOptimizerResult or = o.RunIpopt(x1 | 1.0, x2 | 5.0, x3 | 5.0, x4 | 1.0);

			Console.WriteLine(or.ReturnCode);
			Console.WriteLine("x1 = " + or.OptimalPoint[x1]);
			Console.WriteLine("x2 = " + or.OptimalPoint[x2]);
			Console.WriteLine("x3 = " + or.OptimalPoint[x3]);
			Console.WriteLine("x4 = " + or.OptimalPoint[x4]);
			Console.WriteLine("f = " + or.OptimalValue);
			Console.WriteLine("g1 = " + g1.Value(or.OptimalPoint));
			Console.WriteLine("g2 = " + g2.Value(or.OptimalPoint));
		}
	}
}
