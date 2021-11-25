using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileUtils.FieldConverters
{
	public class FieldBooleanConverterAttribute : FieldConverterAttribute
	{
		readonly IEnumerable<IConvertible> _trueValues;
		readonly IEnumerable<IConvertible> _falseValues;

		readonly bool _defaultSet;
		readonly object _default;

		public StringComparison StringComparison { get; set; } = StringComparison.Ordinal;

		private FieldBooleanConverterAttribute() { }

		public FieldBooleanConverterAttribute(string trueValue, string falseValue)
			: this(new[] { trueValue }, new[] { falseValue }, null) => (_defaultSet) = (false);

		public FieldBooleanConverterAttribute(string trueValue, string falseValue, object defaultOnFailure)
			: this(new[] { trueValue }, new[] { falseValue }, defaultOnFailure) { }

		public FieldBooleanConverterAttribute(params string[] trueValues)
			: this() => (_trueValues, _defaultSet, _default) = (trueValues, true, false);

		public FieldBooleanConverterAttribute(IEnumerable<string> trueValues, IEnumerable<string> falseValues)
			: this() => (_trueValues, _falseValues, _defaultSet) = (trueValues, falseValues, false);

		public FieldBooleanConverterAttribute(IEnumerable<string> trueValues, IEnumerable<string> falseValues, object defaultOnFailure)
			: this(trueValues, falseValues) => (_defaultSet, _default) = (true, defaultOnFailure);

		public override object ReadField(string rawValue, MemberInfo member)
		{
			if (HasMatch(_trueValues, rawValue))
				return true;

			if (HasMatch(_falseValues, rawValue))
				return false;

			if (_defaultSet)
				return _default;

			throw new FieldConversionException($"Failed to convert '{rawValue}' to Boolean for '{member.Name}' and no default value was provided.");
		}

		public override string WriteField(object obj, MemberInfo member)
		{
			if ((bool)obj && _trueValues.Any())
			{
				return _trueValues.First()?.ToString();
			}
			else if (_falseValues.Any())
			{
				return _falseValues.First()?.ToString();
			}

			throw new FieldConversionException($"Failed to convert '{obj}' from Boolean for '{member.Name}'.  No value available for '{obj}'");
		}

		bool HasMatch(IEnumerable<IConvertible> values, string rawValue)
		{
			return values?.Any(v => string.Equals(v?.ToString(), rawValue, StringComparison)) ?? false;
		}
	}
}
