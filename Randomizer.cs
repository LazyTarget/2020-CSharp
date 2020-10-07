using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.models;

namespace DotNet
{
	public class Randomizer
	{
		private readonly GameLayer _gameLayer;
		private readonly GameState _gameState;
		private readonly Random _random = new Random();

		public Randomizer(GameLayer gameLayer)
		{
			_gameLayer = gameLayer;
			_gameState = gameLayer.GetState();
		}

		public void HandleTurn(int turn)
		{
			if (turn < 5)
			{
				// Prioritize on building in the beginning...
				HandleAction(GameActions.StartBuild);
			}
			else
			{
				RandomizeAction();
			}
		}

		public void RandomizeAction()
		{
			var action = GetRandomAction();
			HandleAction(action);
		}

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
					var building = buildingsUnderConstruction.ElementAt(_random.Next(0, buildingsUnderConstruction.Length));
					position = building.Position;
					break;

				case GameActions.Maintenance:
					var damagedBuildings = _gameState.ResidenceBuildings.Where(x => x.Health < 45).ToArray();
					var damagedBuilding = damagedBuildings.ElementAt(_random.Next(0, damagedBuildings.Length));
					position = damagedBuilding.Position;
					break;

				case GameActions.BuyUpgrade:
					var availableUpgrades = _gameState.AvailableUpgrades.Where(x => x.Cost < _gameState.Funds).ToArray();
					var upgrade = availableUpgrades.ElementAt(_random.Next(0, availableUpgrades.Length));
					argument = upgrade.Name; 
					
					var completedBuildings = _gameState.GetCompletedBuildings().ToArray();
					var applicableBuildings = completedBuildings.Where(x => !x.Effects.Contains(upgrade.Name)).ToArray();
					if (applicableBuildings.Length < 1)
					{
						action = GameActions.Wait;
					}
					var buildingToUpgrade = applicableBuildings.ElementAt(_random.Next(0, applicableBuildings.Length));
					position = buildingToUpgrade.Position;
					break;
			}

			_gameLayer.ExecuteAction(action, position, argument);
		}
		
		public Position GetRandomBuildablePosition()
		{
			var positions = _gameState.GetBuildablePositions().ToArray();
			var position = positions.ElementAt(_random.Next(0, positions.Length));
			return position;
		}

		public GameActions GetRandomAction()
		{
			var actions = _gameLayer.GetPossibleActions(includeWait: false).ToArray();
			var action = actions.ElementAtOrDefault(_random.Next(0, actions.Length));
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
			var buildingName = names.ElementAt(_random.Next(0, names.Length));
			return buildingName;
		}
	}
}
