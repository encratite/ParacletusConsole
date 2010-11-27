using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	class ArgumentResult
	{
		public string Argument;
		public int Offset;

		public ArgumentResult(string argument, int offset)
		{
			Argument = argument;
			Offset = offset;
		}

		public bool Match(int offset)
		{
			return offset >= Offset && offset < Offset + Argument.Length;
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
	}
}
