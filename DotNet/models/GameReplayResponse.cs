namespace DotNet.models
{
	public class GameReplayResponse
	{
		public string TeamName { get; set; }

		public GameStateResponse InitialState { get; set; }

		public GameStateResponse[] StateUpdates { get; set; }

		public ScoreResponse Score { get; set; }
	}
}
