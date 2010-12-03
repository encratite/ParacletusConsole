using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		void InitialiseCommands()
		{
			CommandHandlerDictionary = new Dictionary<string, CommandHandler>();
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

			if (!File.Exists(scriptPath))
			{
				//copy the embedded default script to the default location
				StreamReader reader = new StreamReader(Assembly.GetExecutingAssembly().GetManifestResourceStream("ParacletusConsole.DefaultScript.cs"));
				string data = reader.ReadToEnd();
				reader.Close();
				StreamWriter writer = new StreamWriter(scriptPath);
				writer.Write(data);
				writer.Close();
			}

			LoadScript(scriptPath);
		}
	}
}
