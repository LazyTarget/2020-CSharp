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
			var formattedLine = $"{logLevel}: {_categoryName}[{eventId.Id}]: {msg}";
			StringBuilder.AppendLine(formattedLine);
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
