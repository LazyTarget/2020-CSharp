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
		private readonly Random _random = new Random();

		public Randomizer(GameLayer gameLayer)
		{
			_gameLayer = gameLayer;
		}
		
		public void RandomizeAction()
		{
			var action = GetRandomAction();
			var position = GetRandomPosition();

			_gameLayer.ExecuteAction(action, position);
		}
		
		public Position GetRandomPosition()
		{
			var state = _gameLayer.GetState();
			var positions = state.GetBuildablePositions().ToArray();
			var position = positions.ElementAt(_random.Next(0, positions.Length));
			return position;
		}

		public GameActions GetRandomAction()
		{
			var actions = _gameLayer.GetPossibleActions().ToArray();
			var action = actions.ElementAt(_random.Next(0, actions.Length));
			return action;
		}
	}
}
