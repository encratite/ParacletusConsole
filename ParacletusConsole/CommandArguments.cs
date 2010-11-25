using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	class CommandArguments
	{
		public string command;
		public string[] arguments;

		public CommandArguments(string line)
		{
			List<string> tokens = new List<string>();
			string currenToken = "";
			bool newToken = true;
			bool quoteMode = false;
			char lastChar = '\x00';
			for(int i = 0; i < line.Length; i++)
			{
				char input = line[i];
				switch (input)
				{
					case ' ':
						if(quoteMode)
							currenToken += input;
						else
						{
							if(currenToken.Length > 0)
							{
								newToken = true;
								tokens.Add(currenToken);
								currenToken = "";
							}
						}
						break;

					case '"':
						quoteMode = !quoteMode;
						newToken = false;
						break;

					default:
						if (i > 0 && lastChar == '"')
							throw new ArgumentException("Encountered a printable character after a terminating quote");
						newToken = false;
						currenToken += input;
						break;
				}
				lastChar = input;
			}

			if (quoteMode)
				throw new ArgumentException("Missing quote");

			if (currenToken.Length > 0)
				tokens.Add(currenToken);

			command = tokens[0];
			arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
		}

		public string getQuotedArguments()
		{
			string output = "";
			bool first = true;
			foreach (string argument in arguments)
			{
				if (first)
					first = false;
				else
					output += " ";
				if (argument.IndexOf(' ') == -1)
					output += argument;
				else
					output += "\"" + argument + "\"";
			}
			return output;
		}
	}
}
