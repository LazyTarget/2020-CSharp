using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNet.Tests
{
	[TestClass]
	public abstract class RandomizerTests
	{
		private string ApiKey;
		protected virtual string Map { get; }


		[TestInitialize]
		public void Initialize()
		{

		}

		public GameRunner InitRunner()
		{
			if (string.IsNullOrWhiteSpace(Map))
				throw new ArgumentNullException(nameof(Map));

			return GameRunner.New(ApiKey, Map);
		}

		[TestMethod]
		public void DefaultStrategy()
		{
			var runner = InitRunner();
			var score = runner.Run();
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestClass]
		public class training1Map : RandomizerTests
		{
			protected override string Map { get; } = "training1";
		}
	}
}
