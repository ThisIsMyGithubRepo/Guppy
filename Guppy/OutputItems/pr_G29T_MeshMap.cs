using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace Guppy.OutputItems
{
	public class pr_G29T_MeshMap : IOutputItem
	{
		// Static helpers associated with this processed response
		#region Static Helper Methods

		public static List<IOutputItem> BuildProcessedResponseG29T(List<String> commandList)
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

			return new List<IOutputItem> { new pr_G29T_MeshMap(MarlinOutputItemFactory.GetId(), "G29 T Mesh Map - Double Click to View", mesh) };
		}

		//public static Tuple<bool, string> CleanMarlinG29T1UBLMeshResponse(string s)
		//{
		//	return MarlinStringHelpers.StripAndRemoveGeneric(new List<string>(),
		//		new List<string>() { "Bed Topography Report for CSV:" },
		//		s);
		//}

		#endregion

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
