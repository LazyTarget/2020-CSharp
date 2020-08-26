using System;

namespace DotNet.models
{
    public class GamesResponse
    {
        public string GameId { get; set; }
        public bool Active { get; set; }
        public bool Started { get; set; }
        public DateTime StartedAt { get; set; }
    }
}