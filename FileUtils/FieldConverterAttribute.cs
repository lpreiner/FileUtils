using System;
using System.Reflection;

namespace FileUtils.FieldConverters
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public abstract class FieldConverterAttribute : Attribute
	{
		public abstract object ReadField(string rawValue, MemberInfo member);
		public abstract string WriteField(object obj, MemberInfo member);
	}
}
