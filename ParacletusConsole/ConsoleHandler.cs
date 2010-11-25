using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace ParacletusConsole
{
	public class ConsoleHandler
	{
		ConsoleForm consoleForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> commandHandlerDictionary;

		Process process;

		AsynchronousReadHandler
			standardOutputReader,
			standardErrorReader;

		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.consoleHandler = this;
			this.consoleForm = consoleForm;
			process = null;

			commandHandlerDictionary = new Dictionary<string, CommandHandler>();
			AddCommand("cd", "<directory>", "change the working directory", this.ChangeDirectory, 1);

			PrintPrompt();
		}

		void AddCommand(string command, string argumentDescription, string description, CommandHandlerFunction function, int argumentCount)
		{
			CommandHandler handler = new CommandHandler(command, argumentDescription, description, function, argumentCount);
			commandHandlerDictionary.Add(command, handler);
		}

		void PrintPrompt()
		{
			string workingDirectory = System.IO.Directory.GetCurrentDirectory();
			string prompt = workingDirectory + "> ";
			Print(prompt);
		}

		const int WM_VSCROLL = 0x115;
		const int SB_BOTTOM = 7;

		[DllImport("user32.dll")]
		public static extern IntPtr SendMessage(IntPtr window, int message, int wparam, int lparam);

		void Print(string text)
		{
			consoleForm.consoleBox.AppendText(text);
			SendMessage(consoleForm.consoleBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
		}

		void PrintLine(string line)
		{
			Print(line + "\n");
		}

		public void Enter()
		{
			HandleEnter();
			PrintPrompt();
			consoleForm.inputBox.SelectAll();
		}

		void VisualiseArguments(CommandArguments arguments)
		{
			PrintLine("Command: \"" + arguments.command + "\"");
			string output = "Arguments:";
			for (int i = 0; i < arguments.arguments.Length; i++)
			{
				string argument = arguments.arguments[i];
				output += " " + (i + 1).ToString() + ". \"" + argument + "\"";
			}
			PrintLine(output);
		}

		void ChangeDirectory(string[] arguments)
		{
			System.IO.Directory.SetCurrentDirectory(arguments[0]);
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

		public void HandleEnter()
		{
			string line = consoleForm.inputBox.Text;
			Print(line + "\n");
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
				PrintLine(exception.Message);
				return;
			}

			if (commandHandlerDictionary.ContainsKey(arguments.command))
			{
				CommandHandler handler = commandHandlerDictionary[arguments.command];
				if (arguments.arguments.Length != handler.argumentCount)
				{
					PrintLine("Invalid argument count.");
					PrintLine(handler.Usage());
					return;
				}
				handler.function(arguments.arguments);
			}
			else
			{
				lock (this)
				{
					try
					{
						//check for executable programs matching that name
						process = new Process();

						ProcessStartInfo info = process.StartInfo;
						info.UseShellExecute = false;
						info.RedirectStandardInput = true;
						info.RedirectStandardOutput = true;
						info.RedirectStandardError = true;
						info.FileName = arguments.command;
						info.Arguments = arguments.GetQuotedArguments();
						info.WindowStyle = ProcessWindowStyle.Hidden;
						info.CreateNoWindow = true;

						process.Start();

						standardOutputReader = new AsynchronousReadHandler(this, HandleStandardOutputRead, process.StandardOutput);
						standardErrorReader = new AsynchronousReadHandler(this, HandleStandardErrorRead, process.StandardError);
					}
					catch (System.ComponentModel.Win32Exception exception)
					{
						PrintLine(exception.Message);
					}
				}
			}
		}
	}
}
