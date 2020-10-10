using System;
using System.Text;
using Microsoft.Extensions.Logging;

namespace DotNet.Logging
{
	public class InMemoryLogger : ILogger
	{
		private readonly string _categoryName;

		public readonly StringBuilder StringBuilder;

		public InMemoryLogger(string categoryName, StringBuilder stringBuilder = null)
		{
			_categoryName = categoryName;
			StringBuilder = stringBuilder ?? new StringBuilder();
		}

		public bool Enabled { get; set; } = true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			var msg = formatter.Invoke(state, exception);
			var lvl = GetLogLevelAsShortString(logLevel);
			var formattedLine = $"{lvl}: {_categoryName}[{eventId.Id}]: {msg}";
			StringBuilder.AppendLine(formattedLine);
		}

		public string GetLogLevelAsShortString(LogLevel logLevel)
		{
			switch (logLevel)
			{
				case LogLevel.Critical:
					return "FTL";

				case LogLevel.Error:
					return "ERR";

				case LogLevel.Warning:
					return "WRN";

				case LogLevel.Information:
					return "INF";

				case LogLevel.Debug:
					return "DBG";

				case LogLevel.Trace:
					return "TRC";

				default:
					return logLevel.ToString();
			}
		}

		public bool IsEnabled(LogLevel logLevel)
		{
			return Enabled;
		}

		public IDisposable BeginScope<TState>(TState state)
		{
			return null;
		}
	}
}
