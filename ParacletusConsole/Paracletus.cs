using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ParacletusConsole
{
	static class Paracletus
	{
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			ConsoleForm consoleForm = new ConsoleForm();
			ConsoleHandler handler = new ConsoleHandler(consoleForm);

			Application.Run(consoleForm);
		}
	}
}
