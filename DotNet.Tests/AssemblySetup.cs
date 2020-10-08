using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNet.Tests
{
	[TestClass]
	public static class AssemblySetup
	{
		public static IConfiguration Configuration { get; private set; }

		public static string ApiKey { get; private set; }

		[AssemblyInitialize]
		public static Task AssemblyInitialize(TestContext testContext)
		{
			Configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.AddEnvironmentVariables("CONSIDITION_")
				.AddUserSecrets(typeof(AssemblySetup).Assembly, true)
				.Build();

			ApiKey = Configuration.GetValue<string>("ApiKey");
			if (string.IsNullOrWhiteSpace(ApiKey))
				throw new ArgumentNullException(nameof(ApiKey));

			return Task.CompletedTask;
		}
	}
}
