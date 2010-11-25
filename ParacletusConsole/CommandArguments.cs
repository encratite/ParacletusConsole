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
			bool quoteMode = false;
			char lastChar = '\x00';
			for(int i = 0; i < line.Length; i++)
			{
				char input = line[i];
				bool wasQuote = i > 0 && lastChar == '"';
				bool quoteError = !quoteMode && wasQuote;
				switch (input)
				{
					case ' ':
						if(quoteMode)
							currenToken += input;
						else
						{
							if(currenToken.Length > 0)
							{
								tokens.Add(currenToken);
								currenToken = "";
							}
						}
						break;

					case '"':
						if (quoteError)
							throw new ArgumentException("Encountered a quote right after a terminating quote");
						quoteMode = !quoteMode;
						break;

					default:
						if (quoteError)
							throw new ArgumentException("Encountered a printable character after a terminating quote");
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

		public string GetQuotedArguments()
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
