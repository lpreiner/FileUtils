using System;
using System.Globalization;
using System.Reflection;

namespace FileUtils.FieldConverters
{
	public class FieldDateConverterAttribute : FieldConverterAttribute
	{
		readonly string _format;

		readonly bool _defaultSet;
		readonly object _default;
		public FieldDateConverterAttribute(string format)
			=> (_format) = (format);

		public FieldDateConverterAttribute(string format, object defaultOnFailure)
			=> (_format, _defaultSet, _default) = (format, true, defaultOnFailure);

		public override object ReadField(string rawValue, MemberInfo member)
		{
			if (DateTime.TryParseExact(rawValue, _format, null, DateTimeStyles.None, out var result))
				return result;

			if (_defaultSet)
				return _default;

			throw new FieldConversionException($"Failed to convert '{rawValue}' to DateTime for '{member.Name}' and no default value was provided.");
		}

		public override string WriteField(object obj, MemberInfo member)
		{
			return ((DateTime)obj).ToString(_format);
		}
	}
}

