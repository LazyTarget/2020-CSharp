using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
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
			GameState state = GameLayer.GetState();
			_logger.LogInformation("Map: " + Environment.NewLine + state.MapToString());

			var randomizer = new Randomizer(GameLayer, _loggerFactory, strategy);
			while (state.Turn < state.MaxTurns)
			{
				PrintDebug_NewTurn(state);

				randomizer.HandleTurn();

				foreach (var message in state.Messages)
				{
					_logger.LogInformation(message);
				}

				foreach (var error in state.Errors)
				{
					_logger.LogError("Error: " + error);
				}
			}

			state = GameLayer.GetState();
			var score = GameLayer.GetScore(state.GameId);

			_logger.LogInformation("");
			_logger.LogInformation("");
			_logger.LogInformation($"Done with game: {state.GameId}");
			_logger.LogInformation("");
			_logger.LogInformation($"::SUMMARY::");
			_logger.LogInformation($"Funds: {state.Funds}");
			_logger.LogInformation($"Buildings: {state.GetCompletedBuildings().Count()}");
			_logger.LogInformation($"Upgrades: {state.GetCompletedBuildings().Sum(x => x.Effects.Count)}");
			_logger.LogInformation("");
			_logger.LogInformation($"::ACTIONS::");
			foreach (GameActions action in Enum.GetValues(typeof(GameActions)))
			{
				var count = state.ActionHistory.Count(x => x.Value == action);
				if (count < 1)
					continue;
				var percent = count / (double) state.ActionHistory.Count;
				_logger.LogInformation($"\t{percent:P1}\t {action} ({count}/{state.ActionHistory.Count})");
			}
			_logger.LogInformation("");
			_logger.LogInformation($"::SCORE::");
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
				
				// Sleep again to make sure the API and Database has ended the game
				Thread.Sleep(5000);
				_logger.LogInformation("Finished sleeping after signaled Api that the game has ended");
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
