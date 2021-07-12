using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	public class pr_M20_PrintableFile: IOutputItem
	{

		// Static helpers associated with this processed response
		#region Static Helper Methods
		public static List<IOutputItem> BuildProcessedResponse(List<String> commandList)
		{
			// At this point we have a list of file names in the format "THREAD~1.GCO 3257334"
			// We are going to turn each into an M20_PrintableFile output item.

			List<IOutputItem> files = new List<IOutputItem>();
			commandList.ForEach(s => files.Add(new pr_M20_PrintableFile(MarlinOutputItemFactory.GetId(), s)));

			return files;
		}

		public static Tuple<bool, string> M20_FileListCleanerFunction(string s)
		{
			return MarlinStringHelpers.StripAndRemoveGeneric(new List<string>(), new List<string>() { "End file list" }, s);
		}
		#endregion

		public string Value { get; set; }
		public int Id { get; private set; }

		public pr_M20_PrintableFile(int id, string FileString)
		{
			Id = id;
			Value = FileString.Split(' ')[0];
		}


	}
}
