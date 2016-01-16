// SharpMath - C# Mathematical Library
// Copyright (c) 2016 Morten Bakkedal
// This code is published under the MIT License.

using System;

using SharpMath.LinearAlgebra;

namespace SharpMath
{
	public class DifferentialEquationSolver
	{
		/// <summary>
		/// Represents a differential equation $y'=f(t,y)$.
		/// </summary>
		public delegate Vector DifferentialEquation(double t, Vector y);

		/// <summary>
		/// Represents a complex differential equation $y'=f(t,y)$.
		/// </summary>
		//public delegate ComplexVector ComplexDifferentialEquation(double t, ComplexVector y);

		/// <summary>
		/// Solve the differential equation $y'(t)=f(t,y(t))$ with the boundary condition $y(0)=y_0$. Returns $y(s)$.
		/// </summary>
		public static Vector FirstOrderSolve(DifferentialEquation f, Vector y0, double s, int n)
		{
			double dt = s / n;

			Vector y = y0;
			for (int i = 0; i < n; i++)
			{
				double t = i * dt;
				Vector dy = f(t, y);
				y += dy * dt;
			}

			return y;
		}

		/// <summary>
		/// Solve the complex differential equation $y'(t)=f(t,y(t))$ with the boundary condition $y(0)=y_0$. Returns $y(s)$.
		/// </summary>
		/*public static ComplexVector FirstOrderSolve(ComplexDifferentialEquation f, ComplexVector y0, double s, int n)
		{
			double dt = s / n;

			ComplexVector y = y0;
			for (int i = 0; i < n; i++)
			{
				double t = i * dt;
				ComplexVector dy = f(t, y);
				y += dy * dt;
			}

			return y;
		}*/

		/// <summary>
		/// Solve the complex differential equation $y'(t)=f(t,y(t))$ with the boundary condition $y(s)=y_s$. Returns $y(0)$.
		/// </summary>
		/*public static ComplexVector FirstOrderSolveReverse(ComplexDifferentialEquation f, double s, ComplexVector ys, int n)
		{
			ComplexDifferentialEquation g = delegate(double t, ComplexVector y) { return -f(s - t, y); };
			return FirstOrderSolve(g, ys, s, n);
		}*/

		/// <summary>
		/// Solve the differential equation $y'(t)=f(t,y(t))$ with the boundary condition $y(0)=y_0$. Returns $y(s)$.
		/// </summary>
		public static Vector RungeKuttaSolve(DifferentialEquation f, Vector y0, double s, int n)
		{
			double dt = s / n;

			Vector y = y0;
			for (int i = 0; i < n; i++)
			{
				double t = i * dt;

				Vector k1 = f(t, y);
				Vector k2 = f(t + 0.5 * dt, y + 0.5 * dt * k1);
				Vector k3 = f(t + 0.5 * dt, y + 0.5 * dt * k2);
				Vector k4 = f(t + dt, y + dt * k3);

				y += dt * (k1 + 2.0 * k2 + 2.0 * k3 + k4) / 6.0;
			}

			return y;
		}

		/// <summary>
		/// Solve the complex differential equation $y'(t)=f(t,y(t))$ with the boundary condition $y(0)=y_0$. Returns $y(s)$.
		/// </summary>
		/*public static ComplexVector RungeKuttaSolve(ComplexDifferentialEquation f, ComplexVector y0, double s, int n)
		{
			double dt = s / n;

			ComplexVector y = y0;
			for (int i = 0; i < n; i++)
			{
				double t = i * dt;

				ComplexVector k1 = f(t, y);
				ComplexVector k2 = f(t + 0.5 * dt, y + 0.5 * dt * k1);
				ComplexVector k3 = f(t + 0.5 * dt, y + 0.5 * dt * k2);
				ComplexVector k4 = f(t + dt, y + dt * k3);

				y += dt * (k1 + 2.0 * k2 + 2.0 * k3 + k4) / 6.0;
			}

			return y;
		}*/

		/// <summary>
		/// Solve the complex differential equation $y'(t)=f(t,y(t))$ with the boundary condition $y(s)=y_s$. Returns $y(0)$.
		/// </summary>
		/*public static ComplexVector RungeKuttaSolveReverse(ComplexDifferentialEquation f, double s, ComplexVector ys, int n)
		{
			ComplexDifferentialEquation g = delegate(double t, ComplexVector y) { return -f(s - t, y); };
			return RungeKuttaSolve(g, ys, s, n);
		}*/
	}
}
