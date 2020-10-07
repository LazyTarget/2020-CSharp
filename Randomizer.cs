using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.models;

namespace DotNet
{
	public class Randomizer
	{
		public static readonly Randomizer Instance = new Randomizer();

		private readonly Random _random = new Random();
		
		public void RandomizeAction(GameLayer gameLayer)
		{
			var actions = gameLayer.GetPossibleActions().ToArray();
			var action = actions.ElementAt(_random.Next(0, actions.Length));

			Position position = null;
			gameLayer.ExecuteAction(action, position);
		}
	}
}
