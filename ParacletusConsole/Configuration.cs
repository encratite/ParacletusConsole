using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class Configuration
	{
		public Nil.FormState FormState;

		public static string ConfigurationFile = "Paracletus.xml";

		public Configuration()
		{
			FormState = new Nil.FormState();
		}
	}
}
