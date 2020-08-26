namespace DotNet.models
{
    public class BlueprintResidenceBuilding : BlueprintBuilding
    {
        public int MaxPop { get; set; }
        public double IncomePerPop { get; set; }

        /// <summary>
        /// Higher emissivity means that the building leaks more heat to the outside.
        /// Keep the emissivity between 0-0.8 to not make it doable to parry the temperature differences
        /// </summary>
        public double Emissivity { get; set; }

        public int MaintenanceCost { get; set; }
        public double DecayRate { get; set; }

        public double MaxHappiness { get; set; }
    }
}