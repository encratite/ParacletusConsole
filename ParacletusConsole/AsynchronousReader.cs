using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;

namespace ParacletusConsole
{
	public class AsynchronousReader
	{
		public delegate void ReadHandler(byte[] buffer, int bytesRead);
		public delegate void CloseHandler();

		public AutoResetEvent CloseEvent;

		ConsoleHandler ConsoleHandler;
		StreamReader Stream;
		AsyncCallback Callback;

		ReadHandler ReadDelegate;
		CloseHandler CloseDelegate;

		const int ReadSize = 1024;
		byte[] Buffer;

		public AsynchronousReader(ConsoleHandler consoleHandler, ReadHandler readHandler, CloseHandler closeHandler, StreamReader stream)
		{
			ConsoleHandler = consoleHandler;
			ReadDelegate = readHandler;
			CloseDelegate = closeHandler;
			Stream = stream;

			Buffer = new byte[ReadSize];
			Callback = new AsyncCallback(ReadCallback);

			CloseEvent = new AutoResetEvent(false);

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
				{
					CloseEvent.Set();
					return;
				}
				ReadDelegate(Buffer, bytesRead);
				Read();
			}
		}
	}
}
