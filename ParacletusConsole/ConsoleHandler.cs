using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ParacletusConsole
{
	public class ConsoleHandler
	{
		ConsoleForm consoleForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> commandHandlerDictionary;
		bool terminating;

		Process process;

		AsynchronousReadHandler
			standardOutputReader,
			standardErrorReader;

		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.consoleHandler = this;
			this.consoleForm = consoleForm;
			process = null;
			terminating = false;

			commandHandlerDictionary = new Dictionary<string, CommandHandler>();
			AddCommand("cd", "<directory>", "change the working directory", this.ChangeDirectory, 1);
		}

		public void FormLoaded()
		{
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
			consoleForm.consoleBox.Invoke
			(
				(MethodInvoker) delegate
				{
					consoleForm.consoleBox.AppendText(text);
					SendMessage(consoleForm.consoleBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
				}
			);
		}

		void PrintLine(string line)
		{
			Print(line + "\n");
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

		void PromptAndSelect()
		{
			PrintPrompt();
			consoleForm.inputBox.Invoke
			(
				(MethodInvoker)delegate
				{
					consoleForm.inputBox.SelectAll();
				}
			);
		}

		public void Enter()
		{
			Console.WriteLine("Enter()");
			lock (this)
			{
				if (process == null)
					ProcessRegularEnter();
				else
					ProcessRuntimeEnter();
			}
		}

		void ProcessTerminationHandler()
		{
			try
			{
				process.WaitForExit();
			}
			catch (NullReferenceException)
			{
			}
			lock (this)
			{
				if (!terminating)
				{
					process = null;
					PromptAndSelect();
				}
			}
		}

		void ProcessRegularEnter()
		{
			Console.WriteLine("ProcessRegularEnter()");
			string line = consoleForm.inputBox.Text;
			PrintLine(line);
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

			if (commandHandlerDictionary.ContainsKey(arguments.command))
			{
				CommandHandler handler = commandHandlerDictionary[arguments.command];
				if (arguments.arguments.Length != handler.argumentCount)
				{
					PrintLine("Invalid argument count.");
					PrintLine(handler.Usage());
				}
				else
					handler.function(arguments.arguments);
				PromptAndSelect();
			}
			else
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

					new Thread(ProcessTerminationHandler).Start();
				}
				catch (System.ComponentModel.Win32Exception exception)
				{
					PrintLine(exception.Message);
					PromptAndSelect();
				}
			}
		}

		void ProcessRuntimeEnter()
		{
			Console.WriteLine("ProcessRuntimeEnter()");
			string line = consoleForm.inputBox.Text;
			PrintLine(line);
			process.StandardInput.WriteLine(line);
		}

		public void Closing()
		{
			lock (this)
			{
				terminating = true;
				if (process != null)
					process.Kill();
			}
		}
	}
}
