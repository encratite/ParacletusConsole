using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		public void ChangeDirectory(string[] arguments)
		{
			try
			{
				System.IO.Directory.SetCurrentDirectory(arguments[0]);
			}
			catch (DirectoryNotFoundException exception)
			{
				PrintError(exception.Message);
			}
		}
	}
}
