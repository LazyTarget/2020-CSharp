using System;
using System.Collections.Generic;
using System.Text;
using DotNet.models;

namespace DotNet.Interfaces
{
	public interface IGameLayer
	{
		string NewGame(string map);
		void StartGame(string gameId);
		void StartBuild(Position pos, string buildingName, string gameId);
		void Build(Position pos, string gameId);
		void Demolish(Position pos, string gameId);
		void Maintenance(Position pos, string gameId);
		void Wait(string gameId);
		void BuyUpgrade(Position pos, string upgrade, string gameId);
		void AdjustEnergy(Position position, double value, string gameId);
		GameState GetState();
	}
}
