using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class Configuration
	{
		public static string ConfigurationFile = "Paracletus.xml";
		public static string DefaultScriptFile = "Paracletus.cs";

		public string Prompt;

		public SerialisableColour
			CommandTextInputColour,
			CommandTextOutputColour,
			CommandTextOutputHighlightColour,

			DefaultOutputColour,
			ErrorColour,
			WarningColour,
			BackgroundColour,
			InputFieldBackgroundColour,
			SelectionBackgroundColour,

			TitleColour,
			HighlightColour,

			ListDirectoryColour,
			ListFileColour,
			ListExecutableColour;

		public string Font;
		public float FontSize;

		public bool LowerCaseMachineName;

		public Nil.FormState FormState;

		public Nil.SerialisableDictionary<string, string> Aliases;

		public Configuration()
		{
			SerialisableColour white = new SerialisableColour(255, 255, 255);

			Prompt = "#ff57aaff[ #ffffc05e$User$#ffffd4b2@#ffffc05e$MachineName$ #ff57aaff| #ffcacaca$CurrentWorkingDirectory$ #ff57aaff] ";

			CommandTextInputColour = white;
			CommandTextOutputColour = new SerialisableColour(213, 228, 191);
			CommandTextOutputHighlightColour = new SerialisableColour(255, 116, 48);

			DefaultOutputColour = white;
			ErrorColour = new SerialisableColour(255, 0, 0);
			WarningColour = new SerialisableColour(0, 255, 255);
			BackgroundColour = new SerialisableColour(0, 0, 0);
			InputFieldBackgroundColour = new SerialisableColour(64, 64, 64);
			SelectionBackgroundColour = new SerialisableColour(177, 136, 66);

			TitleColour = new SerialisableColour(85, 255, 161);
			HighlightColour = new SerialisableColour(197, 128, 255);

			ListDirectoryColour = new SerialisableColour(85, 255, 209);
			ListFileColour = new SerialisableColour(255, 247, 209);
			ListExecutableColour = new SerialisableColour(255, 100, 231);

			Font = "Lucida Console";
			FontSize = 8.0f;

			LowerCaseMachineName = true;

			FormState = new Nil.FormState();

			Aliases = new Nil.SerialisableDictionary<string, string>();
		}
	}
}
