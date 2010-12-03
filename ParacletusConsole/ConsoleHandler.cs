using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using System.Media;
using System.Reflection;
using System.Resources;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		delegate void KeyPressHandler();
		ConsoleForm MainForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> CommandHandlerDictionary;
		bool Terminating;
		bool ProcessIOActive;

		string CurrentWorkingDirectory;
		Dictionary<string, string> VariableDictionary;

		Process Process;

		AsynchronousReader
			StandardOutputReader,
			StandardErrorReader;

		Nil.Serialiser<Configuration> ConfigurationSerialiser;
		Configuration ProgramConfiguration;
		bool GotConfiguration;

		const char VariableSeparator = '$';
		const char ColourIdentifier = '#';

		Dictionary<char, KeyPressHandler> KeyPressHandlerDictionary;

		HashSet<string> PathNames;

		bool IsWindows;

		Thread AutoCompletionThread;
		AutoCompletionForm AutoCompletionMatchesForm;

		int OriginalAutoListBoxHeight;
		
		bool IgnoreNextLossOfFocus;
		
		string HomePath;
		string ProgramDirectory;

		EmbeddedScripting ScriptingObject;
		
		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.FormConsoleHandler = this;
			this.MainForm = consoleForm;
			Process = null;
			Terminating = false;
			ProcessIOActive = false;
			IgnoreNextLossOfFocus = true;

			ProgramDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			string configurationPath = Path.Combine(ProgramDirectory, Configuration.ConfigurationFile);

			ConfigurationSerialiser = new Nil.Serialiser<Configuration>(configurationPath);

			AutoCompletionMatchesForm = new AutoCompletionForm(this);
			OriginalAutoListBoxHeight = AutoCompletionMatchesForm.AutoCompletionListBox.Height;
			
			HomePath = Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

			LoadConfiguration();
			InitialiseVariableDictionary();
			InitialiseKeyPressHandlerDictionary();
			IsWindows = IsWindowsOS();
			PathNames = LoadPathNames();
			InitialiseCommands();
		}

		void UpdateWorkingDirectory()
		{
			CurrentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
			VariableDictionary["CurrentWorkingDirectory"] = CurrentWorkingDirectory;
			string[] tokens = CurrentWorkingDirectory.Split(Path.DirectorySeparatorChar);
			string folder = tokens[tokens.Length - 1];
			VariableDictionary["CurrentWorkingDirectoryWithoutPath"] = folder;
		}

		public void AddCommand(string command, string argumentDescription, string description, CommandHandlerFunction function, int argumentCount)
		{
			CommandHandler handler = new CommandHandler(command, argumentDescription, description, function, argumentCount);
			CommandHandlerDictionary.Add(command, handler);
		}

		void VisualiseArguments(CommandArguments arguments)
		{
			PrintLine("Command: \"" + arguments.Command + "\"");
			string output = "Arguments:";
			for (int i = 0; i < arguments.Arguments.Length; i++)
			{
				string argument = arguments.Arguments[i].Argument;
				output += " " + (i + 1).ToString() + ". \"" + argument + "\"";
			}
			PrintLine(output);
		}

		public void HandleStandardOutputRead(byte[] buffer, int bytesRead)
		{
			PrintBuffer(buffer, bytesRead);
		}

		public void HandleStandardErrorRead(byte[] buffer, int bytesRead)
		{
			PrintBuffer(buffer, bytesRead);
		}

		void PromptAndSelect()
		{
			PrintPrompt();
			MainForm.InputBox.Invoke
			(
				(MethodInvoker)delegate
				{
					MainForm.InputBox.SelectAll();
				}
			);
		}

		void ProcessTerminationCheckThread()
		{
			try
			{
				Process.WaitForExit();
			}
			catch (NullReferenceException)
			{
			}
			if (ProcessIOActive)
			{
				StandardOutputReader.CloseEvent.WaitOne();
				StandardErrorReader.CloseEvent.WaitOne();
			}
			lock (this)
			{
				Process = null;
				ProcessIOActive = false;
				if (!Terminating)
					PromptAndSelect();
			}
		}

		public void OnTermination()
		{
			lock (this)
			{
				Terminating = true;
				CloseAutoCompletionForm();
				SaveConfiguration();
				KillProcess();
			}
		}

		bool KillProcess()
		{
			if (Process != null)
			{
				Process.Kill();
				return true;
			}
			return false;
		}

		void Escape()
		{
			lock (this)
			{
				if(KillProcess())
					PrintError("Process has been terminated");
			}
		}

		bool IsWindowsOS()
		{
			string osString = System.Environment.OSVersion.ToString();
			return osString.IndexOf("Windows") != -1;
		}

		bool CaseInsensitiveCharacterComparison(char left, char right)
		{
			return Char.ToLower(left) == Char.ToLower(right);
		}

		bool CaseInsensitiveStringComparison(string left, string right)
		{
			return left.ToLower() == right.ToLower();
		}

		void Beep()
		{
			SystemSounds.Beep.Play();
		}

		public void KeyPressed(KeyPressEventArgs keyEvent)
		{
			CloseAutoCompletionForm();
			if (KeyPressHandlerDictionary.ContainsKey(keyEvent.KeyChar))
			{
				keyEvent.Handled = true;
				KeyPressHandlerDictionary[keyEvent.KeyChar]();
			}
		}
	}
}
