using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DotNet.Logging;
using DotNet.models;
using DotNet.Strategy;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Debug;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[assembly: Parallelize(Workers = 4, Scope = ExecutionScope.MethodLevel)]

namespace DotNet.Tests
{
	[TestClass]
	public abstract class GameTests
	{
		private string ApiKey = AssemblySetup.ApiKey;
		protected virtual string Map { get; set; }

		private GameRunner _runner;
		private StringBuilder _output;

		private readonly ILoggerFactory _loggerFactory;
		private readonly ILogger _logger;


		protected GameTests()
		{
			var loggerFilterOptions = new LoggerFilterOptions
			{
				MinLevel = LogLevel.Information,
			};
			var providers = new List<ILoggerProvider>();
			if (Debugger.IsAttached)
			{
				providers.Add(new DebugLoggerProvider());
				loggerFilterOptions.MinLevel = LogLevel.Trace;
			}
			else
			{
				_output = new StringBuilder();
				providers.Add(new InMemoryLoggerProvider(_output));
			}
			_loggerFactory = new LoggerFactory(providers, loggerFilterOptions);
			_logger = _loggerFactory.CreateLogger(GetType());
		}

		[TestInitialize]
		public void Initialize()
		{
			TestInitialize();
		}

		protected virtual void TestInitialize() { }

		[TestCleanup]
		public void Cleanup()
		{
			_runner?.EndGame();

			var fullLog = _output?.ToString();
			if (!string.IsNullOrWhiteSpace(fullLog))
			{
				Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger.LogMessage(fullLog);
				//Debug.WriteLine(fullLog);
			}
		}

		protected virtual GameRunner GetRunner(string map = null)
		{
			if (_runner != null)
				return _runner;

			map ??= Map;
			if (string.IsNullOrWhiteSpace(map))
			{
				Assert.Inconclusive("Missing Map-argument!");
			}
			try
			{
				_logger.LogInformation("Initializing runner");
				var runner = GameRunner.New(ApiKey, map, _loggerFactory);
				_runner = runner;
				return runner;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Exception when initializing runner: {ex}");
				throw;
			}
		}

		protected abstract ScoreResponse Run(string map);

		protected virtual void AssertScore(ScoreResponse score)
		{
			Assert.IsTrue(score.FinalScore > 0, $"Run failed to get a positive score");
			Assert.IsTrue(score.FinalPopulation > 0, $"Run failed to maintain population in the city");
		}


		[TestClass]
		public abstract class StrategyTests : GameTests
		{
			protected abstract TurnStrategyBase GetStrategy();

			protected override ScoreResponse Run(string map)
			{
				try
				{
					var strategy = GetStrategy();
					var runner = GetRunner(map);
					var score = runner.Run(strategy);
					return score;
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, $"Exception when running: {ex}");
					throw;
				}
			}

			[TestMethod]
			[TestCategory("Map_training1")]
			public virtual void Map_training1()
			{
				Map = "training1";
				var score = Run(Map);
				AssertScore(score);
			}

			[TestMethod]
			[TestCategory("Map_training2")]
			public virtual void Map_training2()
			{
				Map = "training2";
				var score = Run(Map);
				AssertScore(score);
			}

			[TestMethod]
			[TestCategory("Map_Gothenburg")]
			public virtual void Map_Gothenburg()
			{
				Map = "Gothenburg";
				var score = Run(Map);
				AssertScore(score);
			}

			[TestMethod]
			[TestCategory("Map_Kiruna")]
			public virtual void Map_Kiruna()
			{
				Map = "Kiruna";
				var score = Run(Map);
				AssertScore(score);
			}

			#region Strategies

