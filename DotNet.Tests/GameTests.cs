using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using DotNet.Logging;
using DotNet.Strategy;
using Microsoft.Extensions.Configuration;
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

		private readonly Lazy<ILoggerFactory> _loggerFactory;


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
				Debug.WriteLine(fullLog);
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
				var runner = GameRunner.New(ApiKey, map, _loggerFactory.Value);
				_runner = runner;
				return runner;
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		protected virtual TurnStrategyBase.StrategyBuilder StrategyBuilder()
		{
			var builder = TurnStrategyBase.Build(_loggerFactory.Value);
			return builder;
		}


		[TestMethod]
		public void DefaultStrategy()
		{
			var runner = GetRunner();
			var score = runner.Run();
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void Default_Apartments()
		{
			var buildingName = "Apartments";

			var strategy = StrategyBuilder()
				.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>(c => c.BuildingName = buildingName)
				.Append<BuyUpgradeTurnStrategy>()
				.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
				.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
				.Append<AdjustBuildingTemperaturesTurnStrategy>()
				.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
				.Compile();

			var score = GetRunner().Run(strategy);
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
			strategy = new SingletonBuildingTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
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
			strategy = new SingletonBuildingTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
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
			strategy = new SingletonBuildingTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
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
			strategy = new SingletonBuildingTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
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
			strategy = new SingletonBuildingTurnStrategy(strategy)
			{
				BuildingName = "Cabin",
			};

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}

		[TestMethod]
		public void WithoutStartBuildOnTurnZero()
		{
			var strategy = StrategyBuilder()
				.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
				.Append<BuyUpgradeTurnStrategy>()
				.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
				.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
				.Append<AdjustBuildingTemperaturesTurnStrategy>()
				//.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
				.Compile();

			var score = GetRunner().Run(strategy);
			Assert.IsTrue(score.FinalScore > 0);
		}


		[TestClass]
		public class training1Map : GameTests
		{
			protected override string Map { get; set; } = "training1";
		}

		[TestClass]
		public class training2Map : GameTests
		{
			protected override string Map { get; set; } = "training2";
		}


		[TestClass]
		public abstract class StrategyTests : GameTests
		{
			protected abstract TurnStrategyBase GetStrategy();

			[TestMethod]
			public virtual void training1()
			{
				Map = "training1";
				var strategy = GetStrategy();

				var runner = GetRunner();
				var score = runner.Run(strategy);
				Assert.IsTrue(score.FinalScore > 0);
			}

			[TestMethod]
			public virtual void training2()
			{
				Map = "training2";
				var strategy = GetStrategy();

				var runner = GetRunner();
				var score = runner.Run(strategy);
				Assert.IsTrue(score.FinalScore > 0);
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
			public class SingletonCabin : StrategyTests
			{
				protected override TurnStrategyBase GetStrategy()
				{
					var strategy = StrategyBuilder()
						//.Append<BuildBuildingWhenCloseToPopMaxTurnStrategy>()
						.Append<BuyUpgradeTurnStrategy>()
						.Append<MaintenanceWhenBuildingIsGettingDamagedTurnStrategy>()
						.Append<BuildWhenHasBuildingsUnderConstructionTurnStrategy>()
						.Append<AdjustBuildingTemperaturesTurnStrategy>()
						.Append<SingletonBuildingTurnStrategy>(c => c.BuildingName = "Cabin")
						.Compile();
					return strategy;
				}
			}

			#endregion Strategies
		}
	}
}
