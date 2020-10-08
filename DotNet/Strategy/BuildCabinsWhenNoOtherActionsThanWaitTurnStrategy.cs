﻿using System;
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
					var building = state.AvailableResidenceBuildings.Find(x => x.BuildingName == "Cabin");
					if (building.Cost > state.Funds)
					{
						Console.WriteLine("Wanted to build building, but cannot afford it");
						return false;
					}

					// Prioritize building Cabins
					var position = randomizer.GetRandomBuildablePosition();
					if (position == null)
					{
						Console.WriteLine("No valid positions to build building");
						return false;
					}

					gameLayer.StartBuild(position, building.BuildingName, state.GameId);
					return true;
				}
			}
			return false;
		}
	}
}