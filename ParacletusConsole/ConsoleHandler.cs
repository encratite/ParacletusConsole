using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ParacletusConsole
{
	public class ConsoleHandler
	{
		ConsoleForm MainForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> CommandHandlerDictionary;
		bool Terminating;
		bool Writable;

		string CurrentWorkingDirectory;
		Dictionary<string, string> VariableDictionary;

		Process Process;

		AsynchronousReadHandler
			StandardOutputReader,
			StandardErrorReader;

		Nil.Serialiser<Configuration> ConfigurationSerialiser;
		Configuration ProgramConfiguration;
		bool GotConfiguration;

		private const char VariableSeparator = '$';
		private const char ColourIdentifier = '#';

		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.ConsoleHandler = this;
			this.MainForm = consoleForm;
			Process = null;
			Terminating = false;
			Writable = false;

			ConfigurationSerialiser = new Nil.Serialiser<Configuration>(Configuration.ConfigurationFile);

			LoadConfiguration();
			InitialiseVariableDictionary();

			CommandHandlerDictionary = new Dictionary<string, CommandHandler>();
			AddCommand("cd", "<directory>", "change the working directory", this.ChangeDirectory, 1);
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

		void UpdateWorkingDirectory()
		{
			CurrentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
			VariableDictionary["CurrentWorkingDirectory"] = CurrentWorkingDirectory;
		}

		void LoadConfiguration()
		{
			try
			{
				ProgramConfiguration = ConfigurationSerialiser.Load();
				MainForm.InputBox.ForeColor = ProgramConfiguration.CommandTextInputColour.ToColour();
				MainForm.InputBox.BackColor = ProgramConfiguration.InputFieldBackgroundColour.ToColour();
				MainForm.ConsoleBox.ForeColor = ProgramConfiguration.DefaultOutputColour.ToColour();
				MainForm.BackColor = ProgramConfiguration.BackgroundColour.ToColour();
				MainForm.ConsoleBox.BackColor = ProgramConfiguration.BackgroundColour.ToColour();

				Font MainFont = new Font(ProgramConfiguration.Font, ProgramConfiguration.FontSize);
				MainForm.InputBox.Font = MainFont;
				MainForm.ConsoleBox.Font = MainFont;

				GotConfiguration = true;
			}
			catch (FileNotFoundException)
			{
				ProgramConfiguration = new Configuration();
				GotConfiguration = false;
			}
		}

		void SaveConfiguration()
		{
			ProgramConfiguration.FormState.Load(MainForm);
			ConfigurationSerialiser.Store(ProgramConfiguration);
		}

		public void FormLoaded()
		{
			if (GotConfiguration)
				ProgramConfiguration.FormState.Apply(MainForm);
			PrintPrompt();
		}

		void AddCommand(string command, string argumentDescription, string description, CommandHandlerFunction function, int argumentCount)
		{
			CommandHandler handler = new CommandHandler(command, argumentDescription, description, function, argumentCount);
			CommandHandlerDictionary.Add(command, handler);
		}

		void PrintPrompt()
		{
			UpdateWorkingDirectory();
			FormattedPrinting(ProgramConfiguration.Prompt);
		}

		const int WM_VSCROLL = 0x115;
		const int SB_BOTTOM = 7;

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

		void Print(string text)
		{
			MainForm.ConsoleBox.Invoke
			(
				(MethodInvoker) delegate
				{
					MainForm.ConsoleBox.AppendText(text);
					SendMessage(MainForm.ConsoleBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
				}
			);
		}

		void PrintLine(string line)
		{
			Print(line + "\n");
		}

		const int HexColoursPerCode = 4;
		const int HexColourSize = 2;
		const int HexCodeTotalSize = HexColoursPerCode * HexColourSize;

		void PrintError(string line)
		{
			SetOutputColour(ProgramConfiguration.ErrorColour.ToColour());
			PrintLine(line);
			SetOutputColour(ProgramConfiguration.DefaultOutputColour.ToColour());
		}

		Color ConvertHexStringToColour(string input)
		{
			List<int> colours = new List<int>();
			for (int i = 0; i < HexCodeTotalSize; i += HexColourSize)
			{
				string currentCode = input.Substring(i, HexColourSize);
				int value = Convert.ToInt32(currentCode, 16);
				colours.Add(value);
			}
			Color output = Color.FromArgb(colours[0], colours[1], colours[2], colours[3]);
			return output;
		}

		void FormattedPrinting(string input, bool useVariables = true, bool useColours = true)
		{
			string currentOutput = "";

			for (int i = 0; i < input.Length; )
			{
				char currentChar = input[i];
				if (useColours && currentChar == ColourIdentifier)
				{
					int offset = i + 1;
					if (offset + HexCodeTotalSize >= input.Length)
					{
						//actually this is an error since the user specified an incomplete colour code, but just keep on printing
						currentOutput += currentChar;
					}
					else
					{
						Print(currentOutput);
						currentOutput = "";
						string hexString = input.Substring(offset, HexCodeTotalSize);
						i = offset + HexCodeTotalSize;
						try
						{
							Color colour = ConvertHexStringToColour(hexString);
							SetOutputColour(colour);
						}
						catch (FormatException)
						{
							//error, the user has specified an invalid hex string, let's just print some garbage instead
							currentOutput += ColourIdentifier + "InvalidColour";
						}
						continue;
					}
				}
				else if (useVariables && currentChar == VariableSeparator)
				{
					int start = i + 1;
					int offset = input.IndexOf(VariableSeparator, start);
					if (offset == -1)
					{
						//error: the user failed to specify a terminating separator, just print it anyways
						currentOutput += currentChar;
					}
					else
					{
						string variable = input.Substring(start, offset - start);
						i = offset + 1;
						if (variable.Length == 0)
						{
							//it's a double separator - print the separator instead
							currentOutput += VariableSeparator;
							continue;
						}
						if (VariableDictionary.ContainsKey(variable))
							currentOutput += VariableDictionary[variable];
						else
							//error: the user has specified an invalid variable name, just append a warning string instead
							currentOutput += "Unknown";
						continue;
					}
				}
				else
					currentOutput += currentChar;
				i++;
			}
			Print(currentOutput);
		}

		void VisualiseArguments(CommandArguments arguments)
		{
			PrintLine("Command: \"" + arguments.Command + "\"");
			string output = "Arguments:";
			for (int i = 0; i < arguments.Arguments.Length; i++)
			{
				string argument = arguments.Arguments[i];
				output += " " + (i + 1).ToString() + ". \"" + argument + "\"";
			}
			PrintLine(output);
		}

		void ChangeDirectory(string[] arguments)
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

		void PrintBuffer(byte[] buffer, int bytesRead)
		{
			string data = System.Text.UTF8Encoding.UTF8.GetString(buffer, 0, bytesRead);
			Print(data);
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

		public void Enter()
		{
			lock (this)
			{
				if (Process == null)
					ProcessRegularEnter();
				else
					ProcessRuntimeEnter();
			}
		}

		void ProcessTerminationHandler()
		{
			try
			{
				Process.WaitForExit();
			}
			catch (NullReferenceException)
			{
			}
			lock (this)
			{
				Process = null;
				Writable = false;
				if (!Terminating)
					PromptAndSelect();
			}
		}

		void SetOutputColour(Color colour)
		{
			MainForm.InputBox.Invoke
			(
				(MethodInvoker)delegate
				{
					MainForm.ConsoleBox.SelectionColor = colour;
				}
			);
		}

		void PrintCommandString(string line)
		{
			int spaceOffset = line.IndexOf(' ');
			if (spaceOffset == -1)
				spaceOffset = line.Length;
			string command = line.Substring(0, spaceOffset);
			string argumentString = line.Substring(spaceOffset);
			SetOutputColour(ProgramConfiguration.CommandTextOutputHighlightColour.ToColour());
			Print(command);
			SetOutputColour(ProgramConfiguration.CommandTextOutputColour.ToColour());
			PrintLine(argumentString);
			SetOutputColour(ProgramConfiguration.DefaultOutputColour.ToColour());
		}

		void ProcessRegularEnter()
		{
			string line = MainForm.InputBox.Text;
			PrintCommandString(line);
			line = line.Trim();
			if (line.Length == 0)
			{
				PromptAndSelect();
				return;
			}

			CommandArguments arguments;

			try
			{
				arguments = new CommandArguments(line);
				//visualiseArguments(arguments);
			}
			catch (ArgumentException exception)
			{
				PrintLine(exception.Message);
				PromptAndSelect();
				return;
			}

			if (CommandHandlerDictionary.ContainsKey(arguments.Command))
			{
				CommandHandler handler = CommandHandlerDictionary[arguments.Command];
				if (arguments.Arguments.Length != handler.ArgumentCount)
				{
					PrintError("Invalid argument count.");
					PrintLine(handler.Usage());
				}
				else
					handler.Function(arguments.Arguments);
				PromptAndSelect();
			}
			else
			{
				try
				{
					Writable = false;

					//check for executable programs matching that name
					Process = new Process();

					ProcessStartInfo info = Process.StartInfo;
					info.UseShellExecute = false;
					info.RedirectStandardInput = true;
					info.RedirectStandardOutput = true;
					info.RedirectStandardError = true;
					info.FileName = arguments.Command;
					info.Arguments = arguments.GetQuotedArguments();
					info.WindowStyle = ProcessWindowStyle.Hidden;
					info.CreateNoWindow = true;

					Process.Start();

					StandardOutputReader = new AsynchronousReadHandler(this, HandleStandardOutputRead, Process.StandardOutput);
					StandardErrorReader = new AsynchronousReadHandler(this, HandleStandardErrorRead, Process.StandardError);

					new Thread(ProcessTerminationHandler).Start();

					Writable = true;
				}
				catch (System.ComponentModel.Win32Exception exception)
				{
					Process = null;
					Writable = false;
					PrintError(exception.Message);
					PromptAndSelect();
				}
			}
		}

		void ProcessRuntimeEnter()
		{
			if (Writable)
			{
				string line = MainForm.InputBox.Text;
				PrintLine(line);
				Process.StandardInput.WriteLine(line);
			}
		}

		public void Closing()
		{
			lock (this)
			{
				Terminating = true;
				SaveConfiguration();
				KillProcess();
			}
		}

		void KillProcess()
		{
			if (Process != null)
				Process.Kill();
		}

		void Escape()
		{
			lock (this)
			{
				PrintError("Process has been terminated");
				KillProcess();
			}
		}

		public void KeyPressed(KeyPressEventArgs keyEvent)
		{
			switch(keyEvent.KeyChar)
			{
				case '\r':
					//suppress beep
					keyEvent.Handled = true;
					Enter();
					break;

				case '\x1b':
					keyEvent.Handled = true;
					Escape();
					break;
			}
		}
	}
}
