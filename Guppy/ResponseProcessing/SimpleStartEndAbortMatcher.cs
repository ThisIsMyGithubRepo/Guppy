using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Guppy.OutputItems;

namespace Guppy.ResponseProcessing
{
	class SimpleStartEndAbortMatcher : IResponseProcessor
	{
		public enum TriggerTestResult
		{
			NotTriggered,
			TriggerAndInclude,
			TriggerAndExclude
		}

		private Func<string, Tuple<bool, string>> _FuncCleaning;
		private Func<List<String>, List<IOutputItem>> _FunctionBuilder;

		private List<string> _StartAndIncludeStrings;
		private List<string> _StartAndExcludeStrings;
		private List<string> _EndAndIncludeStrings;
		private List<string> _EndAndExcludeStrings;
		private List<string> _AbortStrings;

		public SimpleStartEndAbortMatcher(List<string> startAndIncludeStrings,
			List<string> startAndExcludeStrings, List<string> endAndIncludeStrings,
			List<string> endAndExcludeStrings, List<string> abortStrings, Func<string, Tuple<bool, string>> cleaningFunction, Func<List<String>, List<IOutputItem>> builderFunction)
		{
			_StartAndIncludeStrings = startAndIncludeStrings;
			_StartAndExcludeStrings = startAndExcludeStrings;
			_EndAndIncludeStrings = endAndIncludeStrings;
			_EndAndExcludeStrings = endAndExcludeStrings;
			_AbortStrings = abortStrings;

			_FuncCleaning = cleaningFunction;
			_FunctionBuilder = builderFunction;

		}


		private bool _isProcessing = false;
		private int _startId;
		private int _endId;

		private List<string> _FullResponse = new List<string>();

		public string Name { get; set; } = "Name Not Set";

		public List<IOutputItem> ProcessAndReturnOutputItems(IOutputItem outputItem)
		{
			if (!_isProcessing)
			{
				// Only look for Start Triggers since we're not already processing
				TriggerTestResult result = IsStartTrigger(outputItem);

				if (result == TriggerTestResult.TriggerAndInclude) { AddLineToFullResponse(outputItem.Value); }

				if (result != TriggerTestResult.NotTriggered)
				{
					_startId = outputItem.Id;
					_FullResponse.Clear();
					_isProcessing = true;
					return new List<IOutputItem>();
				}

				// In all cases, we do not have a decoration return
				return new List<IOutputItem>();
			}
			else // _isProcessing == true
			{
				if (IsAbortTrigger(outputItem))
				{
					// Not the response we were looking for. Cancel out.
					_startId = -1;
					_isProcessing = false;
					return new List<IOutputItem>();
				}

				TriggerTestResult result = IsEndTrigger(outputItem);

				if (result == TriggerTestResult.TriggerAndInclude)
				{
					_endId = outputItem.Id;
					_isProcessing = false;
					AddLineToFullResponse(outputItem.Value);
					return _FunctionBuilder(_FullResponse);
				}
				else if (result == TriggerTestResult.TriggerAndExclude)
				{
					// _endId = We leave this value as is. It'll have been set with the previous processed line.
					_isProcessing = false;
					return _FunctionBuilder(_FullResponse);
				}
				else // (result == TriggerTestResult.NotTriggered)
				{
					_endId = outputItem.Id; //Keep running count
					AddLineToFullResponse(outputItem.Value);
					return new List<IOutputItem>();
				}
			}
		}

		public TriggerTestResult IsStartTrigger(IOutputItem outputItem)
		{
			if (_StartAndIncludeStrings.Contains(outputItem.Value))
			{
				Debug.WriteLine($"[{Name}] start trigger found on \"{outputItem.Value}\"");
				return TriggerTestResult.TriggerAndInclude;
			}
			else if (_StartAndExcludeStrings.Contains(outputItem.Value))
			{
				Debug.WriteLine($"[{Name}] start trigger found on \"{outputItem.Value}\"");
				return TriggerTestResult.TriggerAndExclude;
			}
			else
			{
				return TriggerTestResult.NotTriggered;
			}
		}

		public TriggerTestResult IsEndTrigger(IOutputItem outputItem)
		{
			// We end on a Marlin Response that matches one of our end triggers.
			if (outputItem is oi_MarlinResponse && _EndAndIncludeStrings.Contains(outputItem.Value))
			{
				Debug.WriteLine($"[{Name}] end trigger found on \"{outputItem.Value}\"");
				return TriggerTestResult.TriggerAndInclude;
			}
			else if (outputItem is oi_MarlinResponse && _EndAndExcludeStrings.Contains(outputItem.Value))
			{
				Debug.WriteLine($"[{Name}] end trigger found on \"{outputItem.Value}\"");
				return TriggerTestResult.TriggerAndExclude;
			}
			else
			{
				return TriggerTestResult.NotTriggered;
			}
		}

		public bool IsAbortTrigger(IOutputItem outputItem)
		{
			// If we come across a new command, then abort. We've clearly not processed an expected result.
			if (outputItem is oi_PrinterCommand) { return true; }
			if (_AbortStrings.Contains(outputItem.Value)) { return true; }

			return false;
		}

		private void AddLineToFullResponse(string s)
		{
			Tuple<bool, string> c;
			c = _FuncCleaning(s);
			if (c.Item1) { _FullResponse.Add(c.Item2); }
		}
	}
}
