using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileUtils.FieldConverters
{
	public class FieldBoolConverterAttribute : FieldConverterAttribute
	{
		readonly IEnumerable<string> _trueValues;
		readonly IEnumerable<string> _falseValues;
		public StringComparison ComparisonType { get; } = StringComparison.Ordinal;
		public FieldBoolConverterAttribute(IEnumerable<string> trueValues, IEnumerable<string> falseValues)
			=> (_trueValues, _falseValues) = (trueValues, falseValues);
		
		public override object ReadField(string rawValue, MemberInfo member)
		{
			return IsTrue(rawValue);
		}

		public override string WriteField(object obj, MemberInfo member)
		{
			var isTrue = obj is null ? false :  obj.ConvertTo<bool>();
			return (isTrue ? _trueValues : _falseValues).FirstOrDefault();
		}

		bool IsTrue(string value)
		{
			return _trueValues.Any(tv => tv.Equals(value, ComparisonType));
		}
	}
}
