using System.Collections.Generic;
using System.Linq;
using DotNet.models;
using Newtonsoft.Json;

namespace DotNet
{
    public class GameState : GameStateResponse
    {
        /// <summary>
        /// A unique ID identifying this game session.
        /// </summary>
        public string GameId { get; set; }

        /// <summary>
        /// The name of the map for the new game session.
        /// </summary>
        public string MapName { get; set; }

        /// <summary>
        /// The maximum number of turns the game.
        /// </summary>
        public int MaxTurns { get; set; }

        /// <summary>
        /// The highest the temperature will reach on this map.
        /// </summary>
        public double MaxTemp { get; set; }
        
        /// <summary>
        /// The lowest the temperature will go on this map.
        /// </summary>
        public double MinTemp { get; set; }
        
        /// <summary>
        /// A representation of the map.
        /// The representative values of the map:
        /// 0 = Not buildable terrain.
        /// 1 = Buildable terrain.
        /// </summary>
        public int[][] Map { get; set; }

        /// <summary>
        /// The available energy sources, with prices and co2 emissions.
        /// </summary>
        public List<EnergyLevel> EnergyLevels { get; set; }

        /// <summary>
        /// A list of the residences that can be built on this map.
        /// </summary>
        public List<BlueprintResidenceBuilding> AvailableResidenceBuildings { get; set; }

        /// <summary>
        /// A list of the utility buildings that can be built on this map.
        /// </summary>
        public List<BlueprintUtilityBuilding> AvailableUtilityBuildings { get; set; }

        public List<Effect> Effects { get; set; }

        public List<Upgrade> AvailableUpgrades { get; set; }


        public void UpdateState(GameStateResponse state)
        {
            Turn = state.Turn;
            ResidenceBuildings = state.ResidenceBuildings;
            Errors = state.Errors;
            Funds = state.Funds;
            Messages = state.Messages;
            CurrentTemp = state.CurrentTemp;
            HousingQueue = state.HousingQueue;
            QueueHappiness = state.QueueHappiness;
            TotalCo2 = state.TotalCo2;
            TotalHappiness = state.TotalHappiness;
            UtilityBuildings = state.UtilityBuildings;
        }

        public override string ToString()
        {
            var gameinfo = "\nstate Index:" + Turn + "\nFunds:" + Funds + "\nOutdoor temperature" + CurrentTemp +
                           "\nHousing Queue:" + HousingQueue + "\n";
            var buildings = "";
            var utilities = "";

            foreach (var building in ResidenceBuildings)
            {
                var effects = "";
                foreach (var effect in building.Effects)
                {
                    effects += "," + effect;
                }


                buildings += ("\n" + building.BuildingName + "\n" + "    energyIn:" + building.EffectiveEnergyIn
                              + "\n" + "    Indoor Temperature: " + building.Temperature
                              + "\n" + "    Health:" + building.Health
                              + "\n" + "    Happiness:" + building.HappinessPerTickPerPop
                              + "\n" + "    Population:" + building.CurrentPop
                              + "\n" + "     Effects:" + effects);
            }

            buildings = UtilityBuildings.Aggregate(buildings,
                (current, building) => current + ("\n" + building.BuildingName + "\n" + "    energyIn:" +
                                                  building.EffectiveEnergyIn + "\n" + "    Progress:" +
                                                  building.BuildProgress));


            return gameinfo + buildings + utilities;
        }
    }


    public class Upgrade
    {
        public string Name { get; set; }
        public string Effect { get; set; }
        public double Cost { get; set; }
    }

    public class EnergyLevel
    {
        public int EnergyThreshold { get; set; }
        public double CostPerMwh { get; set; }
        public double TonCo2PerMwh { get; set; }
    }

    public class Effect
    {
        public string Name { get; set; }
        public int Radius { get; set; }
        public double EmissivityMultiplier { get; set; }
        public double DecayMultiplier { get; set; }
        public double BuildingIncomeIncrease { get; set; }
        public double MaxHappinessIncrease { get; set; }
        public double MwhProduction { get; set; }
        public double BaseEnergyMwhIncrease { get; set; }
        public double Co2PerPopIncrease { get; set; }
        public double DecayIncrease { get; set; }
    }
}