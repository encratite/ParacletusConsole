namespace ParacletusConsole
{
	public class CommandScript: IConsoleScript
	{
		public void Execute(ConsoleHandler consoleHandler)
		{
			consoleHandler.AddCommand("cd", "<directory>", "change the working directory", consoleHandler.ChangeDirectory, 1);
			consoleHandler.AddCommand("help", null, "prints the help menu", consoleHandler.PrintHelp);
			consoleHandler.AddCommand("clear", null, "clears the console", consoleHandler.ClearConsole);
			consoleHandler.AddCommand("print", "<path to file>", "prints a file to the console", consoleHandler.PrintFile, 1);
			consoleHandler.AddCommand("cp", "<source> <destination>", "copy a file or a directory", consoleHandler.CopyFile, 2);
			consoleHandler.AddCommand("echo", "<strings with colour codes and variables>", "print a formatted string", consoleHandler.Echo, -1);
			consoleHandler.AddCommand("ps", null, "print a list of running processes", consoleHandler.ProcessList);
			consoleHandler.AddCommand("kill", "<PID or name of executable>", "terminate a process", consoleHandler.KillProcess, 1);
			consoleHandler.AddCommand("ls", "<path>", "list the contents of a directory", consoleHandler.ListDirectoryContents, -1);
		}
	}
}
