using System.Linq;

namespace DotNet.Strategy
{
	public class MaintenanceWhenBuildingIsGettingDamagedTurnStrategy : TurnStrategyBase
	{
		public MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public int ThresholdHealth { get; set; } = 50;

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			var damagedBuildings = state.ResidenceBuildings.Where(x => x.Health < ThresholdHealth).ToArray();

			// Randomly choose building
			//var building = damagedBuildings.ElementAtOrDefault(randomizer.Random.Next(0, damagedBuildings.Length));

			// Choose the one with least health
			var building = damagedBuildings.OrderBy(x => x.Health).FirstOrDefault();


			if (building != null)
			{
				var position = building.Position;
				gameLayer.ExecuteAction(GameActions.Maintenance, position);
				return true;
			}
			return false;
		}
	}
}