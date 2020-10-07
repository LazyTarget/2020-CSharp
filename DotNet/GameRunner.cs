using System;
using System.Diagnostics;
using System.Linq;
using DotNet.models;
using DotNet.Strategy;

namespace DotNet
{
	public class GameRunner
	{
		#region Static

		public static GameRunner New(string apiKey, string map)
		{
			var runner = new GameRunner
			{
				GameLayer = new GameLayer(apiKey),
			};

			Console.WriteLine($"New game: {map}");
			var gameId = runner.GameLayer.NewGame(map);

			Console.WriteLine($"Starting game: {gameId}");
			runner.GameLayer.StartGame(gameId);
			return runner;
		}

		public static GameRunner Resume(string apiKey, string gameId)
		{
			var runner = new GameRunner
			{
				GameLayer = new GameLayer(apiKey),
			};

			runner.GameLayer.GetNewGameInfo(gameId);

			runner.GameLayer.GetNewGameState(runner.GameLayer.GetState().GameId);

			var state = runner.GameLayer.GetState();
			if (!string.IsNullOrWhiteSpace(gameId))
				Console.WriteLine($"Resuming game specified game: {gameId} on turn {state.Turn}");
			else
				Console.WriteLine($"Resuming previous game: {state.GameId} on turn {state.Turn}");
			return runner;
		}

		#endregion Static

		private GameLayer GameLayer;

		public ScoreResponse Run(TurnStrategyBase strategy = null)
		{
			// Make actions
			var randomizer = new Randomizer(GameLayer, strategy);
			while (GameLayer.GetState().Turn < GameLayer.GetState().MaxTurns)
			{
				var state = GameLayer.GetState();
				PrintDebug_NewTurn(state);

				randomizer.HandleTurn();

				//take_turn(gameId);

				foreach (var message in GameLayer.GetState().Messages)
				{
					Console.WriteLine(message);
				}

				foreach (var error in GameLayer.GetState().Errors)
				{
					Console.WriteLine("Error: " + error);
				}
			}

			var gameId = GameLayer.GetState().GameId;
			Console.WriteLine($"Done with game: {gameId}");

			var score = GameLayer.GetScore(gameId);
			Console.WriteLine($"Final score: {score.FinalScore}");
			Console.WriteLine($"Co2: {score.TotalCo2}");
			Console.WriteLine($"Pop: {score.FinalPopulation}");
			Console.WriteLine($"Pop: {score.TotalHappiness}");
			return score;
		}

		private static void PrintDebug_NewTurn(GameState state)
		{
			var currentPop = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().Sum(x => x.CurrentPop);

			var currentPopMax = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>()
				.Join(state.AvailableResidenceBuildings, ok => ok.BuildingName, ik => ik.BuildingName,
					(rb, bp) => new { bp, rb })
				.Sum(x => x.bp.MaxPop);

			var currentPopPercentage = currentPop / (double)currentPopMax;

			Debug.WriteLine($"Begin New Turn :: Turn={state.Turn}, Funds={state.Funds}, Temp={state.CurrentTemp}, Pop: {currentPop}/{currentPopMax} ({currentPopPercentage:P2})");
		}
	}
}
