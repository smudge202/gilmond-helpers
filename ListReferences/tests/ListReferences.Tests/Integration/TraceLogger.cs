using System;
using Microsoft.Framework.Logging;
using System.Diagnostics;

namespace Gilmond.Helpers.ListReferences.Tests.Integration
{
	sealed class TraceLogger : ILogger
	{
		public TraceLogger()
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Out));
		}

		public IDisposable BeginScopeImpl(object state)
		{
			throw new NotImplementedException();
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return true;
		}

		public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
		{
			if (exception == null)
				Console.WriteLine($"{DateTime.Now.ToShortTimeString()}-[{logLevel}]:\t{state}");
			else
				Console.WriteLine($"{DateTime.Now.ToShortTimeString()}-[{logLevel}]:\t{state}\r\n{exception}");
		}
	}
}
