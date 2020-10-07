using System;
using System.Linq;
using DotNet.models;

namespace DotNet.Strategy
{
	public class BuildCabinsWhenCloseToPopMaxTurnStrategy : TurnStrategyBase
	{
		public BuildCabinsWhenCloseToPopMaxTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public double PopulationPercentageThreshold { get; set; } = 0.8;

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


			Console.WriteLine("BuildCabinsWhenCloseToPopMaxTurnStrategy::");
			Console.WriteLine($"CurrentPop = {currentPop}");
			Console.WriteLine($"CurrentPopMax = {currentPopMax}");
			Console.WriteLine($"CurrentPopPercentage = {currentPopPercentage}");
			Console.WriteLine($"PendingPopMaxIncrease = {pendingPopMaxIncrease}");

			if (currentPopPercentage > PopulationPercentageThreshold)
			{
				var building = state.AvailableResidenceBuildings.Find(x => x.BuildingName == "Cabin");
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