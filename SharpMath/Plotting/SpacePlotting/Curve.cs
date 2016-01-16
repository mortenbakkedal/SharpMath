// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;

namespace SharpMath.Plotting.SpacePlotting
{
	[Serializable]
	public abstract class Curve : ICurve
	{
		public abstract ISpacePoint Value(double t);

		public static Curve Create(Func<double, ISpacePoint> f)
		{
			return new InnerCurve(f);
		}

		[Serializable]
		private class InnerCurve : Curve
		{
			private Func<double, ISpacePoint> f;

			public InnerCurve(Func<double, ISpacePoint> f)
			{
				this.f = f;
			}

			public override ISpacePoint Value(double t)
			{
				return f(t);
			}
		}
	}
}
