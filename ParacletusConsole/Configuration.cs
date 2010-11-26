using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class Configuration
	{
		public static string ConfigurationFile = "Paracletus.xml";

		public string Prompt;
		public Nil.FormState FormState;

		public Configuration()
		{
			Prompt = "#eaff00$User$#96ff00@#eaff00$MachineName$ #7debff$CurrentWorkingDirectory$#506dff>#ffffff ";
			FormState = new Nil.FormState();
		}
	}
}
