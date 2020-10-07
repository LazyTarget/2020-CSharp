namespace DotNet.Strategy
{
	public class BuildCabinsWhenNoOtherActionsThanWaitTurnStrategy : TurnStrategyBase
	{
		public BuildCabinsWhenNoOtherActionsThanWaitTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		protected override bool TryExecuteTurn(Randomizer randomizer, GameLayer gameLayer, GameState state)
		{
			// Only build when has nothing else to do
			var action = randomizer.GetRandomAction(x => x != GameActions.StartBuild);
			if (action == GameActions.Wait)
			{
				action = randomizer.GetRandomAction();
				if (action == GameActions.StartBuild)
				{
					// Prioritize building Cabins
					var position = randomizer.GetRandomBuildablePosition();
					gameLayer.ExecuteAction(GameActions.StartBuild, position, "Cabin");
					return true;
				}
			}
			return false;
		}
	}
}