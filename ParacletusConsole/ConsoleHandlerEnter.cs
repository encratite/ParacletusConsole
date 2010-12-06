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

			Execute(line);
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
