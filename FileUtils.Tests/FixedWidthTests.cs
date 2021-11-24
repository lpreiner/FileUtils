using FileUtils.RowParsers;
using System;
using Xunit;

namespace FileUtils.Tests
{
	public class FixedWidthTests
	{
		[Fact]
		public void SimpleRowParse()
		{
			string rawRow = "TESTTEST-2147483648-9223372036854775808";
			var parser = new FixedFieldRowParser();
			var result = parser.Parse<TestRow>(rawRow);

			Assert.Equal("TESTTEST", result.StringField);
			Assert.Equal(int.MinValue, result.IntField);
			Assert.Equal(long.MinValue, result.LongField);
		}

		[Theory]
		[InlineData(Trim.None, "  test  ", "  test  ")]
		[InlineData(Trim.Left, "  test  ", "test  ")]
		[InlineData(Trim.Right, "  test  ", "  test")]
		[InlineData(Trim.Both, "  test  ", "test")]
		public void TestDefaultTrimSetting(Trim trimMode, string input, string expected)
		{
			var parser = new FixedFieldRowParser(new FixedFieldRowParserSettings
			{
				DefaultFieldTrim = trimMode
			});

			var result = parser.Parse<StringFieldTestRow>(input);
			Assert.Equal(expected, result.StringField);
		}

		[Fact]
		public void TestOffsetSettingZero()
		{
			var input = "TEST1234";
			var parser = new FixedFieldRowParser(new FixedFieldRowParserSettings
			{
				OffsetBase = 0,
			});

			var result = parser.Parse<StringFieldTestRow>(input);
			Assert.Equal(input, result.StringField);
		}

		[Fact]
		public void TestOffsetSettingNonZero()
		{
			var input = "TEST1234";
			var parser = new FixedFieldRowParser(new FixedFieldRowParserSettings
			{
				OffsetBase = 1,
			});

			var result = parser.Parse<StringFieldTestRowOffset1>(input);
			Assert.Equal(input, result.StringField);
		}


		[Fact]
		public void TestModeSettingRelaxed()
		{

		}

		class StringFieldTestRow
        {
			[FixedField(0, 8)]
			public string StringField { get; set; }
		}

		class StringFieldTestRowOffset1
        {
			[FixedField(1, 8)]
			public string StringField { get; set; }
        }

		class TestRow : StringFieldTestRow
		{		
			
			[FixedField(8, 11)]
			public int IntField { get; set; }

			[FixedField(19, 20)]
			public long LongField { get; set; }
			public decimal DecimalField { get; set; }
			public double DoubleField { get; set; }
			public float FloatField { get; set; }
			public Guid GuidField { get; set; }
		}
	}
}
