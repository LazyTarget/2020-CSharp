using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.models;

namespace DotNet
{
	public static class GameLayerExtensions
	{
		public static IEnumerable<Position> GetBuildablePositions(this GameState state)
		{
			for (var i = 0; i < state.Map.Length; i++)
			{
				for (var j = 0; j < state.Map[i].Length; j++)
				{
					if (state.Map[i][j] != 0)
						continue;

					var position = new Position(i, j);

					if (state.ResidenceBuildings.Any(x => x.Position.ToString() == position.ToString()))
						continue;

					yield return position;
				}
			}
		}

		public static void ExecuteAction(this GameLayer gameLayer, GameActions action, Position position, object argument = null)
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
}
