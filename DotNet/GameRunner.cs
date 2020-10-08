using System;
using System.Diagnostics;
using System.Linq;
using DotNet.models;
using DotNet.Strategy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace DotNet
{
	public class GameRunner
	{
		#region Static

		public static GameRunner New(string apiKey, string map, ILoggerFactory loggerFactory)
		{
			var runner = new GameRunner(loggerFactory)
			{
				GameLayer = new GameLayer(apiKey),
			};

			runner._logger.LogInformation($"New game: {map}");
			var gameId = runner.GameLayer.NewGame(map);

			runner._logger.LogInformation($"Starting game: {gameId}");
			runner.GameLayer.StartGame(gameId);
			return runner;
		}

		public static GameRunner Resume(string apiKey, string gameId, ILoggerFactory loggerFactory)
		{
			var runner = new GameRunner(loggerFactory)
			{
				GameLayer = new GameLayer(apiKey),
			};;

			runner.GameLayer.GetNewGameInfo(gameId);

			runner.GameLayer.GetNewGameState(runner.GameLayer.GetState().GameId);

			var state = runner.GameLayer.GetState();
			if (!string.IsNullOrWhiteSpace(gameId))
				runner._logger.LogInformation($"Resuming game specified game: {gameId} on turn {state.Turn}");
			else
				runner._logger.LogInformation($"Resuming previous game: {state.GameId} on turn {state.Turn}");
			return runner;
		}

		#endregion Static

		private GameLayer GameLayer;
		private readonly ILogger _logger;
		private readonly ILoggerFactory _loggerFactory;

		public GameRunner(ILoggerFactory loggerFactory)
		{
			_loggerFactory = loggerFactory ?? new NullLoggerFactory();
			_logger = _loggerFactory?.CreateLogger<GameRunner>();
		}

		public ScoreResponse Run(TurnStrategyBase strategy = null)
		{
			// Make actions
			GameState state;
			var randomizer = new Randomizer(GameLayer, strategy);
			while (GameLayer.GetState().Turn < GameLayer.GetState().MaxTurns)
			{
				state = GameLayer.GetState();
				PrintDebug_NewTurn(state);

				randomizer.HandleTurn();

				//take_turn(gameId);

				foreach (var message in GameLayer.GetState().Messages)
				{
					_logger.LogInformation(message);
					//if (Debugger.IsAttached)
					//	Debug.WriteLine(message);
				}

				foreach (var error in GameLayer.GetState().Errors)
				{
					_logger.LogError("Error: " + error);
				}
			}

			state = GameLayer.GetState();
			_logger.LogInformation("");
			_logger.LogInformation("");
			_logger.LogInformation($"Done with game: {state.GameId}");
			_logger.LogInformation($"Funds: {state.Funds}");
			_logger.LogInformation($"Buildings: {state.GetCompletedBuildings().Count()}");
			_logger.LogInformation($"Upgrades: {state.GetCompletedBuildings().Sum(x => x.Effects.Count)}");

			var score = GameLayer.GetScore(state.GameId);
			_logger.LogInformation("");
			_logger.LogInformation($"Final score: {score.FinalScore}");
			_logger.LogInformation($"Co2: {score.TotalCo2}");
			_logger.LogInformation($"Pop: {score.FinalPopulation}");
			_logger.LogInformation($"Happiness: {score.TotalHappiness}");
			return score;
		}

		public void EndGame()
		{
			var state = GameLayer?.GetState();
			if (state == null)
				return;

			var ended = state.Turn >= state.MaxTurns;
			if (ended)
			{
				// Automatic ending
			}
			else
			{
				// Ends a game prematurely
				// This is not needed to end a game that has been completed by playing all turns.
				GameLayer.EndGame(state.GameId);
				_logger.LogInformation("Game ended prematurely");
			}
		}

		private void PrintDebug_NewTurn(GameState state)
		{
			var currentPop = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().Sum(x => x.CurrentPop);

			var currentPopMax = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>()
				.Join(state.AvailableResidenceBuildings, ok => ok.BuildingName, ik => ik.BuildingName,
					(rb, bp) => new { bp, rb })
				.Sum(x => x.bp.MaxPop);

			var currentPopPercentage = currentPopMax > 0 ? currentPop / (double)currentPopMax : 0;

			_logger.LogDebug($"Begin New Turn :: Turn={state.Turn}, Funds={state.Funds}, Temp={state.CurrentTemp}, Pop: {currentPop}/{currentPopMax} ({currentPopPercentage:P2})");
		}
	}
}
