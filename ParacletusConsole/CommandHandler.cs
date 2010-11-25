using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class CommandHandler
	{
		public string command;
		public string description;
		public string argumentDescription;
		public ConsoleHandler.CommandHandlerFunction function;
		public int argumentCount;

		public CommandHandler(string command, string argumentDescription,  string description, ConsoleHandler.CommandHandlerFunction function, int argumentCount)
		{
			this.command = command;
			this.argumentDescription = argumentDescription;
			this.description = description;
			this.function = function;
			this.argumentCount = argumentCount;
		}

		public string usage()
		{
			return command + " " + argumentDescription + " - " + description;
		}
	}
}
