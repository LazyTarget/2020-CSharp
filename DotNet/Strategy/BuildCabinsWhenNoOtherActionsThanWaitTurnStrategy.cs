using DotNet.Interfaces;

namespace DotNet.Strategy
{
	public class BuildCabinsWhenNoOtherActionsThanWaitTurnStrategy : TurnStrategyBase
	{
		public BuildCabinsWhenNoOtherActionsThanWaitTurnStrategy() : base()
		{
		}

		public BuildCabinsWhenNoOtherActionsThanWaitTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
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
					gameLayer.StartBuild(position, "Cabin", state.GameId);
					return true;
				}
			}
			return false;
		}
	}
}