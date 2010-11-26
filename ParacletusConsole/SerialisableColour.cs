using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace ParacletusConsole
{
	public class SerialisableColour
	{
		public int
			Alpha,
			Red,
			Green,
			Blue;

		public SerialisableColour()
		{
		}

		public SerialisableColour(int red, int green, int blue)
		{
			Alpha = 0xff;
			Red = red;
			Green = green;
			Blue = blue;
		}

		public Color ToColour()
		{
			return Color.FromArgb(Alpha, Red, Green, Blue);
		}
	}
}
