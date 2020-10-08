using System;
using System.Diagnostics;
using System.Linq;
using DotNet.Interfaces;
using DotNet.models;

namespace DotNet.Strategy
{
	public class BuildBuildingOnTurnZeroTurnStrategy : TurnStrategyBase
	{
		public BuildBuildingOnTurnZeroTurnStrategy() : base()
		{
		}

		public BuildBuildingOnTurnZeroTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public string BuildingName { get; set; }

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			if (state.Turn != 0)
				return false;

			var position = randomizer.GetRandomBuildablePosition();
			if (position == null)
			{
				Debug.WriteLine("No valid positions to build building");
				return false;
			}


			BlueprintResidenceBuilding building;
			var buildingName = BuildingName;
			if (!string.IsNullOrWhiteSpace(buildingName))
			{
				building = state.AvailableResidenceBuildings.Find(x => x.BuildingName == buildingName);
			}
			else
			{
				var affordableBuildings = state.AvailableResidenceBuildings
					.Where(x => x.Cost <= state.Funds)
					.ToArray();
				building = affordableBuildings.ElementAtOrDefault(randomizer.Random.Next(0, affordableBuildings.Length));
			}

			if (building == null)
			{
				// No valid building
				return false;
			}

			if (building.Cost > state.Funds)
			{
				// Cannot afford it
				return false;
			}

			gameLayer.StartBuild(position, building.BuildingName, state.GameId);
			return true;
		}
	}
}