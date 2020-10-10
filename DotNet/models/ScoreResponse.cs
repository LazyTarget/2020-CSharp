namespace DotNet.models
{
    public class ScoreResponse
    {
        public string GameId { get; set; }
        public int TotalCo2 { get; set; }
        public int TotalHappiness { get; set; }
        public int FinalPopulation { get; set; }
        public int FinalScore { get; set; }
    }
}