using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		void InitialiseCommands()
		{
			CommandHandlerDictionary = new Dictionary<string, CommandHandler>();
			AddCommand("cd", "<directory>", "change the working directory", this.ChangeDirectory, 1);
		}

		void InitialiseKeyPressHandlerDictionary()
		{
			KeyPressHandlerDictionary = new Dictionary<char, KeyPressHandler>();
			KeyPressHandlerDictionary['\r'] = Enter;
			KeyPressHandlerDictionary['\t'] = Tab;
			KeyPressHandlerDictionary['\x1b'] = Escape;
		}

		void InitialiseVariableDictionary()
		{
			string machineName = Environment.MachineName;
			if (ProgramConfiguration.LowerCaseMachineName)
				machineName = machineName.ToLower();
			VariableDictionary = new Dictionary<string, string>();
			VariableDictionary.Add("User", Environment.UserName);
			VariableDictionary.Add("MachineName", machineName);
		}

		void InitialiseEmbeddedScripting()
		{
			ScriptingObject = new EmbeddedScripting();
			string scriptPath = Path.Combine(ProgramDirectory, Configuration.DefaultScriptFile);
			LoadScript(scriptPath);
		}
	}
}
