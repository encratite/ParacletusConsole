namespace ParacletusConsole
{
	public class CommandScript: IConsoleScript
	{
		public void Execute(ConsoleHandler consoleHandler)
		{
			consoleHandler.AddCommand("cd", "<directory>", "change the working directory", consoleHandler.ChangeDirectory, 1);
		}
	}
}
