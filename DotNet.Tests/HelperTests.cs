using System;
using System.Linq;
using DotNet.models;
using DotNet.Strategy;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNet.Tests
{
	[TestClass]
	public class HelperTests
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

			var state = replay.StateUpdates.Last();

			var score = replay.Score;
			Console.WriteLine($"::Score::");
			Console.WriteLine($"Final score: {score.FinalScore}");
			Console.WriteLine($"Co2: {score.TotalCo2}");
			Console.WriteLine($"Pop: {score.FinalPopulation}");
			Console.WriteLine($"Happiness: {score.TotalHappiness}");
			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine($"::Summary::");
			Console.WriteLine($"Funds: {state.Funds}");
			Console.WriteLine($"Buildings: {state.GetCompletedBuildings().Count()}");
			Console.WriteLine($"Upgrades: {state.GetCompletedBuildings().Sum(x => x.Effects.Count)}");
			Console.WriteLine();
			Console.WriteLine();

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


		[TestMethod]
		public void PurgeActiveGames()
		{
			var gameLayer = new GameLayer(ApiKey);
			var games = gameLayer.GetGames();
			foreach (var game in games)
			{
				if (!game.Active)
					continue;
				
				Console.WriteLine($"Game :: {game.GameId}");
				Console.WriteLine($"Started = {game.Started}");
				Console.WriteLine($"StartedAt = {game.StartedAt}");

				gameLayer.EndGame(game.GameId);
				Console.WriteLine($"Ending game...");

				Console.WriteLine();
			}

			Console.WriteLine();
			Console.WriteLine("Total games: " + games.Count);
			Console.WriteLine("Active games: " + games.Count(x => x.Active));
			Console.WriteLine("Started games: " + games.Count(x => x.Started));
		}
	}
}
