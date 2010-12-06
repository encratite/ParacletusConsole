using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class Alias
	{
		public string
			Identifier,
			Command;

		public Alias(string identifier, string command)
		{
			Identifier = identifier;
			Command = command;
		}
	}
}
