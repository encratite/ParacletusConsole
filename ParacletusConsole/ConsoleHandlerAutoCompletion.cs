using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		void Tab()
		{
			lock (this)
			{
				CloseAutoCompletionForm();

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
					Beep();
					return;
				}
				string argumentString = activeArgument.Argument;

				//fix for weird tabbing behaviour on Windows for strings like "c:"
				if (IsWindows && argumentString.Length == 2 && Char.IsLetter(argumentString[0]) && argumentString[1] == ':')
					argumentString += Path.DirectorySeparatorChar;

				HashSet<string> autoCompletionStrings = new HashSet<string>();
				if (System.Object.ReferenceEquals(activeArgument, arguments.Command))
				{
					//the user is performing the tab within the first unit of the input - that is the command unit
					foreach (string i in PathNames)
						autoCompletionStrings.Add(i);
				}
				else
				{
					//the user is performing the tab within the boundaries of one of the argument units and not the command unit
					List<string> currentDirectory = LoadDirectoryContentsForAPathToAFile(argumentString);
					foreach (string i in currentDirectory)
						autoCompletionStrings.Add(i);
				}

				if (Nil.File.GetFileType(argumentString) == Nil.File.FileType.Directory)
				{
					//the current argument the user is tabbing in refers to a directory
					List<string> directoryContent = LoadDirectoryContent(argumentString);
					foreach (string i in directoryContent)
						autoCompletionStrings.Add(i);
				}
				else
				{
					//the tabbed argument either refers to a file or is simply incomplete and refers to neither a file nor a directory
					//just add the directory it currently refers to then
					List<string> directoryContent = LoadDirectoryContentsForAPathToAFile(argumentString);
					foreach (string i in directoryContent)
						autoCompletionStrings.Add(i);
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

				if (filteredAutoCompletionStrings.Count == 0)
				{
					//no matches could be found
					Beep();
					return;
				}

				if (filteredAutoCompletionStrings.Count > 1)
					ShowAutoCompletionForm(filteredAutoCompletionStrings);

				string longestCommonSubstring = GetLongestCommonSubstring(filteredAutoCompletionStrings, argumentString.Length);
				if (longestCommonSubstring == argumentString)
				{
					//no better match could be found, play a beep
					Beep();
					return;
				}

				PerformInputBoxReplacement(line, longestCommonSubstring, activeArgument);
			}
		}

		void PerformInputBoxReplacement(string line, string replacement, ArgumentResult activeArgument)
		{
			//extend the argument accordingly
			ArgumentResult modifiedArgument = new ArgumentResult(replacement);
			string middle = modifiedArgument.EscapeArgument();
			string left = line.Substring(0, activeArgument.Offset);
			int rightOffset = activeArgument.Offset + activeArgument.Length();
			string right = line.Substring(rightOffset);
			string newLine = left + middle + right;
			int newCursorOffset = activeArgument.Offset + replacement.Length;

			//need to fix the cursor position, it should be at the end of the current argument
			MainForm.InputBox.Text = newLine;
			MainForm.InputBox.SelectionStart = newCursorOffset;
		}

		bool PerformCommonSubstringCheck(List<string> input, string sourceString, int offset)
		{
			for (int i = 1; i < input.Count; i++)
			{
				string currentString = input[i];
				if (offset >= currentString.Length)
					return false;
				if (!CaseInsensitiveCharacterComparison(sourceString[offset], currentString[offset]))
					return false;
			}
			return true;
		}

		string GetLongestCommonSubstring(List<string> input, int initialOffset)
		{
			int offset = initialOffset;
			string sourceString = input.First();
			while (true)
			{
				if (offset >= sourceString.Length)
					break;
				if (!PerformCommonSubstringCheck(input, sourceString, offset))
					break;
				offset++;
			}
			return sourceString.Substring(0, offset);
		}
	}
}
