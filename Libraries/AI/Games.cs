using aima.core.search.adversarial;
//using aima.games;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Aricie.PortalKeeper.AI.Games
{
    public enum GameStrategyType
    {
        MiniMax,
        AlphaBeta
    }

    public class GameStrategyInfo
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public GameStrategyType StrategyType { get; set; }

        public object MakeMove(Game objGame, object state)
        {
            object toReturn = null;
            AdversarialSearch objGameStrategy;
            switch (StrategyType)
            {
                case GameStrategyType.AlphaBeta:
                    objGameStrategy = AlphaBetaSearch.createFor(objGame);
                    break;
                default:
                    objGameStrategy = MinimaxSearch.createFor(objGame);
                    break;
            }
            var action = objGameStrategy.makeDecision(state);
            toReturn = objGame.getResult(state, action);
            return toReturn;
        }

    }



}
