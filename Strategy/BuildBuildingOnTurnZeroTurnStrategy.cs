using DotNet.Interfaces;

namespace DotNet.Strategy
{
	public class BuildBuildingOnTurnZeroTurnStrategy : TurnStrategyBase
	{
		public BuildBuildingOnTurnZeroTurnStrategy(TurnStrategyBase parent = null) : base(parent)
		{
		}

		public string BuildingName { get; set; } = "Cabin";

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			if (state.Turn != 0)
				return false;

			var position = randomizer.GetRandomBuildablePosition();
			gameLayer.StartBuild(position, BuildingName, state.GameId);
			return true;
		}
	}
}