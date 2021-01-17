using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Guppy
{
	public class MacroItem
	{
		public const char ListSeparator = ',';
		public MacroItem(string settingString)
		{
			Label = "";
			string[] s = settingString.Split(ListSeparator);

			List<string> _list = new List<string>(s);

			if (_list.Count == 0) { return; } // Empty list, nothing to do - leave defaults from above.

			// If we get here, we have at least 1 element. That's our label.
			Label = _list[0];
			_list.RemoveAt(0);

			CommandList = MarlinStringHelpers.SanitizeCommandList(_list);
		}

		public MacroItem(string label, string commandString)
		{
			Label = label;
			CommandString = commandString;
		}

		public MacroItem(string label, List<String> commandList)
		{
			Label = label;
			CommandList = MarlinStringHelpers.SanitizeCommandList(commandList);
		}

		public string ControlName { get; set; }
		public string Label { get; set; }
		public List<String> CommandList { get; private set; }

		public string CommandString
		{
			get
			{
				return MarlinStringHelpers.MakeStringFromCommandList(CommandList);
			}

			set
			{
				CommandList = MarlinStringHelpers.MakeSanatizedCommandListFromString(value);
			}
		}



		public override string ToString()
		{
			return $"{Label}{ListSeparator}{String.Join(ListSeparator, CommandList)}";
		}

		public MacroItem ShallowClone()
		{
			return new MacroItem(this.ToString());
		}
	}
}
