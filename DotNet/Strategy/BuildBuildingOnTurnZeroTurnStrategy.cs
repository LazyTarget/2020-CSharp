using System;
using DotNet.Interfaces;

namespace DotNet.Strategy
{
	public class BuildBuildingOnTurnZeroTurnStrategy : TurnStrategyBase
	{
		public BuildBuildingOnTurnZeroTurnStrategy() : base()
		{
		}

		public BuildBuildingOnTurnZeroTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public string BuildingName { get; set; } = "Cabin";

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			if (state.Turn != 0)
				return false;

			var position = randomizer.GetRandomBuildablePosition();
			if (position == null)
			{
				Console.WriteLine("No valid positions to build building");
				return false;
			}

			gameLayer.StartBuild(position, BuildingName, state.GameId);
			return true;
		}
	}
}