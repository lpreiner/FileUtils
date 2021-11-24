using FileUtils.FieldConverters;
using System;
using Xunit;

namespace FileUtils.Tests
{
	public class DateConverterTests
	{
		[Fact]
		public void DateConverterFormatReadWrite()
		{
			var format = "yyyyMMdd_HHmmss.fffffff";
			var date = DateTime.UtcNow;

			var converter = new FieldDateConverterAttribute(format);

			var write = converter.WriteField(date, null);
			var read = converter.ReadField(write, null);

			Assert.Equal(date, read);
		}
	}
}
