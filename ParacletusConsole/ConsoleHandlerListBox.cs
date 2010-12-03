using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		public void OnListBoxDoubleClick(string entry)
		{
			lock (this)
			{
				MainForm.InputBox.Invoke(
					(MethodInvoker)delegate
					{
						ProcessListBoxDoubleClick(entry);
					}
				);
				CloseAutoCompletionForm();
			}
		}

		void ProcessListBoxDoubleClick(string entry)
		{
			string line = MainForm.InputBox.Text;
			int offset = MainForm.InputBox.SelectionStart;
			CommandArguments arguments;
			ArgumentResult activeArgument;
			try
			{
				arguments = new CommandArguments(line);
				activeArgument = arguments.FindMatchingResult(offset);
				PerformInputBoxReplacement(line, entry, activeArgument);
			}
			catch (ArgumentException)
			{
				Beep();
				return;
			}
		}
	}
}
