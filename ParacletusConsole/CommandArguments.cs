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

		bool ArgumentHasQuotes;

		public CommandArguments(string line)
		{
			ProcessLine(line);
		}

		void ProcessResult(string currentToken, int currentTokenOffset, List<ArgumentResult> tokens)
		{
			ArgumentResult newResult = new ArgumentResult(currentToken, currentTokenOffset, ArgumentHasQuotes);
			tokens.Add(newResult);
			ArgumentHasQuotes = false;
		}

		void ProcessLine(string line)
		{
			List<ArgumentResult> tokens = new List<ArgumentResult>();
			string currentToken = "";
			int currentTokenOffset = 0;
			bool quoteMode = false;
			char lastChar = '\x00';
			ArgumentHasQuotes = false;

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
								ProcessResult(currentToken, currentTokenOffset, tokens);
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
						if (quoteMode)
						{
							ArgumentHasQuotes = true;
							currentTokenOffset = i;
						}
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
				ProcessResult(currentToken, currentTokenOffset, tokens);

			Command = tokens[0];
			Arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
		}

		public string GetQuotedArguments()
		{
			string output = "";
			bool first = true;
			foreach (ArgumentResult argumentResult in Arguments)
			{
				if (first)
					first = false;
				else
					output += " ";
				output += argumentResult.EscapeArgument();
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

		public ArgumentResult FindMatchingResult(int offset)
		{
			if (Command.Match(offset))
				return Command;
			foreach (ArgumentResult result in Arguments)
			{
				if (result.Match(offset))
					return result;
			}
			throw new ArgumentException("The offset does not match any of the arguments");
		}
	}
}
