using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Guppy
{
	/// <summary>
	/// Interaction logic for MacroEditWindow.xaml
	/// </summary>
	public partial class MacroEditWindow : Window
	{
		public MacroItem Macro { get; set; }
		public int MacroNumber { get; set; }

		public MacroEditWindow(MacroItem m, int Number)
		{
			InitializeComponent();
			Macro = m;
			this.DataContext = this;
		}

		private void cmdSave_Click(object sender, RoutedEventArgs e)
		{
				this.DialogResult = true;
		}

		public MacroItem GetUpdatedMacro()
		{
			return Macro;
		}
	}
}
