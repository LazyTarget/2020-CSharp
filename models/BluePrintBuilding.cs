namespace DotNet.models
{
    public abstract class BlueprintBuilding
    {
        public string BuildingName { get; set; }
        public int Cost { get; set; }
        public int Co2Cost { get; set; }
        public double BaseEnergyNeed { get; set; }
        public int BuildSpeed { get; set; }
        public string Type { get; set; }
        public int ReleaseTick { get; set; }
    }

}