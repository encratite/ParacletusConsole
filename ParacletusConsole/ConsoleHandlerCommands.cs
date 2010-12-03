using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		public void ChangeDirectory(string[] arguments)
		{
			try
			{
				System.IO.Directory.SetCurrentDirectory(arguments[0]);
			}
			catch (DirectoryNotFoundException exception)
			{
				PrintError(exception.Message);
			}
		}

		public void PrintHelp(string[] arguments)
		{
			CommandHandler[] handlers = CommandHandlerDictionary.Values.ToArray();
			Array.Sort(handlers);
			PrintLineWithColour("Help menu:\n", ProgramConfiguration.TitleColour);
			foreach (CommandHandler handler in handlers)
			{
				PrintWithColour("  " + handler.Command, ProgramConfiguration.HighlightColour);
				string argumentDescriptionString = "";
				if (handler.ArgumentDescription != null)
					argumentDescriptionString = " " + handler.ArgumentDescription;
				PrintLine(argumentDescriptionString + " - " + handler.Description);
			}
		}

		public void ClearConsole(string[] arguments)
		{
			MainForm.ConsoleBox.Text = "";
		}
	}
}
