using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	public class pr_M503orM501_Config: IOutputItem
	{
		// Static helpers associated with this processed response
		#region Static Helper Methods

		public static List<IOutputItem> BuildProcessedResponseM503(List<String> commandList)
		{
			return new List<IOutputItem> { new pr_M503orM501_Config(MarlinOutputItemFactory.GetId(), "M503 or M501 Config - Drag and Drop to Macro Button To Capture", commandList) };
		}

		//public static List<IOutputItem> BuildProcessedResponseM503orM501(string s, List<String> commandList)
		//{
		//	return new List<IOutputItem> { new pr_M503orM501_Config(MarlinOutputItemFactory.GetId(), s, commandList) };
		//}
		#endregion

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
