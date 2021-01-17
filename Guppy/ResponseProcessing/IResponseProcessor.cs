using System;
using System.Collections.Generic;
using System.Text;
using Guppy.OutputItems;

namespace Guppy.ResponseProcessing
{
	public interface IResponseProcessor
	{
		public IOutputItem ProcessAndReturnOutput(IOutputItem outputItem);
		public string Name { get; set; }
	}
}
