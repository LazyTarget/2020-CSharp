using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using DotNet.Interfaces;
using DotNet.models;
using Microsoft.Extensions.Logging;

namespace DotNet.Strategy
{
	public class AdjustBuildingTemperaturesTurnStrategy : TurnStrategyBase
	{
		private const double _degreesPerPop = 0.04;
		private const double _degreesPerExcessMwh = 0.75;
		private const int _adjustCost = 150;

		public AdjustBuildingTemperaturesTurnStrategy() : base()
		{
		}

		public AdjustBuildingTemperaturesTurnStrategy(TurnStrategyBase parent) : base(parent)
		{
		}

		public double MinTemperature { get; set; } = 18;

		public double MaxTemperature { get; set; } = 24;

		public double AllowedDiffMargin { get; set; } = 0.2;

		public double AllowedTemperatureDiffMargin { get; set; } = 2;

		public double TargetTemperature { get; set; } = 21;

		protected override bool TryExecuteTurn(Randomizer randomizer, IGameLayer gameLayer, GameState state)
		{
			double? predictedTrend = null;
			var outdoorTemp = state.CurrentTemp;
			if (state.TemperatureHistory?.Count > 2)
			{
				// Predict outdoor temperature
				var prePreviousTemp = state.TemperatureHistory.Reverse().Skip(2).First().Value;
				var previousTemp = state.TemperatureHistory.Reverse().Skip(1).First().Value;

				var previousDiff = previousTemp - prePreviousTemp;
				var currentDiff = state.CurrentTemp - previousTemp;
				if (previousDiff > 0 && currentDiff > 0)
				{
					// Trend is "getting hotter"
					predictedTrend = (previousDiff + currentDiff) / 2;
				}
				else if (previousDiff < 0 && currentDiff < 0)
				{
					// Trend is "getting colder"
					predictedTrend = (previousDiff + currentDiff) / 2;
				}


				if (predictedTrend.HasValue)
				{
					// Add the trend twice to more quickly react to big temperature changes
					predictedTrend = predictedTrend.Value * 2;

					outdoorTemp = state.CurrentTemp + predictedTrend.Value;
					Logger.LogDebug($"Using prediction for OutdoorTemp: {outdoorTemp}, CurrentTemp: {state.CurrentTemp}");
				}
			}

			var buildings = state.GetCompletedBuildings()
				.OfType<BuiltResidenceBuilding>()
				//.Where(x => x.Temperature < MinTemperature + AllowedTemperatureDiffMargin)
				.OrderBy(x => x.Temperature)
				.ToArray();

			foreach (var building in buildings)
			{
				var blueprint = state.AvailableResidenceBuildings.Find(x => x.BuildingName == building.BuildingName);

				// Predict next temperature
				var newTemp =
					building.Temperature +
					(building.EffectiveEnergyIn - blueprint.BaseEnergyNeed) * _degreesPerExcessMwh +
					_degreesPerPop * building.CurrentPop -
					(building.Temperature - outdoorTemp) * blueprint.Emissivity;
				if (IsBetween(newTemp,
					TargetTemperature - AllowedTemperatureDiffMargin,
					TargetTemperature + AllowedTemperatureDiffMargin))
				{
					// close enough to target temperature already...
					continue;
				}

				if (newTemp < MinTemperature)
				{
					// Is below minimum, fake that it is much colder than it is to make a faster recovery
					outdoorTemp -= Math.Abs(TargetTemperature - building.Temperature);
				}
				if (building.Temperature < MinTemperature)
				{
					// Is below minimum, fake that it is much colder than it is to make a faster recovery
					outdoorTemp -= Math.Abs(TargetTemperature - building.Temperature);
				}

				if (newTemp > MaxTemperature)
				{
					// Is above maximum, fake that it is much hotter than it is to make a faster recovery
					outdoorTemp += Math.Abs(TargetTemperature - building.Temperature);
				}
				if (building.Temperature > MaxTemperature)
				{
					// Is above maximum, fake that it is much hotter than it is to make a faster recovery
					outdoorTemp += Math.Abs(TargetTemperature - building.Temperature);
				}


				var energy= blueprint.BaseEnergyNeed + (building.Temperature - outdoorTemp)
					* blueprint.Emissivity / 1 + 0.5 - building.CurrentPop * 0.04;
				if (predictedTrend.GetValueOrDefault() > 0)
				{
					// Trend is getting hotter
					// Then reduce energy to save heating resources and to cooldown appartment...
					energy *= 0.9;
				}
				else if (predictedTrend.GetValueOrDefault() < 0)
				{
					// Trend is getting colder
					// Then apply more energy to make sure has enough heating...
					energy *= 1.1;
				}

				if (energy < blueprint.BaseEnergyNeed)
				{
					Logger.LogWarning($"Wanted to set lower energy than BaseEnergyNeed, restoring to base: {blueprint.BaseEnergyNeed:N3} Mwh from {energy:N3} Mwh");
					energy = blueprint.BaseEnergyNeed;
				}


				// Predict next temperature, if change energy
				var predictedNewTemp =
					building.Temperature +
					(energy - blueprint.BaseEnergyNeed) * _degreesPerExcessMwh +
					_degreesPerPop * building.CurrentPop -
					(building.Temperature - outdoorTemp) * blueprint.Emissivity;

				Logger.LogDebug($"{building.BuildingName} at {building.Position}");
				Logger.LogDebug($"Current building temp: \t\t{building.Temperature:N3}");
				Logger.LogDebug($"Next building temp: \t\t{newTemp:N3}");
				Logger.LogDebug($"Predicted New Temp: \t\t{predictedNewTemp:N3}");
				Logger.LogDebug($"Current building energy: \t{building.EffectiveEnergyIn}/{building.RequestedEnergyIn} Mwh");
				Logger.LogDebug($"New requested energy: \t\t{energy:N3} Mwh");

				if (newTemp < TargetTemperature)
				{
					// Colder than target
					if (energy < building.RequestedEnergyIn)
					{
						// Should not lower energy if already too cold
						continue;
					}
					else { }
				}
				else if (newTemp > TargetTemperature)
				{
					// Hotter than target
					if (energy > building.RequestedEnergyIn)
					{
						// Should not increase energy if already too hot
						continue;
					}
					else { }
				}

				if (IsBetween(energy,
					building.RequestedEnergyIn - AllowedDiffMargin,
					building.RequestedEnergyIn + AllowedDiffMargin))
				{
					// minimal change, wait to update energy (to reduce calls and save money)
					continue;
				}


				if (state.Funds < _adjustCost)
				{
					Logger.LogWarning($"Wanted to apply energy '{energy}' to building at {building.Position}, but has insufficient funds");
					return false;
				}

				gameLayer.AdjustEnergy(building.Position, energy, state.GameId);
				return true;
			}

			return false;
		}

		public static bool IsBetween(double num, double lower, double upper, bool inclusive = false)
		{
			return inclusive
				? lower <= num && num <= upper
				: lower < num && num < upper;
		}
	}
}
