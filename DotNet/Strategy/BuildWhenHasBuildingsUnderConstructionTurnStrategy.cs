using System.Linq;
using DotNet.Interfaces;
using DotNet.models;

namespace DotNet.Strategy
{
	public class BuildWhenHasBuildingsUnderConstructionTurnStrategy : TurnStrategyBase
	{
		public BuildWhenHasBuildingsUnderConstructionTurnStrategy() : base()
		{
		}

		public BuildWhenHasBuildingsUnderConstructionTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public bool AdjustHeatOnConstructed { get; set; } = true;

		public double AllowedDiffMargin { get; set; } = 0.5;

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
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
						var blueprint = state.AvailableResidenceBuildings.Find(x => x.BuildingName == building.BuildingName);

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
							gameLayer.AdjustEnergy(building.Position, energy, state.GameId);
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