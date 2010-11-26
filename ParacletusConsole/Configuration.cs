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
			DefaultOutputColour,
			ErrorColour,
			BackgroundColour,
			InputFieldBackgroundColour;

		public string Font;
		public float FontSize;

		public Configuration()
		{
			SerialisableColour
				white = new SerialisableColour(0xff, 0xff, 0xff),
				red = new SerialisableColour(0xff, 0, 0),
				black = new SerialisableColour(0, 0, 0),
				darkGrey = new SerialisableColour(0x40, 0x40, 0x40);

			Prompt = "#ff57aaff[ #ffffc05e$User$#ffffd4b2@#ffffc05e$MachineName$ #ffcacaca$CurrentWorkingDirectory$ #ff57aaff] ";

			CommandTextInputColour = white;
			CommandTextOutputColour = white;
			DefaultOutputColour = white;
			ErrorColour = red;
			BackgroundColour = black;
			InputFieldBackgroundColour = darkGrey;

			Font = "Lucida Console";
			FontSize = 8.0f;

			FormState = new Nil.FormState();
		}
	}
}
