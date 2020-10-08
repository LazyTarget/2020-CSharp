using System;
using DotNet.models;
using DotNet.Strategy;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNet.Tests
{
	[TestClass]
	public class ReplayTests
	{
		private string ApiKey;

		[TestInitialize]
		public void Initialize()
		{
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.AddEnvironmentVariables("CONSIDITION_")
				.AddUserSecrets<RandomizerTests>(true)
				.Build();
			ApiKey = configuration.GetValue<string>("ApiKey");
			if (string.IsNullOrWhiteSpace(ApiKey))
				throw new ArgumentNullException(nameof(ApiKey));
		}


		protected virtual GameReplayResponse LoadReplay(string gameId)
		{
			var gameLayer = new GameLayer(ApiKey);
			var replay = gameLayer.GetReplay(gameId);
			Assert.IsNotNull(replay);

			foreach (var update in replay.StateUpdates)
			{
				foreach (var message in update.Messages)
				{
					Console.WriteLine(message);
				}

				foreach (var error in update.Errors)
				{
					Console.WriteLine("Error: " + error);
				}
			}
			return replay;
		}


		[TestMethod]
		public void LoadSpecifiedReplay()
		{
			var gameId = "ce7a97e2-e499-4b1e-9a1c-f72a35f0ac2d";

			var gameLayer = new GameLayer(ApiKey);
			var replay = LoadReplay(gameId);
		}

		[TestMethod]
		public void LoadLastReplay()
		{
			var gameLayer = new GameLayer(ApiKey);
			var gameInfo = gameLayer.GetNewGameState(null);
			var replay = LoadReplay(gameInfo.GameId);

		}
	}
}
