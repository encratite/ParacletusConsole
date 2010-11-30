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
	public partial class TabForm : Form
	{
		ConsoleForm MainForm;

		public TabForm(ConsoleForm mainForm)
		{
			InitializeComponent();
			MainForm = mainForm;
		}

		private void TabForm_Load(object sender, EventArgs e)
		{
			Left = MainForm.Left + MainForm.InputBox.Left;
			Top = MainForm.Top + MainForm.InputBox.Top;
		}
	}
}
