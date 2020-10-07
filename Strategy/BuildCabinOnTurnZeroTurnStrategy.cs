namespace DotNet.Strategy
{
	public class BuildCabinOnTurnZeroTurnStrategy : TurnStrategyBase
	{
		public BuildCabinOnTurnZeroTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			if (state.Turn != 0)
				return false;

			var position = randomizer.GetRandomBuildablePosition();
			gameLayer.ExecuteAction(GameActions.StartBuild, position, "Cabin");
			return true;
		}
	}
}