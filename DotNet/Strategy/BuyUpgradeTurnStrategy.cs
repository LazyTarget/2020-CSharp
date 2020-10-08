using System;
using System.Linq;
using DotNet.Interfaces;
using DotNet.models;

namespace DotNet.Strategy
{
	public class BuyUpgradeTurnStrategy : TurnStrategyBase
	{
		public BuyUpgradeTurnStrategy() : base()
		{
		}

		public BuyUpgradeTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public string[] IncludedUpgrades { get; set; } = null;

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			var upgrades = state.AvailableUpgrades;
			if (!upgrades.Any())
			{
				return false;
			}

			var completedBuildings = state.GetCompletedBuildings().OfType<BuiltResidenceBuilding>().ToArray();
			if (!completedBuildings.Any())
			{
				return false;
			}


			var allowedUpgrades = IncludedUpgrades != null
				? upgrades.Where(x => IncludedUpgrades.Contains(x.Name)).ToArray()
				: upgrades.ToArray();
			var affordedUpgrades = allowedUpgrades.Where(x => x.Cost < state.Funds).ToArray();
			var upgrade = affordedUpgrades.ElementAtOrDefault(randomizer.Random.Next(0, affordedUpgrades.Length));
			if (upgrade == null)
			{
				// No allowed upgrades
				return false;
			}

			var applicableBuildings = completedBuildings.Where(x => !x.Effects.Contains(upgrade.Name)).ToArray();
			var building = applicableBuildings.ElementAtOrDefault(randomizer.Random.Next(0, applicableBuildings.Length));
			if (building == null)
			{
				// No applicable buildings for chosen upgrade
				return false;
			}

			var position = building.Position;
			gameLayer.BuyUpgrade(position, upgrade.Name, state.GameId);
			return true;
		}
	}
}