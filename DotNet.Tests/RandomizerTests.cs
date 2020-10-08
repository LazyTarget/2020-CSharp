using System;
using DotNet.Strategy;
using Microsoft.Extensions.Configuration;
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
			var configuration = new ConfigurationBuilder()
				.AddJsonFile("appsettings.json", true, true)
				.AddEnvironmentVariables("CONSIDITION_")
				.AddUserSecrets<RandomizerTests>(true)
				.Build();
			ApiKey = configuration.GetValue<string>("ApiKey");
			if (string.IsNullOrWhiteSpace(ApiKey))
				throw new ArgumentNullException(nameof(ApiKey));
		}

		public GameRunner InitRunner()
		{
			if (string.IsNullOrWhiteSpace(Map))
				throw new ArgumentNullException(nameof(Map));
			try
			{
				var runner = GameRunner.New(ApiKey, Map);
				return runner;
			}
			catch (Exception ex)
			{
				throw;
			}
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
			var buildingName = "Apartments";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_ModernApartments()
		{
			var buildingName = "ModernApartments";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_Cabin()
		{
			var buildingName = "Cabin";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_EnvironmentalHouse()
		{
			var buildingName = "EnvironmentalHouse";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_HighRise()
		{
			var buildingName = "HighRise";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};

			var runner = InitRunner();
			var score = runner.Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_LuxuryResidence()
		{
			var buildingName = "LuxuryResidence";

			TurnStrategyBase strategy = null;
			strategy = new BuildBuildingWhenCloseToPopMaxTurnStrategy(strategy)
			{
				BuildingName = buildingName,
			};
			strategy = new MaintenanceWhenBuildingIsGettingDamagedTurnStrategy(strategy);
			strategy = new BuildWhenHasBuildingsUnderConstructionTurnStrategy(strategy);
			strategy = new AdjustBuildingTemperaturesTurnStrategy(strategy);
			strategy = new BuildBuildingOnTurnZeroTurnStrategy(strategy)
			{
				BuildingName = buildingName,
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
