using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Guppy.ResponseProcessing;
using Guppy.OutputItems;

namespace Guppy
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SerialPort _port;
		private List<int> _baudRates = new List<int>() { 9600, 19200, 38400, 57600, 115200 };
		private AppSettings _settings = new AppSettings();
		private Queue<string> _oldCommands = new Queue<string>();
		private int _maxCommandsToRemember = 5;
		private int _oldCommandIndex = 0;
		private List<IResponseProcessor> _responseProcessors;
		private FlowDocument flowDocument = new FlowDocument();

		public MainWindow()
		{
			InitializeComponent();
			SanitizeSettings(ref _settings);
			UpdateConnectedStatus(false);
			_responseProcessors = MarlinOutputItemFactory.BootstrapResponseProcessors();

			//InitializeResponseSelectors();

			flowDocument.IsEnabled = true;

			listOutput.Document = flowDocument;

			try
			{
				cbPort.ItemsSource = GetSerialPortList();
				cbPort.SelectedItem = _settings.Port;

				cbBaud.ItemsSource = _baudRates;
				cbBaud.SelectedItem = _settings.BaudRate;

				CreateMacroItemsAndBindToButtons();
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		#region View
		private void txtCommandToSend_PreviewKeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					SendTextboxContentsAsCommand();
					txtCommandToSend.Text = string.Empty;
					e.Handled = true;
					break;
				case Key.Up:
					SetTextboxToCommand(GetPrevCommand());
					break;
				case Key.Down:
					SetTextboxToCommand(GetNextCommand());
					break;
				case Key.Escape:
					SetTextboxToCommand(String.Empty);
					_oldCommandIndex = 0;
					break;
				default:
					break;
			}
		}

		private void cmdSend_Click(object sender, RoutedEventArgs e)
		{
			SendTextboxContentsAsCommand();
			txtCommandToSend.Text = string.Empty;
		}

		private void btnConnect_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				OpenSerialPort();
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		private void btnDisconnect_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				CloseSerialPort();
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		private void btnMacro_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				ExecuteMacro(GetMacroFromButton(sender));
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		private void cbPort_DropDownOpened(object sender, EventArgs e)
		{
			try
			{
				cbPort.ItemsSource = GetSerialPortList();
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		private void btnMacro_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			MacroItem _macroForEdit = GetMacroFromButton(sender).ShallowClone();
			EditMacro((Button)sender, _macroForEdit);
		}

		private void EditMacro(Button macroButton, MacroItem macroForEdit)
		{

			MacroEditWindow _win = new MacroEditWindow(macroForEdit, 1);
			if (_win.ShowDialog() == true)
			{
				MacroItem updatedMacro = _win.GetUpdatedMacro();
				macroButton.DataContext = updatedMacro;
				SaveCurrentMacrosToUserSettings();
			}
		}

		private void ShowMeshView(pr_G29T_MeshMap mm)
		{
			MeshView _win = new MeshView(mm);
			
			_win.ShowDialog();
			_win = null;
		}


		private void btnMacro_Drop(object sender, DragEventArgs e)
		{
			try
			{
				if (e.Data.GetDataPresent(typeof(pr_M503orM501_Config)))
				{
					pr_M503orM501_Config o = e.Data.GetData(typeof(pr_M503orM501_Config)) as pr_M503orM501_Config;
					MacroItem _macroForEdit = new MacroItem(GetMacroFromButton(sender).Label, o.CommandList); //The macro object will sanitize the command list.
					if(_macroForEdit.Label==string.Empty)
					{
						_macroForEdit.Label = "M503 or M501 Config";
					}
					EditMacro((Button)sender, _macroForEdit);
				}
				else if (e.Data.GetDataPresent(DataFormats.Text))
				{
					string s = (string)e.Data.GetData(DataFormats.Text);

					// Strip off any non-command stuff in case the user is drag-dropping echo'ed commands.
					s = MarlinStringHelpers.CleanMarlinResponseAndRemoveTextAndLinesNotNeededForCommands(s).Item2;

					// Take the label from the existing button and the command text from the drag/drop.
					MacroItem _macroForEdit = new MacroItem(GetMacroFromButton(sender).Label, s); //The macro object will sanitize the command string.
					EditMacro((Button)sender, _macroForEdit);
				}
			}
			catch { }
		}

		private void btnMacro_DragOver(object sender, DragEventArgs e)
		{
			e.Effects = DragDropEffects.None;
			if (e.Data.GetDataPresent(typeof(pr_M503orM501_Config)))
			{
				e.Effects = DragDropEffects.Copy;
			}
			else if (e.Data.GetDataPresent(DataFormats.Text))
			{
				e.Effects = DragDropEffects.Copy;
			}
		}

		private void UpdateConnectedStatus(bool isConnected)
		{
			NotConnectedIndicator.Visibility = isConnected ? Visibility.Hidden : Visibility.Visible;
			ConnectedIndicator.Visibility = isConnected ? Visibility.Visible : Visibility.Hidden;
		}

		#endregion

		#region Serial

		private List<string> GetSerialPortList()
		{
			// Get a list of serial port names.
			string[] ports = SerialPort.GetPortNames();

			Debug.WriteLine($"The following serial ports were found: {string.Join(", ", ports)}");
			return ports.ToList<string>();
		}



		private void SanitizeSettings(ref AppSettings settings)
		{
			settings.BaudRate = _baudRates.Contains(settings.BaudRate) ? settings.BaudRate : 115200;
		}

		private void CloseSerialPort()
		{
			try
			{
				UpdateConnectedStatus(false);
				_port.Close();
				_port.Dispose();

			}
			catch { }
		}

		private void OpenSerialPort()
		{
			try
			{
				if (_port != null)
				{
					UpdateConnectedStatus(false);
					_port.Close();
					_port.Dispose();
				}
			}
			catch { }

			_port = new SerialPort((String)cbPort.SelectedValue, (int)cbBaud.SelectedValue, (Parity)Enum.Parse(typeof(Parity), _settings.Parity), _settings.DataBits, (StopBits)_settings.StopBits);
			_port.Handshake = Handshake.RequestToSendXOnXOff;
			_port.DtrEnable = true;
			_port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
			_port.Open();
			UpdateConnectedStatus(true);
		}

		public void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			try
			{
				string s = _port.ReadLine();
				Debug.WriteLine($"Received: {s}");
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildMarlinResponseOutputItem(s));
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		#endregion

		#region Commands
		private void SendTextboxContentsAsCommand()
		{
			try
			{
				string s = MarlinStringHelpers.CleanCommandString(txtCommandToSend.Text);
				SendCommandToPort(s);
				if (_oldCommandIndex == 0) // New Command, not one from the history
				{
					AddCommandToOldCommandQueue(s);
				}
				_oldCommandIndex = 0;
			}
			catch (Exception err)
			{
				AddItemToOutputCollection(MarlinOutputItemFactory.BuildApplicationMessageOutputItem(err.Message));
			}
		}

		public void SendCommandToPort(string s)
		{
			_port.WriteLine(s);
			AddItemToOutputCollection(MarlinOutputItemFactory.BuildPrinterCommandOutputItem(s));
		}



		private void SetTextboxToCommand(string s)
		{
			txtCommandToSend.Text = s;
		}
		#endregion

		#region Command Queue
		// The Command Queue stores the last _maxCommandsToRememeber commands.
		// General idea is that it maintains the queue and allows access using an offset-indicator: _oldCommandIndex.
		// When _oldCommandIndex == 0, then we have not scrolled back in the command stack.
		// Otherwise, _oldCommandIndex is the count in the queue we last accessed.
		// E.g. if _oldCommandIndex = 4 then we last returned the 4th item in the queue (which has an 0 based index of 3).
		// The _oldCommandIndex wraps from beginning to end, and end to beginning.
		// Other parts of the code must reset the _oldCommandIndex to 0 when they consider command scrolling to be complete.
		//
		// This should probably be extracted into it's own class in the future.

		/// <summary>
		/// Adds a command to the queue and, if necessary, removes the oldest item to maintain queue size.
		/// </summary>
		/// <param name="s"></param>
		private void AddCommandToOldCommandQueue(string s)
		{
			_oldCommands.Enqueue(s);
			while (_oldCommands.Count > _maxCommandsToRemember)
			{
				_oldCommands.Dequeue(); //Burn an item from the queue.
			}
		}

		/// <summary>
		/// Gets the previous command, relative to the current position scrolling through the queue.
		/// </summary>
		/// <returns></returns>
		private string GetPrevCommand()
		{
			if (_oldCommands.Count == 0)
			{
				_oldCommandIndex = 0;

				// Special case, no previous commands.
				return string.Empty;
			}

			_oldCommandIndex--;
			if (_oldCommandIndex < 1) { _oldCommandIndex = _oldCommands.Count(); }

			return (_oldCommands.ToArray()[_oldCommandIndex - 1]);
		}

		/// <summary>
		/// Gets the next command, relative to the current position scrolling through the queue.
		/// </summary>
		/// <returns></returns>
		private string GetNextCommand()
		{
			if (_oldCommands.Count == 0)
			{
				_oldCommandIndex = 0;

				// Special case, no previous commands.
				return string.Empty;
			}

			_oldCommandIndex++;
			if (_oldCommandIndex > _oldCommands.Count()) { _oldCommandIndex = 1; }

			return (_oldCommands.ToArray()[_oldCommandIndex - 1]);
		}

		#endregion

		#region Responses and Output List

		/// <summary>
		/// Adds an item to the _OutputItems collection, which listOutput is bound to.
		/// Autoscrolls the list to the newly added item.
		/// </summary>
		/// <param name="item"></param>
		private void AddItemToOutputCollection(IOutputItem OI)
		{
			Dispatcher.Invoke(() =>
						{
							Paragraph p = FlowDocumentFactory.GetFlowDocumentParagraphContent(OI);
							p.DataContext = OI;
							p.Style = GetStyleForOutputType(OI);
							AddEventHandlersForOutputType(p, OI);
							p.DataContext = OI;
							listOutput.Document.Blocks.Add(p);
							listOutput.ScrollToEnd();
							listOutput.Document.Blocks.LastBlock.BringIntoView();
						});

			GetProcessedResponses(OI);
		}

		private void AddEventHandlersForOutputType(Paragraph p, IOutputItem OI)
		{
			if (OI is pr_M503orM501_Config)
			{
				p.MouseLeftButtonDown += P_MouseLeftButtonDownDrag;
			}
			else if (OI is pr_G29T_MeshMap)
			{
				p.MouseDown += P_MouseLeftButtonDownMeshDoubleClick;
			}
		}

		private void P_MouseLeftButtonDownDrag(object sender, MouseButtonEventArgs e)
		{
			Debug.WriteLine("Drag Begun on Processed Response");
			Paragraph p = sender as Paragraph;

			DragDrop.DoDragDrop(p, p.DataContext, DragDropEffects.Copy);
		}

		private void P_MouseLeftButtonDownMeshDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if (e.ClickCount>1)
			{
				Debug.WriteLine("Double Click on Mesh Occured");
				pr_G29T_MeshMap mm = ((Paragraph)sender).DataContext as pr_G29T_MeshMap;
				ShowMeshView(mm);
			}
		}

		private Style GetStyleForOutputType(IOutputItem OI)
		{
			if (OI is oi_MarlinResponse) { return this.FindResource("MarlinResponseTextStyle") as Style; }
			else if (OI is oi_PrinterCommand) { return this.FindResource("PrinterCommandTextStyle") as Style; }
			else if (OI is oi_ApplicationMessage) { return this.FindResource("ApplicationMessageTextStyle") as Style; }
			else if (OI is pr_M503orM501_Config) { return this.FindResource("ProcessedResponseTextStyle") as Style; }
			else if (OI is pr_G29T_MeshMap) { return this.FindResource("ProcessedResponseTextStyle") as Style; }
			else { return this.FindResource("FallthroughTextStyle") as Style; }
		}

		private void GetProcessedResponses(IOutputItem outputItem)
		{
			IOutputItem o;

			foreach (IResponseProcessor i in _responseProcessors)
			{
				o = i.ProcessAndReturnOutput(outputItem);
				if (o != null) { AddItemToOutputCollection(o); }
			}
		}

		#endregion

		#region Macros
		/// <summary>
		/// Sends each command to the Port.
		/// </summary>
		/// <param name="m"></param>
		private void ExecuteMacro(MacroItem m)
		{
			foreach (string s in m.CommandList)
			{
				SendCommandToPort(s);
			}
		}

		/// <summary>
		/// Instantiate all the MacroItems, and bind each one to it's respective button.
		/// </summary>
		private void CreateMacroItemsAndBindToButtons()
		{
			btnM1.DataContext = new MacroItem(_settings.M1);
			btnM2.DataContext = new MacroItem(_settings.M2);
			btnM3.DataContext = new MacroItem(_settings.M3);
			btnM4.DataContext = new MacroItem(_settings.M4);
			btnM5.DataContext = new MacroItem(_settings.M5);
			btnM6.DataContext = new MacroItem(_settings.M6);
			btnM7.DataContext = new MacroItem(_settings.M7);
			btnM8.DataContext = new MacroItem(_settings.M8);
			btnM9.DataContext = new MacroItem(_settings.M9);
			btnM10.DataContext = new MacroItem(_settings.M10);
			btnM11.DataContext = new MacroItem(_settings.M11);
			btnM12.DataContext = new MacroItem(_settings.M12);
			btnM13.DataContext = new MacroItem(_settings.M13);
			btnM14.DataContext = new MacroItem(_settings.M14);
			btnM15.DataContext = new MacroItem(_settings.M15);
			btnM16.DataContext = new MacroItem(_settings.M16);
			btnM17.DataContext = new MacroItem(_settings.M17);
			btnM18.DataContext = new MacroItem(_settings.M18);
			btnM19.DataContext = new MacroItem(_settings.M19);
			btnM20.DataContext = new MacroItem(_settings.M20);
		}

		/// <summary>
		/// Convenience method to extract the MacroItem object that is bound to the supplied object.
		/// Object is expected to be one of the macro buttons.
		/// </summary>
		/// <param name="sender"></param>
		/// <returns></returns>
		private MacroItem GetMacroFromButton(object sender)
		{
			Button btn;

			btn = (Button)sender;
			return (MacroItem)btn.DataContext;
		}

		/// <summary>
		/// Takes the current macro objects, which are attached to the buttons, and writes them to their respective
		/// user settings.
		/// </summary>
		private void SaveCurrentMacrosToUserSettings()
		{
			_settings.M1 = btnM1.DataContext.ToString();
			_settings.M2 = btnM2.DataContext.ToString();
			_settings.M3 = btnM3.DataContext.ToString();
			_settings.M4 = btnM4.DataContext.ToString();
			_settings.M5 = btnM5.DataContext.ToString();
			_settings.M6 = btnM6.DataContext.ToString();
			_settings.M7 = btnM7.DataContext.ToString();
			_settings.M8 = btnM8.DataContext.ToString();
			_settings.M9 = btnM9.DataContext.ToString();
			_settings.M10 = btnM10.DataContext.ToString();
			_settings.M11 = btnM11.DataContext.ToString();
			_settings.M12 = btnM12.DataContext.ToString();
			_settings.M13 = btnM13.DataContext.ToString();
			_settings.M14 = btnM14.DataContext.ToString();
			_settings.M15 = btnM15.DataContext.ToString();
			_settings.M16 = btnM16.DataContext.ToString();
			_settings.M17 = btnM17.DataContext.ToString();
			_settings.M18 = btnM18.DataContext.ToString();
			_settings.M19 = btnM19.DataContext.ToString();
			_settings.M20 = btnM20.DataContext.ToString();
			_settings.Save();
		}
		#endregion
	}
}
