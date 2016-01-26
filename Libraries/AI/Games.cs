using System.Collections.Generic;
using aima.core.agent;
using aima.core.environment.connectfour;
using aima.core.search.adversarial;
using aima.core.search.framework;
using aima.core.search.nondeterministic;
using Aricie.DNN.Services;
using Aricie.DNN.Services.Flee;
using Aricie.DNN.UI.Attributes;
using Aricie.PortalKeeper.AI.Search;
//using aima.games;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Aricie.PortalKeeper.AI.Games
{
    public enum GameStrategyType
    {
        MiniMax,
        AlphaBeta,
        IterativeDeepeningAlphaBeta,
        ConnectFourIDAlphaBeta
    }

    


    public class GameStrategyInfo
    {

        public GameStrategyInfo()
        {
            MinUtility = 0.0;
            MaxUtility = 1.0;
            MaxDurationSeconds = 5;
        }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonConverter(typeof(StringEnumConverter))]
        public GameStrategyType StrategyType { get; set; }

        [ConditionalVisible("StrategyType", false, true, GameStrategyType.IterativeDeepeningAlphaBeta )]
        public double MinUtility { get; set; }

        [ConditionalVisible("StrategyType", false, true, GameStrategyType.IterativeDeepeningAlphaBeta)]
        public double MaxUtility { get; set; }

        [ConditionalVisible("StrategyType", false, true, GameStrategyType.IterativeDeepeningAlphaBeta, GameStrategyType.ConnectFourIDAlphaBeta)]
        public int MaxDurationSeconds { get; set; }

        public GameMoveResult MakeMove(Game objGame, object state)
        {

            var moves = MakeMoves(objGame, state, 1);
            if (moves.Count>0)
            {
                return moves[0];
            } 
            return null;
        }

        public List<GameMoveResult> MakeMoves(Game objGame, object state)
        {
            //todo: relax hard coded limit if needed
            return MakeMoves(objGame,state,10000);
        }

        public List<GameMoveResult> MakeMoves(Game objGame, object state, int movesNumber)
        {
            var toReturn = new List<GameMoveResult>();
            AdversarialSearch objGameStrategy;
            switch (StrategyType)
            {
                case GameStrategyType.AlphaBeta:
                    objGameStrategy = AlphaBetaSearch.createFor(objGame);
                    break;
                case GameStrategyType.IterativeDeepeningAlphaBeta:
                    objGameStrategy = IterativeDeepeningAlphaBetaSearch.createFor(objGame, MinUtility, MaxUtility, MaxDurationSeconds);
                    break;
                case GameStrategyType.ConnectFourIDAlphaBeta:
                    objGameStrategy = new ConnectFourAIPlayer(objGame, MaxDurationSeconds);
                    break;
                default:
                    objGameStrategy = MinimaxSearch.createFor(objGame);
                    break;
            }
            int counter = 0;
            while (!objGame.isTerminal(state) && counter < movesNumber)
            {
                var action = objGameStrategy.makeDecision(state);
                var newMove = new GameMoveResult() { Game = objGame,  InitialState = state, Action = action, Metrics = objGameStrategy.getMetrics()};
                toReturn.Add(newMove);
                state = newMove.ResultState;
                counter++;
            }
            
            return toReturn;
        }

    }

    public class GameMoveResult
    {
        public Game Game { get; set; }

        public object InitialState { get; set; }


        public object Player => Game.getPlayer(InitialState);


        public object Action { get; set; }

        public Metrics Metrics { get; set; }


        private object _resultState;

        public object ResultState
        {
            get
            {
                if (_resultState == null)
                {
                    _resultState = Game.getResult(InitialState, Action);
                }
                return _resultState;
            }
        }


        public bool IsEndMove => Game.isTerminal(ResultState);

        public double ResultUtility
        {
            get
            {
                if (IsEndMove)
                {
                    return Game.getUtility(ResultState, Player);
                }
                return 0;
            }
        }


    }


    public class GameAgentInfo
    {

        public GameAgentInfo()
        {
            Game = new AnonymousGeneralVariableInfo<Game>();
            State = new AnonymousGeneralVariableInfo();
            Strategy = new SimpleOrExpression<GameStrategyInfo>();
        }

        [ExtendedCategory("Game")]
        public AnonymousGeneralVariableInfo<Game> Game { get; set; }

        [ExtendedCategory("State")]
        public AnonymousGeneralVariableInfo State { get; set; }

        [ExtendedCategory("Strategy")]
        public SimpleOrExpression<GameStrategyInfo> Strategy { get; set; }

       
        public List<GameMoveResult> MakeMoves(object owner, IContextLookup globalVars, int movesNumber)
        {
            var objGame = Game.EvaluateTyped(owner, globalVars);
            var objState= State.Evaluate(owner, globalVars);
            var objStrategy = Strategy.GetValue(owner, globalVars);

            return objStrategy.MakeMoves(objGame, objState, movesNumber);

        }

        public GameMoveResult MakeMove(object owner, IContextLookup globalVars)
        {

            var moves = MakeMoves( owner, globalVars, 1);
            if (moves.Count > 0)
            {
                return moves[0];
            }
            return null;
        }

        public List<string> PrintActions(List<GameMoveResult> moves)
        {
            var toReturn = new List<string>();
            foreach (GameMoveResult move in moves)
            {
                toReturn.Add(move.Action.ToString());
            }
            return toReturn;
        }


    }

}

