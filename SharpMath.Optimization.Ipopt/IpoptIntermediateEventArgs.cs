// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.ComponentModel;

namespace SharpMath.Optimization.Ipopt
{
	public class IpoptIntermediateEventArgs : CancelEventArgs
	{
		public IpoptIntermediateEventArgs(IpoptAlgorithmMode algorithmMode, int currentIteration, double objectiveValue, double primalInfeasibility, double dualInfeasibility, double barrierParameter, double gradientNorm, double regularizationSize, double dualAlpha, double primalAlpha, int linesearchTrials)
		{
			AlgorithmMode = algorithmMode;
			CurrentIteration = currentIteration;
			ObjectiveValue = objectiveValue;
			PrimalInfeasibility = primalInfeasibility;
			DualInfeasibility = dualInfeasibility;
			BarrierParameter = barrierParameter;
			GradientNorm = gradientNorm;
			RegularizationSize = regularizationSize;
			DualAlpha = dualAlpha;
			PrimalAlpha = primalAlpha;
			LinesearchTrials = linesearchTrials;
		}

		/// <summary>
		/// Current mode of algorithm (alg_mod).
		/// </summary>
		public IpoptAlgorithmMode AlgorithmMode
		{
			get;
			private set;
		}

		/// <summary>
		/// Current iteration number (iter_count).
		/// </summary>
		public int CurrentIteration
		{
			get;
			private set;
		}

		/// <summary>
		/// Current unscaled objective value (obj_value).
		/// </summary>
		public double ObjectiveValue
		{
			get;
			private set;
		}

		/// <summary>
		/// Current primal infeasibility (inf_pr).
		/// </summary>
		public double PrimalInfeasibility
		{
			get;
			private set;
		}

		/// <summary>
		/// Current dual infeasibility (inf_du).
		/// </summary>
		public double DualInfeasibility
		{
			get;
			private set;
		}

		/// <summary>
		/// Current barrier parameter (mu).
		/// </summary>
		public double BarrierParameter
		{
			get;
			private set;
		}

		/// <summary>
		/// Current gradient norm (d_norm).
		/// </summary>
		public double GradientNorm
		{
			get;
			private set;
		}

		/// <summary>
		/// Current size of regularization (regularization_size).
		/// </summary>
		public double RegularizationSize
		{
			get;
			private set;
		}

		/// <summary>
		/// Current dual alpha (alpha_du).
		/// </summary>
		public double DualAlpha
		{
			get;
			private set;
		}

		/// <summary>
		/// Current primal alpha (alpha_pr).
		/// </summary>
		public double PrimalAlpha
		{
			get;
			private set;
		}

		/// <summary>
		/// Current number of linesearch trials (ls_trials).
		/// </summary>
		public int LinesearchTrials
		{
			get;
			private set;
		}
	}
}
