using System;
using DotNet.Strategy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotNet.Tests
{
	[TestClass]
	public abstract class RandomizerTests
	{
		private string ApiKey;
		protected virtual string Map { get; }


		[TestInitialize]
		public void Initialize()
		{

		}

		public GameRunner InitRunner()
		{
			if (string.IsNullOrWhiteSpace(Map))
				throw new ArgumentNullException(nameof(Map));

			return GameRunner.New(ApiKey, Map);
		}

		[TestMethod]
		public void DefaultStrategy()
		{
			var runner = InitRunner();
			var score = runner.Run();
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_Apartments()
		{
			TurnStrategyBase strategy = null;
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Apartments",
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_ModernApartments()
		{
			TurnStrategyBase strategy = null;
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "ModernApartments",
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_Cabin()
		{
			TurnStrategyBase strategy = null;
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_EnvironmentalHouse()
		{
			TurnStrategyBase strategy = null;
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "EnvironmentalHouse",
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_HighRise()
		{
			TurnStrategyBase strategy = null;
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "HighRise",
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_LuxuryResidence()
		{
			TurnStrategyBase strategy = null;
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = "LuxuryResidence",
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestClass]
		public class training1Map : RandomizerTests
		{
			protected override string Map { get; } = "training1";
		}
	}
}
