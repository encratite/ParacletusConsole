using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;

namespace ParacletusConsole
{
	public class ConsoleHandler
	{
		ConsoleForm consoleForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> commandHandlerDictionary;

		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.consoleHandler = this;
			this.consoleForm = consoleForm;

			commandHandlerDictionary = new Dictionary<string, CommandHandler>();
			addCommand("cd", "<directory>", "change the working directory", this.changeDirectory, 1);

			printPrompt();
		}

		void addCommand(string command, string argumentDescription, string description, CommandHandlerFunction function, int argumentCount)
		{
			CommandHandler handler = new CommandHandler(command, argumentDescription, description, function, argumentCount);
			commandHandlerDictionary.Add(command, handler);
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
			print(line + "\n");
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
			{
				CommandHandler handler = commandHandlerDictionary[arguments.command];
				if (arguments.arguments.Length != handler.argumentCount)
				{
					printLine("Invalid argument count.");
					printLine(handler.usage());
					return;
				}
				handler.function(arguments.arguments);
			}
			else
			{
				//check for executable programs matching that name
				Process process = new Process();

				ProcessStartInfo info = process.StartInfo;
				info.UseShellExecute = false;
				info.RedirectStandardOutput = true;
				info.FileName = arguments.command;
				info.Arguments = arguments.getQuotedArguments();
				info.WindowStyle = ProcessWindowStyle.Hidden;
				info.CreateNoWindow = true;

				process.Start();
				string output = process.StandardOutput.ReadToEnd();
				process.WaitForExit();
				printLine(output);
			}
		}
	}
}
