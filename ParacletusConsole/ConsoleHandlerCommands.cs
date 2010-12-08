using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

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

		public void PrintHelp(string[] arguments)
		{
			CommandHandler[] handlers = CommandHandlerDictionary.Values.ToArray();
			Array.Sort(handlers);
			PrintLineWithColour("Help menu:\n", ProgramConfiguration.TitleColour);
			foreach (CommandHandler handler in handlers)
			{
				PrintWithColour("  " + handler.Command, ProgramConfiguration.HighlightColour);
				string argumentDescriptionString = "";
				if (handler.ArgumentDescription != null)
					argumentDescriptionString = " " + handler.ArgumentDescription;
				PrintLine(argumentDescriptionString + " - " + handler.Description);
			}
		}

		public void ClearConsole(string[] arguments)
		{
			MainForm.ConsoleBox.Text = "";
		}

		public void PrintFile(string[] arguments)
		{
			string path = arguments.First();
			try
			{
				StreamReader reader = new StreamReader(path);
				string content = reader.ReadToEnd();
				reader.Close();
				PrintLine(content);
			}
			catch (Exception exception)
			{
				PrintError(exception.Message);
			}
		}

		void CopyDirectory(string source, string destination)
		{
			DirectoryInfo sourceDirectory = new DirectoryInfo(source);
			DirectoryInfo[] subDirectories = sourceDirectory.GetDirectories();
			foreach(DirectoryInfo directory in subDirectories)
			{
				string path = Path.Combine(destination, directory.Name);
				Directory.CreateDirectory(path);
				CopyDirectory(directory.FullName, path);
			}
			FileInfo[] files = sourceDirectory.GetFiles();
			foreach (FileInfo file in files)
			{
				string path = Path.Combine(destination, file.Name);
				file.CopyTo(path);
			}
		}

		void PrepareDirectoryCopy(string source, string destination)
		{
			Nil.File.FileType destinationType = Nil.File.GetFileType(destination);
			switch (destinationType)
			{
				case Nil.File.FileType.File:
					PrintError("The destination " + destination + " is a file");
					break;

				case Nil.File.FileType.Directory:
					CopyDirectory(source, destination);
					break;

				case Nil.File.FileType.DoesNotExist:
					Directory.CreateDirectory(destination);
					CopyDirectory(source, destination);
					break;
			}
		}

		public void CopyFile(string[] arguments)
		{
			string source = arguments[0];
			string destination = arguments[1];
			try
			{
				Nil.File.FileType sourceType = Nil.File.GetFileType(source);
				switch (sourceType)
				{
					case Nil.File.FileType.File:
						File.Copy(source, destination);
						break;

					case Nil.File.FileType.Directory:
						PrepareDirectoryCopy(source, destination);
						break;

					case Nil.File.FileType.DoesNotExist:
						PrintError("Source " + source + " does not exist");
						break;
				}
			}
			catch (Exception exception)
			{
				PrintError(exception.Message);
			}
		}

		public void Echo(string[] arguments)
		{
			string line = "";
			bool first = true;
			foreach (string argument in arguments)
			{
				if (first)
					first = false;
				else
					line += " ";
				line += argument;
			}
			line += "\n";
			FormattedPrinting(line);
		}

		int ProcessComparison(Process x, Process y)
		{
			return x.ProcessName.ToLower().CompareTo(y.ProcessName.ToLower());
		}

		public void ProcessList(string[] arguments)
		{
			PrintLineWithColour("Running processes:\n", ProgramConfiguration.TitleColour);
			Process[] processes = Process.GetProcesses();
			Array.Sort(processes, ProcessComparison);
			foreach (Process process in processes)
			{
				PrintWithColour(process.ProcessName, ProgramConfiguration.HighlightColour);
				try
				{
					Print(" (PID " + process.Id.ToString());
					Print(", " + process.MainModule.FileName);
				}
				catch (Exception)
				{
				}
				PrintLine(")");
			}
		}

		void TerminateMatchingProcess(Process process)
		{
			try
			{
				process.Kill();
				PrintLine("Successfully terminated process " + process.ProcessName + " (PID " + process.Id.ToString() + ")");
			}
			catch (Exception exception)
			{
				PrintError(exception.Message);
			}
		}

		public void KillProcess(string[] arguments)
		{
			string target = arguments.First();
			Process[] processes = Process.GetProcesses();
			try
			{
				int pid = Convert.ToInt32(target);
				foreach (Process process in processes)
				{
					if (process.Id == pid)
					{
						TerminateMatchingProcess(process);
						return;
					}
				}
				PrintError("Unable to find a process with the PID " + pid.ToString());
				return;
			}
			catch (FormatException)
			{
			}
			catch (OverflowException)
			{
				PrintError("An overflow exception occured, you specified an invalid PID to kill.");
				return;
			}
			foreach (Process process in processes)
			{
				bool doKill = process.ProcessName.ToLower() == target.ToLower();
				try
				{
					string[] tokens = process.MainModule.FileName.Split(Path.DirectorySeparatorChar);
					doKill |= target.ToLower() == tokens.Last().ToLower();
				}
				catch (Exception)
				{
				}
				if (doKill)
					TerminateMatchingProcess(process);
			}
			PrintError("Unable to find a process with the name " + target);
		}

		public void ListDirectoryContents(string[] arguments)
		{
			string path;
			if (arguments.Length == 1)
				path = arguments.First();
			else
				path = ".";
			try
			{
				List<ColouredString[]> rows = new List<ColouredString[]>();
				DirectoryInfo directory = new DirectoryInfo(path);
				foreach (DirectoryInfo subDirectory in directory.GetDirectories())
				{
					List<ColouredString> columns = new List<ColouredString>();
					ColouredString
						name = new ColouredString(subDirectory.Name, ProgramConfiguration.ListDirectoryColour),
						time = new ColouredString(subDirectory.LastWriteTime.ToString()),
						size = new ColouredString("");
					columns.Add(name);
					columns.Add(time);
					columns.Add(size);
					rows.Add(columns.ToArray());
				}
				foreach (FileInfo file in directory.GetFiles())
				{
					const string executableExtension = ".exe";
					string name = file.Name;
					SerialisableColour currentColour;
					if(name.Length >= executableExtension.Length && name.Substring(name.Length - executableExtension.Length) == executableExtension)
						currentColour = ProgramConfiguration.ListExecutableColour;
					else
						currentColour = ProgramConfiguration.ListFileColour;
					List<ColouredString> columns = new List<ColouredString>();
					ColouredString
						nameColumn = new ColouredString(name, currentColour),
						time = new ColouredString(file.LastWriteTime.ToString()),
						size = new ColouredString(Nil.String.GetFileSizeString(file.Length));
					columns.Add(nameColumn);
					columns.Add(time);
					columns.Add(size);
					rows.Add(columns.ToArray());
				}
				PrintTable(rows.ToArray());
			}
			catch (Exception exception)
			{
				PrintError(exception.Message);
			}
		}

		public void CreateDirectory(string[] arguments)
		{
			string path = arguments.First();
			try
			{
				Directory.CreateDirectory(path);
			}
			catch (Exception exception)
			{
				PrintError(exception.Message);
			}
		}

		public void MoveFile(string[] arguments)
		{
			string source = arguments[0];
			string destination= arguments[1];
			try
			{
				Nil.File.FileType sourceType = Nil.File.GetFileType(source);
				switch (sourceType)
				{
					case Nil.File.FileType.File:
						File.Move(source, destination);
						break;

					case Nil.File.FileType.Directory:
						Directory.Move(source, destination);
						break;

					case Nil.File.FileType.DoesNotExist:
						PrintError("Source " + source + " does not exist");
						break;
				}
			}
			catch (Exception exception)
			{
				PrintError(exception.Message);
			}
		}

		public void PrintWorkingDirectory(string[] arguments)
		{
			PrintLine(System.IO.Directory.GetCurrentDirectory());
		}

		void RemoveDirectory(string path)
		{
			DirectoryInfo directory = new DirectoryInfo(path);
			foreach(FileInfo file in directory.GetFiles())
				file.Delete();
			foreach (DirectoryInfo subDirectory in directory.GetDirectories())
				RemoveDirectory(subDirectory.FullName);
			directory.Delete();
		}

		void ProcessRemoveFileCommand(string path)
		{
			Nil.File.FileType sourceType = Nil.File.GetFileType(path);
			switch (sourceType)
			{
				case Nil.File.FileType.File:
					File.Delete(path);
					break;

				case Nil.File.FileType.Directory:
					RemoveDirectory(path);
					break;

				case Nil.File.FileType.DoesNotExist:
					PrintError("File " + path + " does not exist");
					break;
			}
		}

		public void RemoveFile(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				PrintError("You have not specified a file to remove.");
				return;
			}

			foreach (string path in arguments)
				ProcessRemoveFileCommand(path);
		}

		public void OperatingSystemInformation(string[] arguments)
		{
			string servicePackString = System.Environment.OSVersion.ServicePack;
			if (servicePackString.Length == 0)
				servicePackString = "None";
			string[][] stringRows =
			{
				new string[] {"Name", Nil.OperatingSystem.Name()},
				new string[] {"Architecture", System.Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit"},
				new string[] {"Version", System.Environment.OSVersion.Version.ToString()},
				//new string[] {"Platform", System.Environment.OSVersion.Platform.ToString()},
				new string[] {"Service pack", servicePackString},
			};
			List<ColouredString[]> rows = new List<ColouredString[]>();
			foreach (string[] stringRow in stringRows)
			{
				ColouredString[] row = { new ColouredString(stringRow[0] + ":", ProgramConfiguration.HighlightColour), new ColouredString(stringRow[1]) };
				rows.Add(row);
			}
			PrintTable(rows.ToArray());
		}

		public void DefineAlias(string[] arguments)
		{
			if (arguments.Length == 0)
			{
				PrintError("You have not provided an alias.");
				return;
			}
			string identifier = arguments[0];
			List<string> commandStrings = new List<string>(arguments);
			commandStrings = commandStrings.GetRange(1, commandStrings.Count - 1);
			commandStrings = commandStrings.ConvertAll(
				delegate(string x)
				{
					return CommandArgument.EscapeArgument(x);
				}
			);
			string command = String.Join(" ", commandStrings.ToArray());
			ProgramConfiguration.Aliases[identifier] = command;
		}

		public void RemoveAlias(string[] arguments)
		{
			string identifier = arguments[0];
			if (ProgramConfiguration.Aliases.ContainsKey(identifier))
				ProgramConfiguration.Aliases.Remove(identifier);
			else
				PrintError("Unable to find alias " + identifier);
		}
	}
}
