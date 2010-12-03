using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		public void OnMainFormLoaded()
		{
			if (GotConfiguration)
				ProgramConfiguration.FormState.Apply(MainForm);
			PrintPrompt();
			MainForm.InputBox.Focus();
		}

		public void OnMainFormLossOfFocus()
		{
			if (IgnoreNextLossOfFocus)
			{
				IgnoreNextLossOfFocus = false;
				return;
			}

			if (AutoCompletionThread != null)
			{
				AutoCompletionMatchesForm.Invoke(
				(MethodInvoker)delegate
				{
					if (!(AutoCompletionMatchesForm.Focused || AutoCompletionMatchesForm.AutoCompletionListBox.Focused))
						CloseAutoCompletionForm();
				}
				);
			}
		}
	}
}
