using System;
using System.Collections.Generic;
using System.Text;

namespace FileUtils
{
	public class FieldConversionException : Exception
	{
		public FieldConversionException(string message)
			: base(message)
		{ }
	}
}
