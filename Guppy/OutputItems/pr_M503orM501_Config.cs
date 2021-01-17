using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	public class pr_M503orM501_Config: IOutputItem
	{
		public string Value { get; set; }
		public int Id { get; private set; }
		public List<String> CommandList { get; private set; }

		public pr_M503orM501_Config(int id, string value, List<String> commandList)
		{
			Id = id;
			Value = value;
			CommandList = commandList;
		}
	}
}
