using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class CommandHandler
	{
		public string Command;
		public string Description;
		public string ArgumentDescription;
		public ConsoleHandler.CommandHandlerFunction Function;
		public int ArgumentCount;

		public CommandHandler(string command, string argumentDescription,  string description, ConsoleHandler.CommandHandlerFunction function, int argumentCount)
		{
			this.Command = command;
			this.ArgumentDescription = argumentDescription;
			this.Description = description;
			this.Function = function;
			this.ArgumentCount = argumentCount;
		}

		public string Usage()
		{
			return Command + " " + ArgumentDescription + " - " + Description;
		}
	}
}
