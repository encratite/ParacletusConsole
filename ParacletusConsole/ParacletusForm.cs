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
		public ConsoleHandler consoleHandler;

		public ConsoleForm()
		{
			InitializeComponent();
		}

		private void inputBox_KeyPress(object sender, KeyPressEventArgs keyEvent)
		{
			if (keyEvent.KeyChar == '\r')
			{
				//suppress beep
				keyEvent.Handled = true;
				consoleHandler.Enter();
			}
		}

		private void ConsoleForm_Load(object sender, EventArgs e)
		{
			consoleHandler.FormLoaded();
		}
	}
}
