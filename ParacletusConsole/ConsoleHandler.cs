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
		ConsoleForm MainForm;

		public delegate void CommandHandlerFunction(string[] arguments);
		Dictionary<string, CommandHandler> CommandHandlerDictionary;
		bool Terminating;
		bool Writable;

		Process Process;

		AsynchronousReadHandler
			StandardOutputReader,
			StandardErrorReader;

		public ConsoleHandler(ConsoleForm consoleForm)
		{
			consoleForm.consoleHandler = this;
			this.MainForm = consoleForm;
			Process = null;
			Terminating = false;
			Writable = false;

			CommandHandlerDictionary = new Dictionary<string, CommandHandler>();
			AddCommand("cd", "<directory>", "change the working directory", this.ChangeDirectory, 1);
		}

		public void FormLoaded()
		{
			PrintPrompt();
		}

		void AddCommand(string command, string argumentDescription, string description, CommandHandlerFunction function, int argumentCount)
		{
			CommandHandler handler = new CommandHandler(command, argumentDescription, description, function, argumentCount);
			CommandHandlerDictionary.Add(command, handler);
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
			MainForm.consoleBox.Invoke
			(
				(MethodInvoker) delegate
				{
					MainForm.consoleBox.AppendText(text);
					SendMessage(MainForm.consoleBox.Handle, WM_VSCROLL, SB_BOTTOM, 0);
				}
			);
		}

		void PrintLine(string line)
		{
			Print(line + "\n");
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
			MainForm.inputBox.Invoke
			(
				(MethodInvoker)delegate
				{
					MainForm.inputBox.SelectAll();
				}
			);
		}

		public void Enter()
		{
			Console.WriteLine("Enter()");
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

		void ProcessRegularEnter()
		{
			Console.WriteLine("ProcessRegularEnter()");
			string line = MainForm.inputBox.Text;
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

			if (CommandHandlerDictionary.ContainsKey(arguments.Command))
			{
				CommandHandler handler = CommandHandlerDictionary[arguments.Command];
				if (arguments.Arguments.Length != handler.ArgumentCount)
				{
					PrintLine("Invalid argument count.");
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
					PrintLine(exception.Message);
					PromptAndSelect();
				}
			}
		}

		void ProcessRuntimeEnter()
		{
			if (Writable)
			{
				Console.WriteLine("ProcessRuntimeEnter()");
				string line = MainForm.inputBox.Text;
				PrintLine(line);
				Process.StandardInput.WriteLine(line);
			}
		}

		public void Closing()
		{
			lock (this)
			{
				Terminating = true;
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
				PrintLine("Process has been terminated");
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
