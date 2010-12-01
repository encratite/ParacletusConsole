using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParacletusConsole
{
	public partial class AutoCompletionForm : Form
	{
		ConsoleHandler Handler;

		public AutoCompletionForm(ConsoleHandler handler)
		{
			InitializeComponent();
			Handler = handler;
		}

		private void TabForm_Load(object sender, EventArgs e)
		{
			Handler.OnAutoCompletionFormLoad();
		}

		private void AutoCompletionListBox_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (AutoCompletionListBox.SelectedItem != null)
			{
				string item = AutoCompletionListBox.SelectedItem.ToString();
				Handler.OnListBoxDoubleClick(item);
			}
		}
	}
}
