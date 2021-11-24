using System;
using System.Collections.Generic;
using System.Text;

namespace FileUtils
{
	internal static class ConverterExtensions
	{
		public static T ConvertTo<T>(this object value)
		{

			return (T)ConvertTo(value, typeof(T));
		}

		public static object ConvertTo(this object value, Type type)
		{
			if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
			{
				if (value == null)
				{
					return null;
				}

				type = Nullable.GetUnderlyingType(type);
			}

			return Convert.ChangeType(value, type);
		}
	}
}
