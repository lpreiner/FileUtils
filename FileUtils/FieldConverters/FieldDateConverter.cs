using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FileUtils.FieldConverters
{
	public class FieldDateConverterAttribute : FieldConverterAttribute
	{
		readonly string _format;
		public FieldDateConverterAttribute(string format)
			=> (_format) = (format);

		public override object ReadField(string rawValue, MemberInfo member)
		{
			return DateTime.ParseExact(rawValue, _format, null);
		}

		public override string WriteField(object obj, MemberInfo member)
		{
			return ((DateTime)obj).ToString(_format);
		}
	}
}

