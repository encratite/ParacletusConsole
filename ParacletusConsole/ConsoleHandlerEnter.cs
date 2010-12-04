using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace ParacletusConsole
{
	public partial class ConsoleHandler
	{
		public void Enter()
		{
			lock (this)
			{
				if (Process == null)
					ProcessRegularEnter();
				else
					ProcessRuntimeEnter();
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
				if (handler.ArgumentCount != -1 && arguments.Arguments.Length != handler.ArgumentCount)
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

				new Thread(ProcessTerminationCheckThread).Start();

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
	}
}
