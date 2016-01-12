using aima.core.search.adversarial;
using aima.core.environment.connectfour;
using aima.core.environment.tictactoe;

namespace Aricie.PortalKeeper.AI.Games.Demos
{
    public enum GameDemos
    {
        TicTacToe,
        ConnectFour
    }

    public class GamesDemoInfo
    {

        public GameDemos Selection { get; set; }


        public Game GetGame()
        {
            switch (Selection)
            {
                case  GameDemos.TicTacToe:
                    return new TicTacToeGame();
                case GameDemos.ConnectFour:
                    return new ConnectFourGame();
                default:
                    break;
            }
            return null;
        }

    }
}