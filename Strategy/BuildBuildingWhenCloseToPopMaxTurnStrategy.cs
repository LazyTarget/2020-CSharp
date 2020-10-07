using System;
using System.Linq;
using DotNet.models;

namespace DotNet.Strategy
{
	public class BuildBuildingWhenCloseToPopMaxTurnStrategy : TurnStrategyBase
	{
		public BuildBuildingWhenCloseToPopMaxTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public string BuildingName { get; set; } = "Cabin";

		public double PopulationPercentageThreshold { get; set; } = 0.805;

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			// Only build when has nothing else to do
			var currentPop = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().Sum(x => x.CurrentPop);

			var currentPopMax = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>()
				.Join(state.AvailableResidenceBuildings, ok => ok.BuildingName, ik => ik.BuildingName,
					(rb, bp) => new {bp, rb})
				.Sum(x => x.bp.MaxPop);

			var pendingPopMaxIncrease = state.GetBuildingsUnderConstruction().OfType<BuiltResidenceBuilding>()
				.Join(state.AvailableResidenceBuildings, ok => ok.BuildingName, ik => ik.BuildingName,
					(rb, bp) => new {bp, rb})
				.Sum(x => x.bp.MaxPop);
			
			var currentPopPercentage = currentPop / (double)currentPopMax;


			Console.WriteLine($"BuildBuildingWhenCloseToPopMaxTurnStrategy :: Pop {currentPop}/{currentPopMax} = {currentPopPercentage:P2}		(+ {pendingPopMaxIncrease})");

			if (currentPopPercentage > PopulationPercentageThreshold)
			{
				var building = state.AvailableResidenceBuildings.Find(x => x.BuildingName == BuildingName);
				if (building.Cost < state.Funds)
				{
					var position = randomizer.GetRandomBuildablePosition();

					gameLayer.ExecuteAction(GameActions.StartBuild, position, building.BuildingName);
					return true;
				}
			}
			return false;
		}
	}
}