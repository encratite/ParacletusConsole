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

		ConsoleHandler consoleHandler;
		StreamReader stream;
		AsyncCallback callback;
		ReadHandler readHandler;

		const int ReadSize = 1024;
		byte[] buffer;

		public AsynchronousReadHandler(ConsoleHandler consoleHandler, ReadHandler readHandler, StreamReader stream)
		{
			this.consoleHandler = consoleHandler;
			this.readHandler = readHandler;
			this.stream = stream;

			buffer = new byte[ReadSize];
			callback = new AsyncCallback(ReadCallback);

			Read();
		}

		void Read()
		{
			stream.BaseStream.BeginRead(buffer, 0, ReadSize, callback, null);
		}

		void ReadCallback(IAsyncResult result)
		{
			lock (consoleHandler)
			{
				int bytesRead = stream.BaseStream.EndRead(result);
				if (bytesRead == 0)
					return;
				readHandler(buffer, bytesRead);
				Read();
			}
		}
	}
}
