using System;
using System.Collections.Generic;
using System.Text;
using Guppy.OutputItems;

namespace Guppy.ResponseProcessing
{
	public interface IResponseProcessor
	{
		public List<IOutputItem> ProcessAndReturnOutputItems(IOutputItem outputItem);
		public string Name { get; set; }
	}
}
