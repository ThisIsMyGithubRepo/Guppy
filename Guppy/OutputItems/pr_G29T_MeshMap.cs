using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	public class pr_G29T_MeshMap: IOutputItem
	{
		public string Value { get; set; }
		public int Id { get; private set; }
		public double[,] MeshValues { get; private set; }
		
		public pr_G29T_MeshMap(int id, string value, double[,] meshValues)
		{
			Id = id;
			Value = value;
			MeshValues = meshValues;
		}

		public pr_G29T_MeshMap(int id, string value, List<string> commandList)
		{
			Id = id;
			Value = value;
		}
	}
}
