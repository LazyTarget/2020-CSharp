using System.Collections.Generic;

namespace DotNet.models
{
    
    public class GameStateResponse
    {
        /// <summary>
        /// The index of the current state.
        /// </summary>
        public int Turn { get; set; }
        
        /// <summary>
        /// A list of the residence buildings on the map.
        /// </summary>
        public List<BuiltResidenceBuilding> ResidenceBuildings { get; set; }
        
        /// <summary>
        /// A list of the utility buildings on the map
        /// </summary>
        public List<BuiltUtilityBuilding> UtilityBuildings { get; set; }
        
        /// <summary>
        /// The available funds, used to purchase, for example, buildings and energy.
        /// </summary>
        public double Funds { get; set; }
        
        /// <summary>
        /// The total amount of CO2 released during the game so far.
        /// </summary>
        public double TotalCo2 { get; set; }
        
        /// <summary>
        /// The total amount of happiness generated during the game so far.
        /// </summary>
        public double TotalHappiness { get; set; }
        
        /// <summary>
        /// The current outdoors temperature.
        /// </summary>
        public double CurrentTemp { get; set; }
        
        /// <summary>
        /// How many people are currently standing in queue to get a place to live.
        /// </summary>
        public int HousingQueue { get; set; }

        /// <summary>
        /// The general happiness for people not in your residences.
        /// </summary>
        public double QueueHappiness { get; set; }
        
        /// <summary>
        /// Any error message produced in this state.
        /// </summary>
        public List<string> Errors { get; set; }
        /// <summary>
        /// Any regular message produced in this state.
        /// </summary>
        public List<string> Messages { get; set; }

       

    }
}
