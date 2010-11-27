using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
	}
}
