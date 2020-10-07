using System;
using System.Collections.Generic;

namespace DotNet.models
{
    public  class BuiltBuilding 
    {
        public string BuildingName { get; set; }
        public Position Position { get; set; }
        public double EffectiveEnergyIn { get; set; }
        public int BuildProgress { get; set; }
        public bool CanBeDemolished { get; set; }
        
        public List<string> Effects { get; set; }
    }
    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return ( x + ","+ y);
        }
    }
}