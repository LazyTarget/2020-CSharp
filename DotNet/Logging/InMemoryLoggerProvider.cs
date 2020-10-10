using System.Text;
using Microsoft.Extensions.Logging;

namespace DotNet.Logging
{
	public class InMemoryLoggerProvider : ILoggerProvider
	{
		public InMemoryLoggerProvider(StringBuilder stringBuilder)
		{
			_stringBuilder = stringBuilder;
		}

		private readonly StringBuilder _stringBuilder;

		public ILogger CreateLogger(string categoryName)
		{
			var logger = new InMemoryLogger(categoryName, _stringBuilder);
			return logger;
		}

		public void Dispose()
		{
			
		}
	}
}