using System;
using System.Collections;
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

	}
}
