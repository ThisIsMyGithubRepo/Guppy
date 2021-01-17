//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Diagnostics;
//using Guppy.OutputItems;

//namespace Guppy.ResponseProcessing
//{
//	class G29TResponse : IResponseProcessor
//	{
//		public enum TriggerTestResult
//		{
//			NotTriggered,
//			TriggerAndInclude,
//			TriggerAndDrop
//		}

//		private List<string> _StartAndIncludeStrings = new List<string>() {};
//		private List<string> _StartAndDropStrings = new List<string>() { "Bed Topography Report:" };
//		private List<string> _EndAndDropStrings = new List<string>() { "ok" };
//		private List<string> _AbortStrings = new List<string>();

//		private bool _isProcessing = false;
//		private int _startId;
//		private int _endId;

//		private List<string> _FullResponse = new List<string>();

//		public string Name { get; private set; } = "M503 or M501";

//		public IOutputItem ProcessAndReturnOutput(IOutputItem outputItem)
//		{
//			if (!_isProcessing)
//			{
//				// Only look for Start Triggers since we're not already processing
//				TriggerTestResult result = IsStartTrigger(outputItem);

//				if (result == TriggerTestResult.TriggerAndInclude) { AddLineToFullResponse(outputItem.Value); }

//				if (result != TriggerTestResult.NotTriggered)
//				{
//					_startId = outputItem.Id;
//					_FullResponse.Clear();
//					_isProcessing = true;
//					return null;
//				}



//				// In all cases, we do not have a decoration return
//				return null;
//			}
//			else // _isProcessing == true
//			{
//				if (IsAbortTrigger(outputItem))
//				{
//					// Not the response we were looking for. Cancel out.
//					_startId = -1;
//					_isProcessing = false;
//					return null;
//				}

//				TriggerTestResult result = IsEndTrigger(outputItem);

//				if (result == TriggerTestResult.TriggerAndInclude)
//				{
//					_endId = outputItem.Id;
//					_isProcessing = false;
//					return MarlinOutputItemFactory.BuildProcessedResponseG29T(_FullResponse);
//				}
//				else if (result == TriggerTestResult.TriggerAndDrop)
//				{
//					// _endId = We leave this value as is. It'll have been set with the previous processed line.
//					_isProcessing = false;
//					return MarlinOutputItemFactory.BuildProcessedResponseG29T(_FullResponse);
//				}
//				else // (result == TriggerTestResult.NotTriggered)
//				{
//					_endId = outputItem.Id; //Keep running count
//					AddLineToFullResponse(outputItem.Value);
//					return null;
//				}
//			}
//		}

//		public TriggerTestResult IsStartTrigger(IOutputItem outputItem)
//		{
//			// We start on either the M503 or M501 command
//			if (outputItem is oi_PrinterCommand && _StartAndIncludeStrings.Contains(outputItem.Value))
//			{
//				Debug.WriteLine($"[{Name}] start trigger found on \"{outputItem.Value}\"");
//				return TriggerTestResult.TriggerAndInclude;
//			}
//			else if (outputItem is oi_PrinterCommand && _StartAndDropStrings.Contains(outputItem.Value))
//			{
//				Debug.WriteLine($"[{Name}] start trigger found on \"{outputItem.Value}\"");
//				return TriggerTestResult.TriggerAndDrop;
//			}
//			else
//			{
//				return TriggerTestResult.NotTriggered;
//			}
//		}

//		public TriggerTestResult IsEndTrigger(IOutputItem outputItem)
//		{
//			// We end on a Marlin Response that matches one of our end triggers.
//			if (outputItem is oi_MarlinResponse && _EndAndDropStrings.Contains(outputItem.Value))
//			{
//				Debug.WriteLine($"[{Name}] end trigger found on \"{outputItem.Value}\"");
//				return TriggerTestResult.TriggerAndDrop;
//			}
//			else
//			{
//				return TriggerTestResult.NotTriggered;
//			}
//		}

//		public bool IsAbortTrigger(IOutputItem outputItem)
//		{
//			// If we come across a new command, then abort. We've clearly not processed an expected result.
//			if (outputItem is oi_PrinterCommand) { return true; }
//			if (_AbortStrings.Contains(outputItem.Value)) { return true; }

//			return false;
//		}

//		private void AddLineToFullResponse(string s)
//		{
//			_FullResponse.Add(MarlinStringHelpers.StripReponseTextNotNeededForCommand(s));
//		}
//	}
//}
