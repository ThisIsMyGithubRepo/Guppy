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
				startAndIncludeStrings: new List<string>(),
				startAndExcludeStrings: new List<string>() { "echo:  G21    ; Units in mm (mm)" },
				endAndIncludeStrings: new List<string>(),
				endAndExcludeStrings: new List<string>() { "ok" },
				abortStrings: new List<string>(),
				cleaningFunction: MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands,
				builderFunction: pr_M503orM501_Config.BuildProcessedResponseM503);

			p.Name = "M503 or M501 Processor";
			col.Add(p);

			//G29T Processor

			p = new SimpleStartEndAbortMatcher(
				startAndIncludeStrings: new List<string>(),
				startAndExcludeStrings: new List<string>() { "Bed Topography Report for CSV:" },
				endAndIncludeStrings: new List<string>(),
				endAndExcludeStrings: new List<string>() { "ok" },
				abortStrings: new List<string>(),
				cleaningFunction: MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands,
				builderFunction: pr_G29T_MeshMap.BuildProcessedResponseG29T);

			//p.Name = "G29 T1 Processor";
			col.Add(p);

			//M20 Processor - SD Card File List

			p = new SimpleStartEndAbortMatcher(
				startAndIncludeStrings: new List<string>(),
				startAndExcludeStrings: new List<string>() { "Begin file list" },
				endAndIncludeStrings: new List<string>(),
				endAndExcludeStrings: new List<string>() { "ok" },
				abortStrings: new List<string>(),
				cleaningFunction: pr_M20_PrintableFile.M20_FileListCleanerFunction,
				builderFunction: pr_M20_PrintableFile.BuildProcessedResponse);

			//p.Name = "G20 SD Card File List Processor";
			col.Add(p);
			return col;

		}

		public static int GetId()
		{
			return Interlocked.Increment(ref _id);
		}

		public static IOutputItem BuildApplicationMessageOutputItem(string s)
		{
			return new oi_ApplicationMessage(GetId(), s);
		}

		public static IOutputItem BuildMarlinResponseOutputItem(string s)
		{
			return new oi_MarlinResponse(GetId(), s);
		}

		public static IOutputItem BuildPrinterCommandOutputItem(string s)
		{
			return new oi_PrinterCommand(GetId(), s);
		}
	}
}
