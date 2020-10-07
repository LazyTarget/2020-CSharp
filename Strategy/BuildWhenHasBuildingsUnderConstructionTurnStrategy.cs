using System.Linq;
using DotNet.models;

namespace DotNet.Strategy
{
	public class BuildWhenHasBuildingsUnderConstructionTurnStrategy : TurnStrategyBase
	{
		public BuildWhenHasBuildingsUnderConstructionTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public bool AdjustHeatOnConstructed { get; set; } = true;

		public double AllowedDiffMargin { get; set; } = 0.5;

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			var buildingsUnderConstruction = state.GetBuildingsUnderConstruction().ToArray();

			// Randomly choose building
			//var building = buildingsUnderConstruction.ElementAt(randomizer.Random.Next(0, buildingsUnderConstruction.Length));

			// Choose the one closest to completion
			var building = buildingsUnderConstruction.OrderByDescending(x => x.BuildProgress).FirstOrDefault();


			if (building != null)
			{
				var position = building.Position;
				gameLayer.ExecuteAction(GameActions.Build, position);

				building = state.GetCompletedBuildings().FirstOrDefault(x => x.Position.ToString() == position.ToString());

				if (building is BuiltResidenceBuilding residence)
				{
					if (AdjustHeatOnConstructed)
					{
						var blueprint = gameLayer.GetResidenceBlueprint(building.BuildingName);

						var energy = blueprint.BaseEnergyNeed + (residence.Temperature - state.CurrentTemp)
							* blueprint.Emissivity / 1 - 0.5 - residence.CurrentPop * 0.04;

						if (IsBetween(energy,
							residence.RequestedEnergyIn - AllowedDiffMargin,
							residence.RequestedEnergyIn + AllowedDiffMargin))
						{
							// current energy setting is sufficient
						}
						else
						{
							// adjust energy as the first action after completing building (will take one turn)
							gameLayer.AdjustEnergy(building.Position, energy);
						}
					}
				}
				else
				{

				}
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