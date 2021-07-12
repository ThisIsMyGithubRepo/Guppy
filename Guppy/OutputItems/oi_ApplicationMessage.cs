using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	class oi_ApplicationMessage: IOutputItem
	{
		public string Value { get; set; }
		public int Id { get; private set; }

		public oi_ApplicationMessage(int id, string value)
		{
			Id = id;
			Value = value;
		}

	}
}
