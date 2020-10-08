using System;
using System.Diagnostics;
using System.Linq;
using DotNet.Interfaces;
using DotNet.models;
using Microsoft.Extensions.Logging;

namespace DotNet.Strategy
{
	public class BuildUtilityCloseToResidencesTurnStrategy : TurnStrategyBase
	{
		public BuildUtilityCloseToResidencesTurnStrategy() : base()
		{
		}

		public BuildUtilityCloseToResidencesTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public string BuildingName { get; set; } = "Park";

		public int MaxNumberOfBuildings { get; set; } = 2;

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			BlueprintUtilityBuilding building;
			var buildingName = BuildingName;
			if (!string.IsNullOrWhiteSpace(buildingName))
			{
				building = state.AvailableUtilityBuildings.Find(x => x.BuildingName == buildingName);
			}
			else
			{
				var affordableBuildings = state.AvailableUtilityBuildings
					.Where(x => x.Cost <= state.Funds)
					.ToArray();
				building = affordableBuildings.ElementAtOrDefault(randomizer.Random.Next(0, affordableBuildings.Length));
			}

			if (building == null)
			{
				// No valid building
				return false;
			}

			var buildings = state.GetBuiltBuildings().Where(x => x.BuildingName == building.BuildingName).ToArray();
			if (buildings.Length >= MaxNumberOfBuildings)
			{
				// Don't build any more buildings
				return false;
			}
			if (buildings.Any(x => x.BuildProgress < 100))
			{
				// Already one in progress
				return false;
			}

			if (building.Cost > state.Funds)
			{
				// Cannot afford it
				Logger.LogWarning("Wanted to build building, but cannot afford it");
				return false;
			}


			var position = randomizer.GetRandomBuildablePositionNearResidence();
			if (position == null || !state.IsBuildablePosition(position))
			{
				Logger.LogWarning("No valid positions to build utility building");
				return false;
			}

			gameLayer.StartBuild(position, building.BuildingName, state.GameId);
			return true;
		}
	}
}