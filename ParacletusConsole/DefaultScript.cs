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
		}
	}
}
