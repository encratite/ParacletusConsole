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
	public partial class ConsoleForm : Form
	{
		public ConsoleHandler ConsoleHandler;

		public ConsoleForm()
		{
			InitializeComponent();
		}

		private void inputBox_KeyPress(object sender, KeyPressEventArgs keyEvent)
		{
			ConsoleHandler.KeyPressed(keyEvent);
		}

		private void ConsoleForm_Load(object sender, EventArgs e)
		{
			ConsoleHandler.OnMainFormLoaded();
		}

		private void ConsoleForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			ConsoleHandler.Closing();
		}

		private void ConsoleForm_LocationChanged(object sender, EventArgs e)
		{
			ConsoleHandler.UpdateAutoCompletionFormPosition();
		}
	}
}
