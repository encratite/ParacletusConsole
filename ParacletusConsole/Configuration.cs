using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class Configuration
	{
		public static string ConfigurationFile = "Paracletus.xml";

		public string Prompt;
		public Nil.FormState FormState;

		public SerialisableColour
			CommandTextInputColour,
			CommandTextOutputColour,
			CommandTextOutputHighlightColour,
			DefaultOutputColour,
			ErrorColour,
			BackgroundColour,
			InputFieldBackgroundColour;

		public string Font;
		public float FontSize;

		public Configuration()
		{
			SerialisableColour white = new SerialisableColour(255, 255, 255);

			Prompt = "#ff57aaff[ #ffffc05e$User$#ffffd4b2@#ffffc05e$MachineName$ #ffcacaca$CurrentWorkingDirectory$ #ff57aaff] ";

			CommandTextInputColour = white;
			CommandTextOutputColour = new SerialisableColour(213, 228, 191);
			CommandTextOutputHighlightColour = new SerialisableColour(255, 116, 48);
			DefaultOutputColour = white;
			ErrorColour = new SerialisableColour(255, 0, 0);
			BackgroundColour = new SerialisableColour(0, 0, 0);
			InputFieldBackgroundColour = new SerialisableColour(64, 64, 64);

			Font = "Lucida Console";
			FontSize = 8.0f;

			FormState = new Nil.FormState();
		}
	}
}
