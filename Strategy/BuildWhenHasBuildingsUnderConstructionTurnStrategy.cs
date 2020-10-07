using System.Linq;

namespace DotNet.Strategy
{
	public class BuildWhenHasBuildingsUnderConstructionTurnStrategy : TurnStrategyBase
	{
		public BuildWhenHasBuildingsUnderConstructionTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

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
				return true;
			}
			return false;
		}
	}
}