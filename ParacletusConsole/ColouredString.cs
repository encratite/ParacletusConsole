using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ParacletusConsole
{
	public class ColouredString
	{
		public string Content;
		public SerialisableColour Colour;

		public ColouredString(string content, SerialisableColour colour = null)
		{
			Content = content;
			Colour = colour;
		}
	}
}
