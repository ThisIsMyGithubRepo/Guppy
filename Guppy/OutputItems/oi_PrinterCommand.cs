using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	public class oi_PrinterCommand: IOutputItem
	{
		public Type OutputType { get; set; }
		public string Value { get; set; }
		public int Id { get; private set; }

		public oi_PrinterCommand(int id, string value)
		{
			Id = id;
			Value = value;
		}

	}
}