			[TestClass]
			public class DefaultStrategy : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					return null;
				}
			}

			[TestClass]
			[TestCategory("Map_Gothenburg")]
			public class Gothenburg_1 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Playground", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "EnvironmentalHouse")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Gothenburg")]
			public class Gothenburg_2 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Playground", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Gothenburg")]
			public class Gothenburg_3 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Playground", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Gothenburg")]
			public class Gothenburg_4 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Playground", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Apartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Kiruna")]
			public class Kiruna_1 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 4;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new []{ "Insulation", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "EnvironmentalHouse")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Kiruna")]
			public class Kiruna_2 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Insulation", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Kiruna")]
			public class Kiruna_3 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 3;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Insulation", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Map_Kiruna")]
			public class Kiruna_4 : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 5;
						})
						.Append<BuyUpgradeTurnStrategy>(c => c.IncludedUpgrades = new[] { "Insulation", "Charger" })
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Apartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_CabinStarter_Max10Buildings_WithParksAndMalls : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Mall";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_ModernStarter_Max3Buildings : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 3;
						})
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_ModernStarter_Max5Buildings : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 5;
						})
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_ModernStarter_Max10Buildings : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_ModernStarter_Max10Buildings_WithParksAndMalls : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Mall";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c =>
						{
							c.MaxNumberOfResidences = 10;
						})
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_ModernStarter_WithParksAndMalls : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Mall";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class DefaultStrategy_ModernStarter_WithMallsAndParks : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Park";
							c.MaxNumberOfBuildings = 2;
						})
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
						{
							c.BuildingName = "Mall";
							c.MaxNumberOfBuildings = 3;
						})
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "ModernApartments")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class WithoutStartBuildOnTurnZero : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						//.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class WithoutStartBuildOnTurnZero_WithUtility : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c => c.BuildingName = "Mall")
						.Append<BuildUtilityCloseToResidencesTurnStrategy>(c => c.BuildingName = "Park")
						.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						//.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
						.Compile();
					return strategy;
				}
			}

			[TestClass]
			public class SingletonApartments : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("Apartments");
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Singleton")]
			public class SingletonModernApartments : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("ModernApartments");
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Singleton")]
			public class SingletonCabin : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("Cabin");
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Singleton")]
			public class SingletonEnvironmentalHouse : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("EnvironmentalHouse");
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Singleton")]
			public class SingletonHighRise : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("HighRise");
					return strategy;
				}
			}

			[TestClass]
			[TestCategory("Singleton")]
			public class SingletonLuxuryResidence : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("LuxuryResidence");
					return strategy;
				}
			}

			#endregion Strategies


			#region Helpers

			protected virtual TurnStrategyBase.StrategyBuilder StrategyBuilder()
			{
				var builder = TurnStrategyBase.Build(_loggerFactory);
				return builder;
			}

			protected TurnStrategyBase BuildSingletonStrategy(string buildingName)
			{
				var strategy = StrategyBuilder()
					.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
					{
						c.BuildingName = "Mall";
						c.MaxNumberOfBuildings = 1;
					})
					.Append<BuildUtilityCloseToResidencesTurnStrategy>(c =>
					{
						c.BuildingName = "Park";
						c.MaxNumberOfBuildings = 2;
					})
					//.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
					.Append<BuyUpgradeTurnStrategy>()
					.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
					.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
					.Append<AdjustBuildingTemperaturesTurnStrategy>()
					.Append<SingletonBuildingTurnStrategy>(
						c => c.BuildingName = buildingName
						, predicate: (state) => VerifyBuildingIsAvailable(state, buildingName)
						//, callback: VerifyStrategySucceeded
					)
					.Compile();
				return strategy;
			}

			protected bool VerifyBuildingIsAvailable(GameState state, string buildingName)
			{
				var building = state.AvailableResidenceBuildings.FirstOrDefault(x => x.BuildingName == buildingName);
				if (building == null)
				{
					Assert.Inconclusive("Building is not available from start");
				}
				return true;
			}

			protected void VerifyStrategySucceeded(GameState state, bool result, bool globalResult)
			{
				Assert.IsTrue(result);
			}

			#endregion
		}
	}
}
