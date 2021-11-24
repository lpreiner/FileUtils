using FileUtils.FieldConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace FileUtils.RowParsers
{
	#region Attributes
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class FixedFieldAttribute : Attribute
	{
		public int Offset { get; }
		public int Length { get; set; }

		public FixedFieldAttribute(int offset, int length)
			=> (Offset, Length) = (offset, length);
	}

	public class FixedFieldRangeAttribute : FixedFieldAttribute
	{
		public FixedFieldRangeAttribute(int start, int end)
			: base(start, end - start)
		{ }
	}

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class FieldTrimAttribute : Attribute
	{
		public Trim Trim { get; }

		public FieldTrimAttribute(Trim trim = Trim.Both)
			=> (Trim) = (trim);
	}
	#endregion

	public class FixedFieldRowParserSettings
	{
		public Trim DefaultFieldTrim { get; set; } = Trim.None;
		public int OffsetBase { get; set; } = 0;
	}

	public enum Trim
	{
		None,
		Left,
		Right,
		Both,
	}

	public class FixedFieldRowParser : IRowParser
	{
		readonly TypeInfo _typeInfo;
		public FixedFieldRowParserSettings Settings { get; set; }

		public FixedFieldRowParser()
			: this(new FixedFieldRowParserSettings())
		{ }

		public FixedFieldRowParser(FixedFieldRowParserSettings settings)
		{
			_typeInfo = new TypeInfo();
			Settings = settings;
		}

		public T Parse<T>(string rowData)
		{
			var item = GetInstance<T>();
			var fields = GetFields<T>();

			foreach (var field in fields)
			{
				// TODO: enforce parse modes (i.e. strict, relaxed)

				var offset = field.FixedField.Offset - Settings.OffsetBase;
				var rawValue = rowData.Substring(offset, field.FixedField.Length);
				var trimAttr = field.Member.GetCustomAttribute<FieldTrimAttribute>();

				var trim = trimAttr is null ? Settings.DefaultFieldTrim : trimAttr.Trim;
				rawValue = trim switch
				{
					Trim.Both => rawValue?.Trim(),
					Trim.Left => rawValue?.TrimStart(),
					Trim.Right => rawValue?.TrimEnd(),
					_ => rawValue,
				};

				try
				{
					object value = rawValue;

					var converterAttr = field.Member.GetCustomAttribute<FieldConverterAttribute>();
					if (converterAttr != null)
					{
						value = converterAttr.ReadField(rawValue, field.Member);
					}
					
					Action<object> setFunc = field.Member switch
					{
						PropertyInfo p => (v) => p.SetValue(item, value.ConvertTo(p.PropertyType)),
						FieldInfo f => (v) => f.SetValue(item, value.ConvertTo(f.FieldType)),
						_ => (v) => { return; }
					};
					setFunc(rawValue);
				}
				catch (Exception ex)
				{
					var msg = new StringBuilder($"Parsing failed for member '{field.Member.Name}'");
					msg.Append($" '{field.Member.Name}' ({GetMemberType(field.Member)})");
					msg.Append($" for range {offset} - {offset + field.FixedField.Length}.");
					msg.Append($" Input: '{rawValue}'");

					throw new FixedFieldParserException(msg.ToString() , ex);
				}
			}

			return item;
		}

		Type GetMemberType(MemberInfo memberInfo)
		{
			return memberInfo switch
			{
				PropertyInfo p => p.PropertyType,
				FieldInfo f => f.FieldType,
				_ => null,
			};
		}

		T GetInstance<T>()
		{
			return (T)Activator.CreateInstance(typeof(T), true);
		}

		IEnumerable<FixedFieldInfo> GetFields<T>()
		{
			var type = typeof(T);
			if (!_typeInfo.TryGetValue(type, out var fields))
			{
				var memberTypes = MemberTypes.Property | MemberTypes.Field;
				var flags = BindingFlags.Public | BindingFlags.Instance;
				fields = type.FindMembers(memberTypes, flags, null, null)
					.Where(t => t.IsDefined(typeof(FixedFieldAttribute)) || t.IsDefined(typeof(FixedFieldRangeAttribute)))
					.Select(t => new FixedFieldInfo
					{
						Member = t,
						FixedField = (FixedFieldAttribute)t.GetCustomAttribute(typeof(FixedFieldAttribute))
					});

				_typeInfo.TryAdd(type, fields);
			}

			return fields;
		}

		class TypeInfo : Dictionary<Type, IEnumerable<FixedFieldInfo>>
		{ }

		class FixedFieldParserException : Exception
		{
			public FixedFieldParserException(string message, Exception innerException)
				: base(message, innerException)
			{ }
		}

		class FixedFieldInfo
		{
			public MemberInfo Member { get; set; }
			public FixedFieldAttribute FixedField { get; set; }
		}
	}
}
