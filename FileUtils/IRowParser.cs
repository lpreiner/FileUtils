using System;
using System.Collections.Generic;
using System.Text;

namespace FileUtils.RowParsers
{
	public interface IRowParser
	{
		T Parse<T>(string rowData);
	}
}
