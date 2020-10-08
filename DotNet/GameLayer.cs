using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Interfaces;
using DotNet.models;
using Newtonsoft.Json;

namespace DotNet
{
    public class GameLayer : IGameLayer
    {
        private GameState _gameState;

        private readonly Api _api;

        public GameLayer(string apiKey)
        {
            _api = new Api(apiKey);
        }

        ///  <summary> Creates a new game.</summary>
        ///  <param name="map">map choice </param>
        public string NewGame(string map)
        {
            var mapName = "{\"mapName\":" + JsonConvert.SerializeObject(map) + "}";
            var state = _api.NewGame(mapName).Result;
            _gameState = state;
            return state.GameId;
        }

        /// <summary> Starts a new game.</summary>
        ///<param name="gameId">gameId choice </param>
        public void StartGame(string gameId = null)
        {
            var state = _api.StartGame(gameId);
            _gameState.UpdateState(state.Result);
            _gameState.ActionHistory?.Clear();
        }

        /// <summary> Place a foundation.</summary>
        /// <param name="pos"> specify where to put the building. </param>
        /// <param name="buildingName"> specify which building. </param>
        /// <param name="gameId"></param>
        /// 
        public void StartBuild(Position pos, string buildingName, string gameId = null)
        {
            var foundation = "{\"position\":" + JsonConvert.SerializeObject(pos) + ",\"buildingName\":" +
                             JsonConvert.SerializeObject(buildingName) + "}";
            var response = _api.StartBuild(foundation, gameId);
            _gameState.UpdateState(response.Result);
            _gameState.UpdateActions(response.Result, GameActions.StartBuild);
        }

        /// <summary> Creates a new game.</summary>
        /// <param name="pos"> specify where to build. </param>
        /// <param name="gameId"></param>
        /// 
        public void Build(Position pos, string gameId = null)
        {
            var position = "{\"position\":" + JsonConvert.SerializeObject(pos) + "}";
            var response = _api.Build(position, gameId);

            _gameState.UpdateState(response.Result);
            _gameState.UpdateActions(response.Result, GameActions.Build);
        }

        ///  <summary> Destroys the building.</summary>
        /// <param name="pos"> specify where to build. </param>
        ///  <param name="gameId"></param>
        public void Demolish(Position pos, string gameId = null)
        {
            var position = "{\"position\":" + JsonConvert.SerializeObject(pos) + "}";
            var response = _api.Demolish(position, gameId);
            
            _gameState.UpdateState(response.Result);
            _gameState.UpdateActions(response.Result, GameActions.Demolish);
        }

        ///  <summary> Increases the building's health points.</summary>
        /// <param name="pos"> specify where the building is. </param>
        ///  <param name="gameId"></param>
        public void Maintenance(Position pos, string gameId = null)
        {
            var position = "{\"position\":" + JsonConvert.SerializeObject(pos) + "}";
            var response = _api.Maintenance(position, gameId);

            _gameState.UpdateState(response.Result);
            _gameState.UpdateActions(response.Result, GameActions.Maintenance);
        }

        /// <summary> Waits one tick.</summary>
        ///
        public void Wait(string gameId = null)
        {
            var state = _api.Wait(gameId).Result;

            _gameState.UpdateState(state);
            _gameState.UpdateActions(state, GameActions.Wait);
        }

        ///  <summary> Buys an upgrade on the specific building.</summary>
        /// <param name="pos"> specify where the building is. </param>
        ///  <param name="upgrade"> specify which upgrade</param>
        ///  <param name="gameId"></param>
        ///  
        public void BuyUpgrade(Position pos, string upgrade, string gameId = null)
        {
            var body = "{\"position\":" + JsonConvert.SerializeObject(pos) + ",\"upgradeAction\":" +
                       JsonConvert.SerializeObject(upgrade) + "}";
            var response = _api.BuyUpgrade(body, gameId);

            _gameState.UpdateState(response.Result);
            _gameState.UpdateActions(response.Result, GameActions.Wait);
        }

        ///  <summary> Adjusts the energy on the specific buildings.</summary>
        /// <param name="position"> specify where the buildings are. </param>
        ///  <param name="value"> specify how much energy each building gets respectively. </param>
        ///  <param name="gameId"></param>
        public void AdjustEnergy(Position position, double value, string gameId = null)
        {
            var positionJson = JsonConvert.SerializeObject(position);
            var valueJson = JsonConvert.SerializeObject(value);
            var body = "{ \"position\":" + positionJson + " ,\"value\":" + valueJson + "}";
            var response = _api.AdjustEnergy(body, gameId);

            _gameState.UpdateState(response.Result);
            _gameState.UpdateActions(response.Result, GameActions.AdjustEnergy);
        }

        /// <summary> Gets the score for the specified game.</summary>
        ///
        public ScoreResponse GetScore(string gameId)
        {
            var response = _api.GetScore(gameId);
            return response.Result;
        }

        public GameReplayResponse GetReplay(string gameId)
        {
	        var response = _api.GetReplay(gameId);
	        return response.Result;
        }

        /// <summary> Updates the GameLayer with the game info from the specified gameId.</summary>
        ///
        public GameState GetNewGameInfo(string gameId)
        {
            _gameState = _api.GetGameInfo(gameId).Result;
            return _gameState;
        }

        /// <summary>
        /// Updates the GameLayer with the game state from the specified gameId.
        /// </summary>
        public GameState GetNewGameState(string gameId)
        {
	        var state = _api.GetGameState(gameId).Result;
            _gameState.UpdateState(state);
            return _gameState;
        }

        /// <summary> Returns the GameState</summary>
        ///
        public GameState GetState()
        {
            return _gameState;
        }
        
        public List<GamesResponse> GetGames()
        {
            return _api.GetGames().Result;
        }

        /// <summary>
        /// End a game
        /// </summary>
        /// <param name="gameId"></param>
        public async void EndGame(string gameId = null)
        {
            await _api.EndGame(gameId);
        }

        /// <summary>
        /// Returns a blueprint matching the building name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public BlueprintBuilding GetBlueprint(string name)
        {
            var residenceBlueprint = GetResidenceBlueprint(name);
            if (residenceBlueprint != null) return residenceBlueprint;
            return GetUtilityBlueprint(name);
        }
        
        /// <summary>
        /// Returns the blueprint matching the residence name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>BlueprintResidenceBuilding</returns>
        public BlueprintResidenceBuilding GetResidenceBlueprint(string name)
        {
            return _gameState.AvailableResidenceBuildings.Find(x => x.BuildingName == name);
        }

        /// <summary>
        /// Returns the blueprint matching the utility name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>BlueprintUtilityBuilding</returns>
        public BlueprintUtilityBuilding GetUtilityBlueprint(string name)
        {
            return _gameState.AvailableUtilityBuildings.Find(x => x.BuildingName == name);
        }

        /// <summary> Returns the Effect with name effectName</summary>
        ///  <param name="effectName"> The name of the effect you want</param>
        ///
        public Effect GetEffect(string effectName)
        {
            foreach (var effect in _gameState.Effects)
            {
                if (effect.Name == effectName)
                    return effect;
            }
            return null;
        }
    }
}