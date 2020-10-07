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
				case GameActions.StartBuild:
					var buildingName = (string) argument ?? throw new ArgumentNullException(nameof(argument));
					gameLayer.StartBuild(position, buildingName);
					break;

				case GameActions.Build:
					gameLayer.Build(position);
					break;

				case GameActions.Wait:
					gameLayer.Wait();
					break;


				case GameActions.None:
					throw new NotSupportedException();

				default:
					throw new NotImplementedException();
			}
		}

		public static IEnumerable<GameActions> GetPossibleActions(this GameLayer gameLayer, bool includeWait = true)
		{
			var state = gameLayer.GetState();

			// If has money availble for a building, then can start building
			if (state.GetBuildingBlueprints().Any(x => x.Cost <= state.Funds))
				yield return GameActions.StartBuild;

			// If has non-completed buildings, then can build
			if (state.ResidenceBuildings.Any(x => x.BuildProgress < 100))
				yield return GameActions.Build;

			if (includeWait)
				yield return GameActions.Wait;
		}
		
		public static IEnumerable<BlueprintBuilding> GetBuildingBlueprints(this GameState state)
		{
			var buildings = state.AvailableResidenceBuildings.OfType<BlueprintBuilding>().Concat(state.AvailableUtilityBuildings);
			return buildings;
		}

		public static IEnumerable<BuiltBuilding> GetBuiltBuildings(this GameState state)
		{
			var buildings = state.ResidenceBuildings.OfType<BuiltBuilding>().Concat(state.UtilityBuildings);
			return buildings;
		}

		public static IEnumerable<BuiltBuilding> GetBuildingsUnderConstruction(this GameState state)
		{
			var buildings = state.GetBuiltBuildings().Where(x => x.BuildProgress < 100);
			return buildings;
		}
	}
}
