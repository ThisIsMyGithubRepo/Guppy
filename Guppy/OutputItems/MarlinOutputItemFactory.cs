using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Guppy.ResponseProcessing;

namespace Guppy.OutputItems
{
	public static class MarlinOutputItemFactory
	{
		static int _id = 0;

		public static List<IResponseProcessor> BootstrapResponseProcessors()
		{
			List<IResponseProcessor> col = new List<IResponseProcessor>();
			IResponseProcessor p;

			//M503 Processor
			p = new SimpleStartEndAbortMatcher(
				new List<string>() { "M503", "M501" },
				new List<string>(),
				new List<string>(),
				new List<string>() { "ok" },
				new List<string>(),
				MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands,
				BuildProcessedResponseM503);

			p.Name = "M503 or M501 Processor";
			col.Add(p);

			//G29T Processor

			p = new SimpleStartEndAbortMatcher(
				new List<string>(),
				new List<string>() { "Bed Topography Report for CSV:" },
				new List<string>(),
				new List<string>() { "ok" },
				new List<string>(),
				MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands,
				BuildProcessedResponseG29T);

			p.Name = "G29 T1 Processor";
			col.Add(p);

			return col;

		}

		static int GetId()
		{
			return Interlocked.Increment(ref _id);
		}

		public static IOutputItem BuildMarlinResponseOutputItem(string s)
		{
			return new oi_MarlinResponse(GetId(), s);
		}

		public static IOutputItem BuildApplicationMessageOutputItem(string s)
		{
			return new oi_ApplicationMessage(GetId(), s);
		}

		public static IOutputItem BuildPrinterCommandOutputItem(string s)
		{
			return new oi_PrinterCommand(GetId(), s);
		}

		public static IOutputItem BuildProcessedResponseM503orM501(string s, List<String> commandList)
		{
			return new pr_M503orM501_Config(GetId(), s, commandList);
		}

		public static IOutputItem BuildProcessedResponseM503(List<String> commandList)
		{
			return new pr_M503orM501_Config(GetId(), "M503 or M501 Config - Drag and Drop to Macro Button To Capture", commandList);
		}

		public static IOutputItem BuildProcessedResponseG29T(List<String> commandList)
		{
			List<string> stringsToRemove = new List<String>() { "Bed Topography Report for CSV:", string.Empty };
			List<string> clean = commandList.FindAll(s => !stringsToRemove.Contains(s.Trim()));

			// List is in reverse order (y=max is the first line) so we want to fix that.
			clean.Reverse();

			int sizeY = clean.Count;
			int sizeX = clean[0].Split("\t").Length;

			double[,] mesh = new double[sizeX, sizeY];

			double[] row;
			for (int y = 0; y < sizeY; y++)
			{
				// Split the string and convert the values in floats.
				row = Array.ConvertAll(clean[y].Split("\t"), s => double.Parse(s));

				for (int x = 0; x < sizeX; x++)
				{
					// clean array 
					mesh[x, y] = row[x];
				}
			}

			return new pr_G29T_MeshMap(GetId(), "G29 T Mesh Map - Double Click to View", mesh);
		}
	}
}
