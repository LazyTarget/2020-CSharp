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

		public void RandomizeTurn()
		{
			RandomizeAction();
		}

		public void RandomizeAction()
		{
			object argument = null;
			var action = GetRandomAction();
			var position = GetRandomPosition();
			switch (action)
			{
				case GameActions.StartBuild:
					argument = GetRandomBuildingName();
					break;
			}

			_gameLayer.ExecuteAction(action, position, argument);
		}
		
		public Position GetRandomPosition()
		{
			var positions = _gameState.GetBuildablePositions().ToArray();
			var position = positions.ElementAt(_random.Next(0, positions.Length));
			return position;
		}

		public GameActions GetRandomAction()
		{
			var actions = _gameLayer.GetPossibleActions().ToArray();
			var action = actions.ElementAt(_random.Next(0, actions.Length));
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
