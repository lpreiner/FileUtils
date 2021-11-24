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
			var converter = new FieldBoolConverterAttribute(new[] { true.ToString() }, new[] { false.ToString() });

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(read, value);
		}

		[Fact]
		public void BoolConverterFalseReadWrite()
		{
			var value = false;
			var converter = new FieldBoolConverterAttribute(new[] { true.ToString() }, new[] { false.ToString() });

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(read, value);
		}

		[Theory]
		[InlineData(true, new[] { "YES" }, new[] { "NO" })]
		[InlineData(false, new[] { "YES" }, new[] { "NO" })]
		public void BoolConverterCustomTrueReadWrite(bool value, string[] trueValues, string[] falseValues)
		{
			var converter = new FieldBoolConverterAttribute(trueValues, falseValues);

			var write = converter.WriteField(value, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(read, value);
		}
	}
}
