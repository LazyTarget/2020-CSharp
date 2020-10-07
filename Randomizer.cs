using System;
using System.Linq;
using DotNet.models;
using DotNet.Strategy;

namespace DotNet
{
	public class Randomizer
	{
		public readonly Random Random = new Random();

		private readonly GameLayer _gameLayer;
		private readonly GameState _gameState;
		private readonly TurnStrategyBase _strategy;

		public Randomizer(GameLayer gameLayer)
		{
			_gameLayer = gameLayer;
			_gameState = gameLayer.GetState();

			//_strategy = new BuildCabinsWhenNoOtherActionsThanWaitTurnStrategy(_strategy);
			_strategy = new BuildCabinsWhenCloseToPopMaxTurnStrategy(_strategy);
			_strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(_strategy)
			{
				ThresholdHealth = 50,
			};
			_strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(_strategy);
			_strategy = new BuildCabinOnTurnZeroTurnStrategy(_strategy);
		}

		public void HandleTurn()
		{
			if (_strategy != null)
			{
				var executed = _strategy.TryExecuteStrategy(this, _gameLayer, _gameState);
				if (executed)
					return;
			}

			var action = GetRandomAction();
			HandleAction(action);
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
			var positions = _gameState.GetBuildablePositions().ToArray();
			var position = positions.ElementAt(Random.Next(0, positions.Length));
			return position;
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
