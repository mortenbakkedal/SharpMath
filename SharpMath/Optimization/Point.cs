// SharpMath - C# Mathematical Library
// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace SharpMath.Optimization
{
	[Serializable]
	[DebuggerDisplay("{ToString(),nq}")]
	public class Point : IPoint, IEquatable<Point>
	{
		private Dictionary<Variable, double> assignments;
		private int hashCode;

		static Point()
		{
			Empty = new Point();
		}

		private Point()
		{
			assignments = new Dictionary<Variable, double>();
			hashCode = 0;
		}

		public Point(IEnumerable<VariableAssignment> assignments)
			: this()
		{
			// Copy to keep immutable (and faster using a hash table).
			foreach (VariableAssignment assignment in assignments)
			{
				Add(assignment.Variable, assignment.Value);
			}
		}

		public Point(params VariableAssignment[] assignments)
			: this((IEnumerable<VariableAssignment>)assignments)
		{
		}

		private void Add(Variable variable, double value)
		{
			if (variable == null)
			{
				throw new ArgumentNullException("variable");
			}

			double value2;
			if (assignments.TryGetValue(variable, out value2))
			{
				if (value != value2)
				{
					throw new ArgumentException("The variable is already assigned to a different value.");
				}

				// Already added with the same value.
				return;
			}

			assignments.Add(variable, value);

			// Explicitly unchecked. We need overflowing integer arithmetics.
			unchecked
			{
				// The hash code of the variable-number-pair is composed as described here:
				// http://stackoverflow.com/questions/892618/create-a-hashcode-of-two-numbers
				int hashCodePair = (23 * 31 + variable.GetHashCode()) * 31 + value.GetHashCode();

				// Compute a symmetric hash code of the added variable-number-pairs (i.e. interchanging
				// two pairs doesn't change the hash code). Xor has this property.
				hashCode ^= hashCodePair;
			}
		}

		public bool ContainsVariable(Variable variable)
		{
			return assignments.ContainsKey(variable);
		}

		public bool Equals(Point other)
		{
			// Follows Item 6 in Effective C# in first three tests.

			if (object.ReferenceEquals(other, null))
			{
				return false;
			}

			if (object.ReferenceEquals(this, other))
			{
				return true;
			}

			if (GetType() != other.GetType())
			{
				return false;
			}

			if (hashCode != other.hashCode)
			{
				// Can't be equal if they have different hash codes.
				return false;
			}

			if (assignments.Count != other.assignments.Count)
			{
				// Can't be equal if they contain a different number of variables.
				return false;
			}

			foreach (KeyValuePair<Variable, double> assignment in assignments)
			{
				Variable variable = assignment.Key;
				double value = assignment.Value;

				double otherValue;
				if (!other.assignments.TryGetValue(variable, out otherValue))
				{
					// The other point doesn't contain this variable so they can't be equal.
					return false;
				}

				if (value != otherValue)
				{
					return false;
				}
			}

			// No differences found.
			return true;
		}

		public override bool Equals(object other)
		{
			return Equals(other as Point);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}

		public IEnumerator<VariableAssignment> GetEnumerator()
		{
			foreach (KeyValuePair<Variable, double> assignment in assignments)
			{
				yield return new VariableAssignment(assignment.Key, assignment.Value);
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public override string ToString()
		{
			return ToString("r");
		}

		public string ToString(string format)
		{
			StringBuilder sb = new StringBuilder();

			foreach (KeyValuePair<Variable, double> assignment in assignments)
			{
				Variable variable = assignment.Key;
				double value = assignment.Value;

				if (sb.Length > 0)
				{
					sb.Append(", ");
				}

				if (!string.IsNullOrEmpty(variable.Name))
				{
					sb.Append(variable.Name);
					sb.Append(" = ");
				}

				sb.Append(value.ToString(format, CultureInfo.InvariantCulture));
			}

			return sb.ToString();
		}

		public bool Equals(IPoint other)
		{
			throw new NotImplementedException();
		}

		public double this[Variable variable]
		{
			get
			{
				double value;
				if (!assignments.TryGetValue(variable, out value))
				{
					throw new VariableNotAssignedException(variable, "Value of a non-assigned variable is required.");
				}

				return value;
			}
		}

		public int Count
		{
			get
			{
				return assignments.Count;
			}
		}

		public static bool operator ==(Point point1, Point point2)
		{
			if (object.ReferenceEquals(point1, null))
			{
				return false;
			}

			return point1.Equals(point2);
		}

		public static bool operator !=(Point point1, Point point2)
		{
			return !(point1 == point2);
		}

		/// <summary>
		/// Point with no variables for evaluation of constant functions.
		/// </summary>
		public static Point Empty
		{
			get;
			private set;
		}

		public IEnumerable<IVariableAssignment> Assignments
		{
			get
			{
				throw new NotImplementedException();
			}
		}
	}
}
