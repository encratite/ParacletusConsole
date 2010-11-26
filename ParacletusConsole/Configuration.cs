using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ParacletusConsole
{
	public class Configuration
	{
		public static string ConfigurationFile = "Paracletus.xml";

		public string Prompt;
		public Nil.FormState FormState;

		public Color
			CommandTextInputColour,
			CommandTextOutputColour,
			DefaultOutputColour,
			ErrorColour,
			BackgroundColour,
			InputFieldBackgroundColour;

		public Configuration()
		{
			Prompt = "#eaff00$User$#96ff00@#eaff00$MachineName$ #7debff$CurrentWorkingDirectory$#506dff> ";
			CommandTextInputColour = Color.White;
			CommandTextOutputColour = Color.White;
			DefaultOutputColour = Color.White;
			ErrorColour = Color.Red;
			BackgroundColour = Color.Black;
			InputFieldBackgroundColour = Color.DarkGray;
			FormState = new Nil.FormState();
		}
	}
}
