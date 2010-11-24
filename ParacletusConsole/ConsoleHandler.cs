using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class ConsoleHandler
	{
		ConsoleForm consoleForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> commandHandlerDictionary;

		public ConsoleHandler(ConsoleForm newConsoleForm)
		{
			consoleForm = newConsoleForm;
			consoleForm.consoleHandler = this;

			commandHandlerDictionary.Add("cd", this.changeDirectory);

			printPrompt();
		}

		void printPrompt()
		{
			string workingDirectory = System.IO.Directory.GetCurrentDirectory();
			string prompt = workingDirectory + "> ";
			print(prompt);
		}

		void print(string text)
		{
			consoleForm.consoleBox.AppendText(text);
		}

		void printLine(string line)
		{
			print(line + "\n");
		}

		public void enter()
		{
			print("\n");
			handleEnter();
			printPrompt();
			consoleForm.inputBox.SelectAll();
		}

		void visualiseArguments(CommandArguments arguments)
		{
			printLine("Command: \"" + arguments.command + "\"");
			string output = "Arguments:";
			for (int i = 0; i < arguments.arguments.Length; i++)
			{
				string argument = arguments.arguments[i];
				output += " " + (i + 1).ToString() + ". \"" + argument + "\"";
			}
			printLine(output);
		}

		void changeDirectory(string[] arguments)
		{
			System.IO.Directory.SetCurrentDirectory(arguments[0]);
		}

		public void handleEnter()
		{
			string line = consoleForm.inputBox.Text;
			line = line.Trim();
			if (line.Length == 0)
				return;

			CommandArguments arguments;

			try
			{
				arguments = new CommandArguments(line);
				//visualiseArguments(arguments);
			}
			catch (ArgumentException exception)
			{
				printLine(exception.Message);
				return;
			}

			if (commandHandlerDictionary.ContainsKey(arguments.command))
				commandHandlerDictionary[arguments.command](arguments.arguments);
			else
			{
				//got to check for executable programs matching that name now
			}
		}
	}
}
