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
using System.Media;

namespace ParacletusConsole
{
	public class ConsoleHandler
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

		List<string> PathNames;

		bool IsWindows;

		Thread AutoCompletionThread;
		AutoCompletionForm AutoCompletionMatchesForm;

		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.ConsoleHandler = this;
			this.MainForm = consoleForm;
			Process = null;
			Terminating = false;
			ProcessIOActive = false;

			ConfigurationSerialiser = new Nil.Serialiser<Configuration>(Configuration.ConfigurationFile);

			AutoCompletionMatchesForm = new AutoCompletionForm(this);

			LoadConfiguration();
			InitialiseVariableDictionary();
			InitialiseKeyPressHandlerDictionary();

			IsWindows = IsWindowsOS();
			PathNames = LoadPathNames();

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

		void UpdateWorkingDirectory()
		{
			CurrentWorkingDirectory = System.IO.Directory.GetCurrentDirectory();
			VariableDictionary["CurrentWorkingDirectory"] = CurrentWorkingDirectory;
			string[] tokens = CurrentWorkingDirectory.Split(Path.DirectorySeparatorChar);
			string folder = tokens[tokens.Length - 1];
			VariableDictionary["CurrentWorkingDirectoryWithoutPath"] = folder;
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

				AutoCompletionMatchesForm.AutoCompletionListBox.ForeColor = ProgramConfiguration.DefaultOutputColour.ToColour();
				AutoCompletionMatchesForm.AutoCompletionListBox.BackColor = ProgramConfiguration.BackgroundColour.ToColour();
				AutoCompletionMatchesForm.AutoCompletionListBox.Font = MainFont;

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

		void CloseAutoCompletionForm()
		{
			if (AutoCompletionThread != null)
			{
				AutoCompletionMatchesForm.Invoke(
					(MethodInvoker)delegate
					{
						AutoCompletionMatchesForm.AutoCompletionListBox.Items.Clear();
						AutoCompletionMatchesForm.Close();
					}
				);
				AutoCompletionThread.Join();
				AutoCompletionThread = null;
			}
		}

		void ShowAutoCompletionForm(List<string> autoCompletionStrings)
		{
			AutoCompletionThread = new Thread(() =>
			{
				AutoCompletionMatchesForm.AutoCompletionListBox.Items.AddRange(autoCompletionStrings.ToArray());
				AutoCompletionMatchesForm.ShowDialog();
			}
			);
			AutoCompletionThread.Start();
		}

		public void UpdateAutoCompletionFormPosition()
		{
			AutoCompletionMatchesForm.Invoke(
				(MethodInvoker)delegate
				{
					AutoCompletionMatchesForm.Left = MainForm.Left + MainForm.InputBox.Left + 16;
					AutoCompletionMatchesForm.Top = MainForm.Top + MainForm.InputBox.Top - AutoCompletionMatchesForm.Height;
				}
			);
		}

		public void OnAutoCompletionFormLoad()
		{
			UpdateAutoCompletionFormPosition();
			AutoCompletionMatchesForm.TopMost = true;
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
				string argument = arguments.Arguments[i].Argument;
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
				CloseAutoCompletionForm();

				if (Process == null)
					ProcessRegularEnter();
				else
					ProcessRuntimeEnter();
			}
		}

		void ProcessTermination()
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

		bool IsDirectory(string path)
		{
			try
			{
				FileAttributes attributes = File.GetAttributes(path);
				return (attributes & FileAttributes.Directory) != 0;
			}
			catch (FileNotFoundException)
			{
			}
			return false;
		}

		bool PerformChangeDirectoryCheck(string path)
		{
			if (IsDirectory(path))
			{
				//this is a directory, it cannot be executed, let's just cd instead
				System.IO.Directory.SetCurrentDirectory(path);
				PromptAndSelect();
				return true;
			}
			return false;
		}

		void AttemptProcessExecution(CommandArguments arguments)
		{
			try
			{
				ProcessIOActive = false;

				//check for executable programs matching that name
				Process = new Process();

				ProcessStartInfo info = Process.StartInfo;
				info.UseShellExecute = false;
				info.RedirectStandardInput = true;
				info.RedirectStandardOutput = true;
				info.RedirectStandardError = true;
				info.FileName = arguments.Command.Argument;
				info.Arguments = arguments.GetQuotedArguments();
				info.WindowStyle = ProcessWindowStyle.Hidden;
				info.CreateNoWindow = true;

				Process.Start();

				StandardOutputReader = new AsynchronousReader(this, HandleStandardOutputRead, Process.StandardOutput);
				StandardErrorReader = new AsynchronousReader(this, HandleStandardErrorRead, Process.StandardError);

				new Thread(ProcessTermination).Start();

				ProcessIOActive = true;
			}
			catch (System.ComponentModel.Win32Exception exception)
			{
				Process = null;
				ProcessIOActive = false;
				PrintError(exception.Message);
				PromptAndSelect();
			}
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
				PrintError(exception.Message);
				PromptAndSelect();
				return;
			}

