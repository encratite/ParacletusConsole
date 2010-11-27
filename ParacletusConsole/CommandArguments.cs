using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	class CommandArguments
	{
		public ArgumentResult Command;
		public ArgumentResult[] Arguments;

		public CommandArguments(string line)
		{
			List<ArgumentResult> tokens = new List<ArgumentResult>();
			string currentToken = "";
			int currentTokenOffset = 0;
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
							currentToken += input;
						else
						{
							if(currentToken.Length > 0)
							{
								ArgumentResult newResult = new ArgumentResult(currentToken, currentTokenOffset);
								tokens.Add(newResult);
								currentToken = "";
							}
						}
						break;

					case '"':
						if (lastChar == '\\')
						{
							currentToken.Remove(currentToken.Length - 1);
							currentToken += input;
							break;
						}
						if (quoteError)
							throw new ArgumentException("Encountered a quote right after a terminating quote");
						quoteMode = !quoteMode;
						if(quoteMode)
							currentTokenOffset = i;
						break;

					default:
						if (quoteError)
							throw new ArgumentException("Encountered a printable character after a terminating quote");
						if(currentToken.Length == 0)
							currentTokenOffset = i;
						currentToken += input;
						break;
				}
				lastChar = input;
			}

			if (quoteMode)
				throw new ArgumentException("Missing quote");

			if (currentToken.Length > 0)
			{
				ArgumentResult lastResult = new ArgumentResult(currentToken, currentTokenOffset);
				tokens.Add(lastResult);
			}

			Command = tokens[0];
			Arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
		}

		public string GetQuotedArguments()
		{
			string output = "";
			bool first = true;
			foreach (ArgumentResult argumentResult in Arguments)
			{
				string argument = argumentResult.Argument;
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

		public string[] GetArgumentString()
		{
			List<string> output = new List<string>();
			foreach (ArgumentResult result in Arguments)
				output.Add(result.Argument);
			return output.ToArray();
		}
	}
}
