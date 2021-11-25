using FileUtils.FieldConverters;
using System;
using Xunit;

namespace FileUtils.Tests
{
	public class BoolConverterTests
	{
		[Fact]
		public void BoolConverterTrueReadWrite()
		{
			var value = true;
			var converter = new FieldBooleanConverterAttribute(true.ToString());

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(read, value);
		}

		[Fact]
		public void BoolConverterFalseReadWrite()
		{
			var value = false;
			var converter = new FieldBooleanConverterAttribute(true.ToString(), false.ToString());

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(read, value);
		}

		[Theory]
		[InlineData(true, "YES" )]
		[InlineData(true, "T" )]
		public void BoolConverterCustomTrueReadWrite(bool value, string trueValue)
		{
			var converter = new FieldBooleanConverterAttribute(trueValue);

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(trueValue, write);
			Assert.Equal(value, read);
		}

		[Theory]
		[InlineData(false, "F")]
		[InlineData(false, "NO")]
		public void BoolConverterCustomFalseReadTest(bool value, string falseValue)
        {
			var converter = new FieldBooleanConverterAttribute(new[] { "T" }, new[] { falseValue });

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(falseValue, write);
			Assert.Equal(value, read);
        }

		[Fact]
		public void BoolConverterDefaultTest()
        {
			var converter = new FieldBooleanConverterAttribute("test");

			var read = converter.ReadField("NO", null);

			Assert.Equal(false, read);
        }
	}
}
