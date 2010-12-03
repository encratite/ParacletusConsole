using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.CodeDom.Compiler;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		string GetErrorString(CompilerError error, string description)
		{
			return description + " in file " + error.FileName + " (line " + error.Line + "): " + error.ErrorText + " (error number " + error.ErrorNumber + ")";
		}

		public bool LoadScript(string scriptPath)
		{
			CompilerResults results;

			try
			{
				StreamReader reader = new StreamReader(scriptPath);
				string code = reader.ReadToEnd();
				reader.Close();
				results = ScriptingObject.Compile(code);

				if (results.Errors.HasWarnings)
				{
					foreach(CompilerError error in results.Errors)
					{
						if (error.IsWarning)
						{
							string message = GetErrorString(error, "Warning");
							PrintWarning(message);
						}
					}
				}

				if (results.Errors.HasErrors)
				{
					foreach(CompilerError error in results.Errors)
					{
						if(!error.IsWarning)
						{
							string message = GetErrorString(error, "Error");
							PrintError(message);
						}
					}
					return false;
				}
			}
			catch (FileNotFoundException)
			{
				PrintError("Unable to load script " + scriptPath);
				return false;
			}
			return ProcessScriptAssembly(results);
		}

		bool ProcessScriptAssembly(CompilerResults results)
		{
			try
			{
				Assembly assembly = results.CompiledAssembly;
				foreach (Type type in assembly.GetExportedTypes())
				{
					foreach (Type currentInterface in type.GetInterfaces())
					{
						if (currentInterface == typeof(IConsoleScript))
							ConstructAndExecuteScriptInterface(type);
					}
				}

			}
			catch (Exception exception)
			{
				PrintError("An exception occured:");
				PrintError(exception.ToString());
				return false;
			}
			return true;
		}

		void ConstructAndExecuteScriptInterface(Type currentInterface)
		{
			ConstructorInfo constructor = currentInterface.GetConstructor(System.Type.EmptyTypes);
			if (constructor == null)
				throw new ArgumentException("Encountered a null constructor");
			if (!constructor.IsPublic)
				throw new ArgumentException("The constructor of a scripting interface must be public");
			IConsoleScript scriptObject = constructor.Invoke(null) as IConsoleScript;
			scriptObject.Execute(this);
		}
	}
}
