using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ParacletusConsole
{
	public class AsynchronousReadHandler
	{
		public delegate void ReadHandler(byte[] buffer, int bytesRead);

		ConsoleHandler ConsoleHandler;
		StreamReader Stream;
		AsyncCallback Callback;
		ReadHandler DelegateInstance;

		const int ReadSize = 1024;
		byte[] Buffer;

		public AsynchronousReadHandler(ConsoleHandler consoleHandler, ReadHandler readHandler, StreamReader stream)
		{
			this.ConsoleHandler = consoleHandler;
			this.DelegateInstance = readHandler;
			this.Stream = stream;

			Buffer = new byte[ReadSize];
			Callback = new AsyncCallback(ReadCallback);

			Read();
		}

		void Read()
		{
			Stream.BaseStream.BeginRead(Buffer, 0, ReadSize, Callback, null);
		}

		void ReadCallback(IAsyncResult result)
		{
			lock (ConsoleHandler)
			{
				int bytesRead = Stream.BaseStream.EndRead(result);
				if (bytesRead == 0)
					return;
				DelegateInstance(Buffer, bytesRead);
				Read();
			}
		}
	}
}
