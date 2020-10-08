using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DotNet.Interfaces;
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

		public static IEnumerable<Position> GetAdjacentPositions(this Position position)
		{
			yield return new Position(position.x - 1, position.y - 1);
			yield return new Position(position.x, position.y - 1);
			yield return new Position(position.x + 1, position.y - 1);
			
			yield return new Position(position.x - 1, position.y);
			//yield return new Position(position.x, position.y);
			yield return new Position(position.x + 1, position.y);

			yield return new Position(position.x - 1, position.y + 1);
			yield return new Position(position.x, position.y + 1);
			yield return new Position(position.x +- 1, position.y + 1);
		}

		public static int CalculatePositionAvailability(this GameState state, Position position, int radius = 1)
		{
			var rating = 0;
			var buildings = state.GetBuiltBuildings().ToArray();
			for (var i = position.x - radius; i < position.x + radius; i++)
			{
				if (i < 0)
					continue;
				if (i >= state.Map.Length)
					continue;

				for (var j = position.y - radius; j < position.y + radius; j++)
				{
					if (j < 0)
						continue;
					if (i >= state.Map[i].Length)
						continue;
					if (i == position.x && j == position.y)
						continue;

					var blockedByTerrain = state.Map[i][j] != 0;
					if (blockedByTerrain)
						continue;

					var blockedByBuilding = buildings.Any(x => x.Position.ToString() == position.ToString());
					if (blockedByBuilding)
					{
						// Bonus points for adjacent buildings
						rating++;
					}

					rating++;
				}
			}
			return rating;
		}

		public static void ExecuteAction(this IGameLayer gameLayer, GameActions action)
		{
			if (action == GameActions.Wait)
				ExecuteAction(gameLayer, action, position: null);
			else
				throw new InvalidOperationException($"Only 'GameActions.Wait' can be invoked without a position");
		}

		public static void ExecuteAction(this IGameLayer gameLayer, GameActions action, Position position, object argument = null)
		{
			var state = gameLayer.GetState();
			switch (action)
			{
				case GameActions.StartBuild:
					var buildingName = (string) argument ?? throw new ArgumentNullException(nameof(argument));
					gameLayer.StartBuild(position, buildingName, state.GameId);
					break;

				case GameActions.Build:
					gameLayer.Build(position, state.GameId);
					break;

				case GameActions.Maintenance:
					gameLayer.Maintenance(position, state.GameId);
					break;

				case GameActions.BuyUpgrade:
					var upgradeName = (string) argument ?? throw new ArgumentNullException(nameof(argument));
					gameLayer.BuyUpgrade(position, upgradeName, state.GameId);
					break;



				case GameActions.Wait:
					gameLayer.Wait(state.GameId);
					break;
					
				case GameActions.None:
					throw new NotSupportedException();

				default:
					throw new NotImplementedException();
			}
		}

		public static IEnumerable<GameActions> GetPossibleActions(this IGameLayer gameLayer, bool includeWait = true)
		{
			var state = gameLayer.GetState();

			// If has money availble for a building, then can start building
			var blueprints = state.GetBuildingBlueprints().ToArray();
			if (blueprints.Any(x => x.Cost <= state.Funds))
			{
				// Only build if will have plenty of money left...

				var fraction = state.Funds / blueprints.Average(x => x.Cost);
				if (fraction >= 2)
				{
					yield return GameActions.StartBuild;
				}
				else
				{

				}
			}

			// If has non-completed buildings, then can build
			if (state.GetBuildingsUnderConstruction().Any(x => x.BuildProgress < 100))
				yield return GameActions.Build;

			// If has completed buildings and they are low on hp, then can do maintenance
			var damagedBuildings = state.ResidenceBuildings.Where(x => x.Health < 50).ToArray();
			if (damagedBuildings.Any())
			{
				var buildingTypes = damagedBuildings.Select(x => x.BuildingName).Distinct().ToArray();
				var affordedMaintenance = state.AvailableResidenceBuildings
					.Where(x => buildingTypes.Contains(x.BuildingName))
					.Where(x => x.MaintenanceCost < state.Funds)
					.ToArray();
				if (affordedMaintenance.Any())
					yield return GameActions.Maintenance;
			}

			// If has completed buildings and they are low on hp, then can do maintenance
			if (state.GetCompletedBuildings().Any())
			{
				if (state.AvailableUpgrades.Any(x => x.Cost < state.Funds))
				{
					var fraction = state.Funds / state.AvailableUpgrades.OrderByDescending(x => x.Cost).First().Cost;
					if (fraction >= 6)
					{
						// todo: enable when logic is more stable
						//yield return GameActions.BuyUpgrade;
					}
					else
					{

					}
				}
			}

			if (includeWait)
				yield return GameActions.Wait;
		}
		
		public static IEnumerable<BlueprintBuilding> GetBuildingBlueprints(this GameState state)
		{
			var buildings = state.AvailableResidenceBuildings.OfType<BlueprintBuilding>().Concat(state.AvailableUtilityBuildings);
			return buildings;
		}

		public static IEnumerable<BuiltBuilding> GetBuiltBuildings(this GameStateResponse state)
		{
			var buildings = state.ResidenceBuildings.OfType<BuiltBuilding>().Concat(state.UtilityBuildings);
			return buildings;
		}

		public static IEnumerable<BuiltBuilding> GetBuildingsUnderConstruction(this GameStateResponse state)
		{
			var buildings = state.GetBuiltBuildings().Where(x => x.BuildProgress < 100);
			return buildings;
		}

		public static IEnumerable<BuiltBuilding> GetCompletedBuildings(this GameStateResponse state)
		{
			var buildings = state.GetBuiltBuildings().Where(x => x.BuildProgress >= 100);
			return buildings;
		}
	}
}
