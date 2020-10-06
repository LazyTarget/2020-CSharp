using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.models;

namespace DotNet
{
	public static partial class GameLayerExtensions
	{
		private static readonly Random Random = new Random();
		
		public static void RandomizeAction(this GameLayer gameLayer)
		{
			var actions = gameLayer.GetPossibleActions().ToArray();
			var action = actions.ElementAt(Random.Next(0, actions.Length));
			
			gameLayer.ExecuteAction(action);
		}

		public static IEnumerable<Position> GetRandomBuildablePosition(this GameState state)
		{
			for (var i = 0; i < 10; i++)
			{
				for (var j = 0; j < 10; j++)
				{
					if (state.Map[i][j] == 0)
					{
						yield return new Position(i, j);
						break;
					}
				}
			}
		}

		public static void ExecuteAction(this GameLayer gameLayer, GameActions action)
		{
			switch (action)
			{
				case GameActions.Build:
					throw new NotImplementedException();
					break;

				default:
					throw new NotImplementedException();
			}
		}

		public static IEnumerable<GameActions> GetPossibleActions(this GameLayer gameLayer)
		{
			yield return GameActions.Build;
		}
	}

	public enum GameActions
	{
		Build,
	}
}
