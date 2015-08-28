using Microsoft.Framework.Logging;
using System;

namespace CodeGate.Tfs.ApplicationTier
{
	sealed class ColouredConsoleLogger : ILogger
	{
		public IDisposable BeginScopeImpl(object state)
		{
			throw new NotImplementedException();
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			throw new NotImplementedException();
		}

		public void Log(LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter)
		{
			var outputStrategy = DetermineOutputStrategy(logLevel);
			outputStrategy.Log(state, exception, formatter);
		}

		OutputStrategy DetermineOutputStrategy(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Debug:
				case LogLevel.Verbose:
					return OutputStrategy.Info;
				case LogLevel.Warning:
					return OutputStrategy.Warn;
				case LogLevel.Error:
				case LogLevel.Critical:
					return OutputStrategy.Fail;
				default:
					return OutputStrategy.Normal;
			}
		}

		sealed class OutputStrategy
		{
			public static OutputStrategy Info { get; } = new OutputStrategy(ConsoleColor.Gray);
			public static OutputStrategy Warn { get; } = new OutputStrategy(ConsoleColor.Yellow);
			public static OutputStrategy Fail { get; } = new OutputStrategy(ConsoleColor.Red);
			public static OutputStrategy Normal { get; } = new OutputStrategy(ConsoleColor.White);

			readonly ConsoleColor _outputColour;

			OutputStrategy(ConsoleColor outputColour)
			{
				_outputColour = outputColour;
			}

			internal void Log(object state, Exception exception, Func<object, Exception, string> formatter)
			{
				var currentColour = Console.ForegroundColor;
				try
				{
					Console.ForegroundColor = _outputColour;
					if (formatter == null)
						Write(state, exception);
					else
						Console.WriteLine(formatter(state, exception));
				}
				finally
				{
					Console.ForegroundColor = currentColour;
				}
			}

			void Write(object state, Exception exception)
			{
				if (exception == null)
					Console.WriteLine($"{DateTime.Now.ToShortTimeString()}:\t{state}");
				else
					Console.WriteLine($"{DateTime.Now.ToShortTimeString()}:\t{state}\r\n{exception}");
			}
		}
	}
}
