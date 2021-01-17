using System;
using System.Collections.Generic;
using System.Text;

namespace Guppy.OutputItems.Builders
{
	public interface IOutputItemBuilder
	{
		public IOutputItem BuildFromCommandList(List<string> commandList);
	}
}