			if (CommandHandlerDictionary.ContainsKey(arguments.Command.Argument))
			{
				CommandHandler handler = CommandHandlerDictionary[arguments.Command.Argument];
				if (arguments.Arguments.Length != handler.ArgumentCount)
				{
					PrintError("Invalid argument count.");
					PrintLine(handler.Usage());
				}
				else
					handler.Function(arguments.GetArgumentString());
				PromptAndSelect();
			}
			else
			{
				if (PerformChangeDirectoryCheck(arguments.Command.Argument))
					return;

				AttemptProcessExecution(arguments);
			}
		}

		void ProcessRuntimeEnter()
		{
			if (ProcessIOActive)
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
				CloseAutoCompletionForm();

				if(KillProcess())
					PrintError("Process has been terminated");
			}
		}

		string ProcessDirectoryContentString(string input, bool removePrefix)
		{
			if (removePrefix)
				//get rid of the leading "." + path separator
				return input.Substring(2);
			return input;
		}

		List<string> LoadDirectoryContent(string path, bool removePrefix = false)
		{
			List<string> output = new List<string>();
			DirectoryInfo selectedDirectory = new DirectoryInfo(path);
			DirectoryInfo[] directories = selectedDirectory.GetDirectories();
			foreach (DirectoryInfo currentDirectory in directories)
			{
				string directoryPath = ProcessDirectoryContentString(Path.Combine(path, currentDirectory.Name), removePrefix);
				output.Add(directoryPath);
			}
			FileInfo[] files = selectedDirectory.GetFiles();
			foreach (FileInfo currentFile in files)
			{
				string filePath = ProcessDirectoryContentString(Path.Combine(path, currentFile.Name), removePrefix);
				output.Add(filePath);
			}
			return output;
		}

		bool IsWindowsOS()
		{
			string osString = System.Environment.OSVersion.ToString();
			//Console.WriteLine("OS: " + osString);
			return osString.IndexOf("Windows") != -1;
		}

		void LoadPathDirectory(List<string> pathStrings, string path)
		{
			//Console.WriteLine("Loading PATH: " + path);
			try
			{
				DirectoryInfo directory = new DirectoryInfo(path);
				//on Windows one could use PATHEXT extensions instead of just the exe extension at this point but I didn't really feel it was worth it
				//regular batch files don't work with this program anyways
				string extension;
				if (IsWindows)
					extension = ".exe";
				else
					extension = "";
				FileInfo[] executables = directory.GetFiles("*" + extension);
				foreach (FileInfo executable in executables)
				{
					//add the name without the extension
					string name = executable.Name;
					//Console.WriteLine("Discovered a binary: " + name);
					//string processedName = name.Substring(0, name.Length - extension.Length);
					//let's just add the full names for now, for great justice!
					string processedName = name.Substring(0, name.Length);
					pathStrings.Add(processedName);
				}
			}
			catch (Exception)
			{
				//just ignore invalid PATHs for now although we could do the user a favour and inform them about invalid stuff in their PATH
			}
		}

		List<string> LoadPathNames()
		{
			List<string> pathStrings = new List<string>();
			string pathString = Environment.GetEnvironmentVariable("PATH");
			char separator;
			const char windowsPathSeparator = ';';
			if(pathString.IndexOf(windowsPathSeparator) != -1)
				separator = windowsPathSeparator;
			else
				//for UNIX likes
				separator = ':';
			string[] tokens = pathString.Split(separator);
			foreach (string path in tokens)
				LoadPathDirectory(pathStrings, path);
			if (IsWindows)
			{
				//Windows actually appears to have more paths it checks than just the ones in PATH
				//the root and system32/SysWoW64 are not part of it for some reason
				string root = Environment.GetEnvironmentVariable("SystemRoot");
				string systemPath = Path.Combine(root, "system32");
				string[] exceptions = { root, systemPath };
				foreach (string path in exceptions)
					LoadPathDirectory(pathStrings, path);
			}
			return pathStrings;
		}

		bool CaseInsensitiveCharacterComparison(char left, char right)
		{
			return Char.ToLower(left) == Char.ToLower(right);
		}

		bool CaseInsensitiveStringComparison(string left, string right)
		{
			return left.ToLower() == right.ToLower();
		}

		List<string> LoadDirectoryContentsForAPathToAFile(string path)
		{
			int offset = path.LastIndexOf(Path.DirectorySeparatorChar);
			string directoryPath;
			bool removePrefix;
			if (offset == -1)
			{
				directoryPath = ".";
				removePrefix = true;
			}
			else
			{
				directoryPath = path.Substring(0, offset);
				removePrefix = false;
			}
			return LoadDirectoryContent(directoryPath, removePrefix);
		}

		void Tab()
		{
			lock (this)
			{
				CloseAutoCompletionForm();

				Console.WriteLine("Tab was pressed");
				int offset = MainForm.InputBox.SelectionStart;
				string line = MainForm.InputBox.Text;
				CommandArguments arguments;
				try
				{
					arguments = new CommandArguments(line);
				}
				catch (ArgumentException)
				{
					//there was a missing quote - this could possibly be handed by automatically attaching another quote at the end
					Beep();
					return;
				}
				ArgumentResult activeArgument;
				try
				{
					activeArgument = arguments.FindMatchingResult(offset);
				}
				catch (ArgumentException)
				{
					//the cursor of the user was not within the boundaries of any argument within the line
					Console.WriteLine("Cursor not within boundaries");
					Beep();
					return;
				}
				string argumentString = activeArgument.Argument;
				List<string> autoCompletionStrings = new List<string>();
				if (System.Object.ReferenceEquals(activeArgument, arguments.Command))
				{
					//the user is performing the tab within the first unit of the input - that is the command unit
					Console.WriteLine("Command tab detected");
					autoCompletionStrings.AddRange(PathNames);
				}
				else
				{
					//the user is performing the tab within the boundaries of one of the argument units and not the command unit
					Console.WriteLine("Argument tab detected");
					List<string> currentDirectory = LoadDirectoryContentsForAPathToAFile(argumentString);
					autoCompletionStrings.AddRange(currentDirectory);
				}

				Console.WriteLine("Number of auto completion strings: " + autoCompletionStrings.Count);

				if (IsDirectory(argumentString))
				{
					//the current argument the user is tabbing in refers to a directory
					List<string> directoryContent = LoadDirectoryContent(argumentString);
					autoCompletionStrings.AddRange(directoryContent);
				}
				else
				{
					//the tabbed argument either refers to a file or is simply incomplete and refers to neither a file nor a directory
					//just add the directory it currently refers to then
					List<string> directoryContent = LoadDirectoryContentsForAPathToAFile(argumentString);
					autoCompletionStrings.AddRange(directoryContent);
				}

				//filter out the strings which do not match the tabbed argument
				List<string> filteredAutoCompletionStrings = new List<string>();
				foreach (string target in autoCompletionStrings)
				{
					if
					(
						target.Length >= argumentString.Length &&
						CaseInsensitiveStringComparison(argumentString, target.Substring(0, argumentString.Length))
					)
						filteredAutoCompletionStrings.Add(target);
				}

				Console.WriteLine("Count after filtering: " + filteredAutoCompletionStrings.Count);

				if (filteredAutoCompletionStrings.Count == 0)
				{
					//no matches could be found
					Console.WriteLine("No matches could be found");
					Beep();
					return;
				}

				ShowAutoCompletionForm(filteredAutoCompletionStrings);

				string longestCommonSubstring = GetLongestCommonSubstring(filteredAutoCompletionStrings, argumentString.Length);
				Console.WriteLine("LCS: " + longestCommonSubstring);
				if (longestCommonSubstring == argumentString)
				{
					//no better match could be found, play a beep
					Console.WriteLine("Unable to find a better match");
					Beep();
					return;
				}

				//extend the argument accordingly
				ArgumentResult modifiedArgument = new ArgumentResult(longestCommonSubstring);
				string replacement = modifiedArgument.EscapeArgument();
				string left = line.Substring(0, activeArgument.Offset);
				int rightOffset = activeArgument.Offset + activeArgument.Length();
				string right = line.Substring(rightOffset);
				string newLine = left + replacement + right;
				int newCursorOffset = activeArgument.Offset + replacement.Length;

				//need to fix the cursor position, it should be at the end of the current argument
				MainForm.InputBox.Text = newLine;
				MainForm.InputBox.SelectionStart = newCursorOffset;
			}
		}

		bool PerformCommonSubstringCheck(List<string> input, string sourceString, int offset)
		{
			for (int i = 1; i < input.Count; i++)
			{
				string currentString = input[i];
				if (offset >= currentString.Length)
				{
					Console.WriteLine("Offset " + offset + " exceeds the length of the string " + currentString);
					return false;
				}
				if (!CaseInsensitiveCharacterComparison(sourceString[offset], currentString[offset]))
				{
					Console.WriteLine("String mismatch: " + sourceString + " vs. " + currentString);
					return false;
				}
			}
			return true;
		}

		string GetLongestCommonSubstring(List<string> input, int initialOffset)
		{
			int offset = initialOffset;
			string sourceString = input.First();
			Console.WriteLine("Source string: " + sourceString);
			while (true)
			{
				if (offset >= sourceString.Length)
				{
					Console.WriteLine("Offset " + offset + " exceeds the length of the source string");
					break;
				}
				if (!PerformCommonSubstringCheck(input, sourceString, offset))
				{
					Console.WriteLine("Common substring check failed at offset " + offset);
					break;
				}
				offset++;
			}
			return sourceString.Substring(0, offset);
		}

		void Beep()
		{
			SystemSounds.Beep.Play();
		}

		public void KeyPressed(KeyPressEventArgs keyEvent)
		{
			if (KeyPressHandlerDictionary.ContainsKey(keyEvent.KeyChar))
			{
				keyEvent.Handled = true;
				KeyPressHandlerDictionary[keyEvent.KeyChar]();
			}
		}
	}
}
