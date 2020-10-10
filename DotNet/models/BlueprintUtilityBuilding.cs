using System.Collections.Generic;

namespace DotNet.models
{
    public class BlueprintUtilityBuilding : BlueprintBuilding
    {
        
        public List<string> Effects { get; set; }
        public double QueueIncrease { get; set; } = 0;
    }
}