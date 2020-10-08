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

[assembly: Parallelize(Workers = 3, Scope = ExecutionScope.MethodLevel)]

namespace DotNet.Tests
{
	[TestClass]
	public abstract class GameTests
	{
		private string ApiKey = AssemblySetup.ApiKey;
		protected virtual string Map { get; set; }

		private GameRunner _runner;
		private StringBuilder _output;

		private readonly Lazy<ILoggerFactory> _loggerFactory;
		private readonly ILogger _logger;


		protected GameTests()
		{
			_loggerFactory = new Lazy<ILoggerFactory>(() =>
			{
				var loggerFilterOptions = new LoggerFilterOptions
				{
					MinLevel = LogLevel.Information,
				};
				var providers = new List<ILoggerProvider>();
				if (Debugger.IsAttached)
				{
					providers.Add(new DebugLoggerProvider());
					loggerFilterOptions.MinLevel = LogLevel.Debug;
				}
				else
				{
					_output = new StringBuilder();
					providers.Add(new InMemoryLoggerProvider(_output));
				}
				var loggerFactory = new LoggerFactory(providers, loggerFilterOptions);
				return loggerFactory;
			});
			_logger = _loggerFactory.Value.CreateLogger(GetType());
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
				var runner = GameRunner.New(ApiKey, map, _loggerFactory.Value);
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
			public virtual void Map_training1()
			{
				Map = "training1";
				var score = Run(Map);
				AssertScore(score);
			}

			[TestMethod]
			public virtual void Map_training2()
			{
				Map = "training2";
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
			public class SingletonApartments : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("Apartments");
					return strategy;
				}
			}

			[TestClass]
			public class SingletonModernApartments : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("ModernApartments");
					return strategy;
				}
			}

			[TestClass]
			public class SingletonCabin : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("Cabin");
					return strategy;
				}
			}

			[TestClass]
			public class SingletonEnvironmentalHouse : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("EnvironmentalHouse");
					return strategy;
				}
			}

			[TestClass]
			public class SingletonHighRise : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = BuildSingletonStrategy("HighRise");
					return strategy;
				}
			}

			[TestClass]
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
				var builder = TurnStrategyBase.Build(_loggerFactory.Value);
				return builder;
			}

			protected TurnStrategyBase BuildSingletonStrategy(string buildingName)
			{
				var strategy = StrategyBuilder()
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
