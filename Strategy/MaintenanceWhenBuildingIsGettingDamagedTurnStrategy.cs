using System.Linq;
using DotNet.models;

namespace DotNet.Strategy
{
	public class MaintenanceWhenBuildingIsGettingDamagedTurnStrategy : TurnStrategyBase
	{
		public MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public int ThresholdHealth { get; set; } = 50;

		public bool PrioritizeWeakest { get; set; } = false;

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			var damagedBuildings = state.ResidenceBuildings.Where(x => x.Health < ThresholdHealth)
				.OrderBy(x => x.Health)
				.ToArray();
			if (!damagedBuildings.Any())
				return false;

			var affordedBuildings = damagedBuildings.Where(x =>
			{
				var blueprint = state.AvailableResidenceBuildings.Single(p => p.BuildingName == x.BuildingName);
				var afforded = blueprint.MaintenanceCost < state.Funds;
				return afforded;
			}).ToArray();

			BuiltResidenceBuilding building;
			if (PrioritizeWeakest)
			{
				building = damagedBuildings.First();
				if (!affordedBuildings.Contains(building))
				{
					// Can't afford it, then skip hole strategy to save money for next time
					return false;
				}
			}
			else
			{
				// Can afford maintenance on the weakest building
				building = affordedBuildings.First();
			}

			
			var position = building.Position;
			gameLayer.ExecuteAction(GameActions.Maintenance, position);
			return true;
		}
	}
}