// Copyright (c) 2014 Morten Bakkedal
// This code is published under the MIT License.

using System;
using System.Collections.Generic;

namespace SharpMath
{
	public static class EqualityHelper
	{
		public static bool Equals(object object1, object object2, object field1, object field2)
		{
			return Equals(object1, object2, field1, field2, 0, 0);
		}

		public static bool Equals(object object1, object object2, object field1, object field2, int hashCode1, int hashCode2)
		{
			return Equals(object1, object2, new object[] { field1 }, new object[] { field2 }, hashCode1, hashCode2);
		}

		public static bool Equals(object object1, object object2, object[] fields1, object[] fields2)
		{
			return Equals(object1, object2, fields1, fields2, 0, 0);
		}

		public static bool Equals(object object1, object object2, object[] fields1, object[] fields2, int hashCode1, int hashCode2)
		{
			if (object.ReferenceEquals(object1, object2))
			{
				// Important special case where the references are identical.
				return true;
			}

			if (object.ReferenceEquals(object1, null) || object.ReferenceEquals(object2, null))
			{
				// Case where both are null is already handled above.
				return false;
			}

			if (object1.GetType() != object2.GetType())
			{
				return false;
			}

			if (hashCode1 != 0 && hashCode2 != 0 && hashCode1 != hashCode2)
			{
				// Can't be equal if hash codes are computed and are different.
				return false;
			}

			int n = fields1.Length;

			if (fields2.Length != n)
			{
				throw new ArgumentException();
			}

			// At this point we have to compare all relevant fields.
			return FieldEquals(fields1, fields2);
		}

		public static int HashCode(object[] fields)
		{
			return FieldHashCode(fields);
		}

		private static int FieldHashCode(object field)
		{
			if (field == null)
			{
				// Don't fail on null fields; but may produce a poor inefficient hash code.
				return 0;
			}

			if (field is Array)
			{
				Array a = (Array)field;
				int n = a.Length;

				int hashCode = 23;
				unchecked
				{
					for (int i = 0; i < n; i++)
					{
						hashCode = hashCode * 31 + FieldHashCode(a.GetValue(i));
					}
				}

				return hashCode;
			}

			return field.GetHashCode();
		}

		private static bool FieldEquals(object field1, object field2)
		{
			if (object.ReferenceEquals(field1, null))
			{
				if (object.ReferenceEquals(field2, null))
				{
					return true;
				}

				return false;
			}

			if (field1 is Array && field2 is Array)
			{
				// Special handling of arrays (not IEnumerable in general where a custom defined equality
				// operator may be defined; don't want to mess with that).
				Array a1 = (Array)field1;
				Array a2 = (Array)field2;
				int n = a1.Length;

				if (a2.Length != n)
				{
					return false;
				}

				for (int i = 0; i < n; i++)
				{
					if (!FieldEquals(a1.GetValue(i), a2.GetValue(i)))
					{
						return false;
					}
				}

				return true;
			}

			return field1.Equals(field2);
		}
	}
}
