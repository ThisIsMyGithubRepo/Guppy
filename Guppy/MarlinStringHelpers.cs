using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace Guppy
{
	public static class MarlinStringHelpers
	{
		public static string StripReponseTextNotNeededForCommand(string s)
		{
			return string.Empty;
		}

		// Cleaner methods return a tuple where the boolean indicates if the line should be kept.
		// Accepts multi-line strings.
		// If Item1 = True, then keep the line. If item1 = false, then drop the line.
		// If not kept, then the calling method should drop the string as unnecessary.
		public static Tuple<bool, string> CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands(string s)
		{
			List<string> commandList = MakeCommandListFromString(s);
			List<string> cleanList = new List<string>();
			List<string> subStringsToRemove = new List<string>() { "echo:", "ok" };
			List<string> linesToRemove = new List<string>() { "Unified Bed Leveling System.*" };


 			Tuple<bool, string> result;

			foreach (string str in commandList)
			{
				result = StripAndRemoveGeneric(subStringsToRemove, linesToRemove, str);
				if(result.Item1)
				{
					cleanList.Add(result.Item2);
				}
			}

			if(cleanList.Count>0)
			{
				string cleanString = MakeStringFromCommandList(cleanList);
				return new Tuple<bool, string>(true, cleanString);
			}
			else
			{
				return new Tuple<bool, string>(false, string.Empty);
			}
		}

		public static Tuple<bool, string> CleanMarlinG29T1UBLMeshResponse(string s)
		{
			return StripAndRemoveGeneric(new List<string>(),
				new List<string>() { "Bed Topography Report for CSV:" },
				s);
		}

		public static Tuple<bool, string> StripAndRemoveGeneric(List<string> subStringsToRemove, List<string> linesToRemove, string s)
		{
			string ns = s.Trim();

			
			if (linesToRemove.Count(z => Regex.IsMatch(ns, z)) > 0)
			{
				return new Tuple<bool, string>(false, string.Empty);
			}
			else
			{
				foreach (string ss in subStringsToRemove)
				{
					ns = ns.Replace(ss, string.Empty, StringComparison.CurrentCultureIgnoreCase);
				}

				ns = ns.Trim();

				return new Tuple<bool, string>(true, ns);
			}
		}

		public static string MakeStringFromCommandList(List<string> commandList)
		{
			return string.Join("\r\n", commandList);
		}

		public static List<string> MakeCommandListFromString(string s)
		{
			return new List<string>(s.Split("\r\n"));
		}

		public static List<string> MakeSanatizedCommandListFromString(string s)
		{
			return SanitizeCommandList(MakeCommandListFromString(s));
		}

		public static List<string> SanitizeCommandList(List<string> commands)
		{
			List<string> StringsToDrop = new List<string>() { string.Empty }; // Expecting more strings to be added in the future.
			string working;

			List<string> clean = new List<string>();
			foreach (string s in commands)
			{
				working = CleanCommandString(s);
				if (!StringsToDrop.Contains(working))
				{
					clean.Add(s);
				}
			}

			return clean;
		}

		public static string CleanCommandString(string s)
		{
			return s.Trim('\n', '\r').Trim();
		}
	}
}
