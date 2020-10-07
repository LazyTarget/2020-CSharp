using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotNet.models;

namespace DotNet.Strategy
{
	public class AdjustBuildingTemperatureWhenTooLowTurnStrategy : TurnStrategyBase
	{
		public AdjustBuildingTemperatureWhenTooLowTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public double MinTemperature { get; set; } = 18;

		public double AllowedDiffMargin { get; set; } = 0.5;

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			var buildings = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().ToArray();
			var needsAdjustment = buildings
				.Where(x => x.Temperature < MinTemperature)
				.OrderBy(x => x.Temperature)
				.ToArray();

			var building = needsAdjustment.FirstOrDefault();
			if (building != null)
			{
				var blueprint = gameLayer.GetResidenceBlueprint(building.BuildingName);

				var energy = blueprint.BaseEnergyNeed + (building.Temperature - state.CurrentTemp)
					* blueprint.Emissivity / 1 + 0.5 - building.CurrentPop * 0.04;

				if (IsBetween(energy, 
					building.RequestedEnergyIn - AllowedDiffMargin,
					building.RequestedEnergyIn + AllowedDiffMargin))
				{
					// already updated energy
					return false;
				}
				
				gameLayer.AdjustEnergy(building.Position, energy);
				return true;
			}

			return false;
		}

		public static bool IsBetween(double num, double lower, double upper, bool inclusive = false)
		{
			return inclusive
				? lower <= num && num <= upper
				: lower < num && num < upper;
		}
	}
}
