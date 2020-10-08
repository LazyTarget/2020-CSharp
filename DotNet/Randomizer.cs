using System;
using System.Linq;
using DotNet.Interfaces;
using DotNet.models;
using DotNet.Strategy;
using Microsoft.Extensions.Logging;

namespace DotNet
{
	public class Randomizer
	{
		public readonly Random Random = new Random();

		private readonly ILogger _logger;
		private readonly IGameLayer _gameLayer;
		private readonly GameState _gameState;
		private readonly TurnStrategyBase _strategy;

		public Randomizer(IGameLayer gameLayer, ILoggerFactory loggerFactory, TurnStrategyBase strategy = null)
		{
			_gameLayer = gameLayer;
			_gameState = gameLayer.GetState();
			_logger = loggerFactory.CreateLogger<Randomizer>();

			if (strategy == null)
			{
				_strategy = TurnStrategyBase.Build(loggerFactory)
					.Append<BuildUtilityCloseToResidencesTurnStrategy>(c => c.BuildingName = "Mall")
					.Append<BuildUtilityCloseToResidencesTurnStrategy>(c => c.BuildingName = "Park")
					.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
					.Append<BuyUpgradeTurnStrategy>()
					.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
					.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
					.Append<AdjustBuildingTemperaturesTurnStrategy>()
					.Append<SingletonBuildingTurnStrategy>()
					.Compile();
			}
			else
			{
				_strategy = strategy;
			}
			_logger.LogInformation($"Strategy: {Environment.NewLine + _strategy}");
		}

		public void HandleTurn()
		{
			if (_strategy != null)
			{
				var executed = _strategy.TryExecuteStrategy(this, _gameLayer, _gameState);
				if (executed)
					return;
			}

			//var action = GetRandomAction();
			//HandleAction(action);

			_logger.LogTrace("No strategy executed, invoking Wait as fallback");
			_gameLayer.ExecuteAction(GameActions.Wait);
		}

		[Obsolete("Better to use strategies")]
		public void HandleAction(GameActions action)
		{
			Position position = null;
			object argument = null;
			switch (action)
			{
				case GameActions.StartBuild:
					position = GetRandomBuildablePosition();
					argument = GetRandomBuildingName();
					break;

				case GameActions.Build:
					var buildingsUnderConstruction = _gameState.GetBuildingsUnderConstruction().ToArray();
					var building = buildingsUnderConstruction.ElementAt(Random.Next(0, buildingsUnderConstruction.Length));
					position = building.Position;
					break;

				case GameActions.Maintenance:
					var damagedBuildings = _gameState.ResidenceBuildings.Where(x => x.Health < 50).ToArray();
					var damagedBuilding = damagedBuildings.ElementAt(Random.Next(0, damagedBuildings.Length));
					position = damagedBuilding.Position;
					break;

				case GameActions.BuyUpgrade:
					var availableUpgrades = _gameState.AvailableUpgrades.Where(x => x.Cost < _gameState.Funds).ToArray();
					var upgrade = availableUpgrades.ElementAt(Random.Next(0, availableUpgrades.Length));
					argument = upgrade.Name; 
					
					var completedBuildings = _gameState.GetCompletedBuildings().ToArray();
					var applicableBuildings = completedBuildings.Where(x => !x.Effects.Contains(upgrade.Name)).ToArray();
					if (applicableBuildings.Length < 1)
					{
						action = GameActions.Wait;
					}
					var buildingToUpgrade = applicableBuildings.ElementAt(Random.Next(0, applicableBuildings.Length));
					position = buildingToUpgrade.Position;
					break;
			}

			_gameLayer.ExecuteAction(action, position, argument);
		}
		
		public Position GetRandomBuildablePosition()
		{
			var positions = _gameState.GetBuildablePositions()
				.Where(_gameState.IsBuildablePosition)
				.ToArray();
			var position = positions.ElementAtOrDefault(Random.Next(0, positions.Length));
			return position;
		}

		public Position GetRandomBuildablePositionWithLotsOfSpace()
		{
			throw new NotImplementedException();
		}

		public Position GetRandomBuildablePositionNearResidence()
		{
			int? scoreR1 = null;
			int? scoreR2 = null;
			Position bestPosition = null;

			var builtBuildings = _gameState.GetBuiltBuildings().OfType<BuiltResidenceBuilding>().ToArray();
			foreach (var building in builtBuildings)
			{
				var adjacentPositions = building.Position.GetAdjacentPositions();
				foreach (var pos in adjacentPositions)
				{
					if (!_gameState.IsBuildablePosition(pos))
						continue;
					if (builtBuildings.Any(x => x.Position.ToString() == pos.ToString()))
						continue;

					var score = _gameState.CalculatePositionAvailability(pos, bonusForBuildings: false);
					if (scoreR1.HasValue && score < scoreR1.Value)
					{
						// Not better than previous R1
						continue;
					}
					
					if (scoreR1.HasValue && score == scoreR1.Value)
					{
						scoreR1 = score;
						score = _gameState.CalculatePositionAvailability(pos, radius: 2, bonusForBuildings: true);

						if (scoreR2.HasValue && score < scoreR2.Value)
						{
							// Not better than previous R2
							continue;
						}

						scoreR2 = score;
					}

					scoreR1 = score;
					bestPosition = pos;
				}
			}
			return bestPosition;
		}

		public GameActions GetRandomAction(Func<GameActions, bool> predicate = null)
		{
			var actions = _gameLayer.GetPossibleActions(includeWait: false).ToArray();
			if (predicate != null)
				actions = actions.Where(predicate).ToArray();

			var action = actions.ElementAtOrDefault(Random.Next(0, actions.Length));
			if (action == GameActions.None)
			{
				// Can't do anything else, then should Wait
				action = GameActions.Wait;
			}
			return action;
		}

		public string GetRandomBuildingName()
		{
			var names = _gameState.AvailableResidenceBuildings.Select(x => x.BuildingName).ToArray();
			var buildingName = names.ElementAt(Random.Next(0, names.Length));
			return buildingName;
		}
	}
}
