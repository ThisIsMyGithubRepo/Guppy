using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Documents;
using Guppy.OutputItems;
using System.Windows.Controls;

namespace Guppy
{
	static class FlowDocumentFactory
	{

		public static Paragraph GetFlowDocumentParagraphContent(IOutputItem OI)
		{
			return CreateParagraphFromString(OI.Value);
		}

		static private Paragraph CreateParagraphFromString(string s)
		{
			Paragraph p = new Paragraph();
			p.Inlines.Add(new Run(s));
			return p;
		}
	}
}
