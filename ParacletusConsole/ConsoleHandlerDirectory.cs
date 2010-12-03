using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
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
			try
			{
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
			}
			catch (DirectoryNotFoundException)
			{
			}
			return output;
		}

		void LoadPathDirectory(HashSet<string> pathStrings, string path)
		{
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

		HashSet<string> LoadPathNames()
		{
			HashSet<string> pathStrings = new HashSet<string>();
			string pathString = Environment.GetEnvironmentVariable("PATH");
			char separator;
			const char windowsPathSeparator = ';';
			if (pathString.IndexOf(windowsPathSeparator) != -1)
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
				directoryPath = path.Substring(0, offset + 1);
				removePrefix = false;
			}
			return LoadDirectoryContent(directoryPath, removePrefix);
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
			catch (DirectoryNotFoundException)
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
	}
}
