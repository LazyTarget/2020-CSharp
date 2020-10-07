using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNet.models;

namespace DotNet.Strategy
{
	public class AdjustBuildingTemperatureWhenTooHighTurnStrategy : TurnStrategyBase
	{
		public AdjustBuildingTemperatureWhenTooHighTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public double MaxTemperature { get; set; } = 24;

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			var buildings = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().ToArray();
			var needsAdjustment = buildings
				.Where(x => x.Temperature > MaxTemperature)
				.OrderByDescending(x => x.Temperature)
				.ToArray();

			var building = needsAdjustment.FirstOrDefault();
			if (building != null)
			{
				var blueprint = gameLayer.GetResidenceBlueprint(building.BuildingName);

				var energy = blueprint.BaseEnergyNeed + (building.Temperature - state.CurrentTemp)
					* blueprint.Emissivity / 1 - 0.5 - building.CurrentPop * 0.04;

				gameLayer.AdjustEnergy(building.Position, energy);
				return true;
			}

			return false;
		}
	}
}
