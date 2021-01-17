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

		//static private Paragraph CreateParagraphWithButton(string s)
		//{
		//	Paragraph p = CreateParagraphFromString(s);
		//	Button b = new Button();
		//	b.Content = "Button!";
		//	b.IsEnabled = true;
		//	InlineUIContainer inline = new InlineUIContainer(b);
		//	p.Inlines.Add(inline);
		//	return p;
		//}
	}
}
