using System;
using System.Collections.Generic;
using System.Text;

namespace Guppy.OutputItems
{
	public interface IOutputItem
	{
		public string Value { get; set; }
		public int Id { get;}
	}
}
