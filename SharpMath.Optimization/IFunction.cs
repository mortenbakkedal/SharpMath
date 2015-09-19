using System;

namespace SharpMath.Optimization
{
	public interface IFunction
	{
		IFunction Derivative(IVariable variable);

		double this[params VariableAssignment[] assignments]
		{
			get;
		}
	}
}
