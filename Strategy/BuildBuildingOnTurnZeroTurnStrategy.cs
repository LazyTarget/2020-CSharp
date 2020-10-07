namespace DotNet.Strategy
{
	public class BuildBuildingOnTurnZeroTurnStrategy : TurnStrategyBase
	{
		public BuildBuildingOnTurnZeroTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public string BuildingName { get; set; } = "Cabin";

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			if (state.Turn != 0)
				return false;

			var position = randomizer.GetRandomBuildablePosition();
			gameLayer.ExecuteAction(GameActions.StartBuild, position, BuildingName);
			return true;
		}
	}
}