using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	public class CommandArgument
	{
		public string Argument;
		public int Offset;
		public bool HasQuotes;

		public CommandArgument(string argument, int offset = 0, bool hasQuotes = false)
		{
			Argument = argument;
			Offset = offset;
			HasQuotes = hasQuotes;
		}

		public bool Match(int offset)
		{
			return offset >= Offset && offset <= Offset + Argument.Length;
		}

		public string EscapeArgument()
		{
			string output = Argument.Replace("\"", "\\\"");
			if (output.IndexOf(' ') != -1)
				output = "\"" + output + "\"";
			return output;
		}

		public string CurrentTabTerm()
		{
			string[] tokens = Argument.Split(Path.DirectorySeparatorChar);
			return tokens[tokens.Length - 1];
		}

		public int Length()
		{
			int output = Argument.Length;
			if (HasQuotes)
				output += 2;
			return output;
		}
	}
}
