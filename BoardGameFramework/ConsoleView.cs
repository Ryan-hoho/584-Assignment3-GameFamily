using static System.Console;

public interface IGameObserver
{
    void OnGameStarted(Game game);
    void OnTurnChanged(Game game);
    void OnBoardChanged(Game game);
    void OnMessage(string message);
    void OnGameOver(Game game, GameResult result);
    void OnHelpRequested(string rulesText);
}

public class ConsoleView : IGameObserver

{
    public void OnGameStarted(Game game)
    {
        WriteLine();
        WriteLine("========================================");
        WriteLine("              GAME START");
        WriteLine("========================================");

    }
    public void OnTurnChanged(Game game)
    {
        int playerNumber = game.currentPlayer == game.p1 ? 1 : 2;
        
        WriteLine();
        WriteLine("----------------------------------------");
        WriteLine($"Turn {game.turnCount} | Player {playerNumber}");
        WriteLine("----------------------------------------");
    }
    public void OnBoardChanged(Game game)
    {
        IVisibilityStrategy visibility =
            game.GetVisibilityStrategy();
        char symbol =
            game.currentPlayer == game.p1
                ? Piece.ordinary[0]
                : Piece.ordinary[1];
        char[,] visibleBoard =
            visibility.GetVisibleBoard(
                game.board,
                symbol
            );
        game.board.DisplayBoard(visibleBoard);
    }

    public void OnMessage(string message)
    {
        WriteLine(message);
    }

    public void OnGameOver(Game game, GameResult result)
    {
        WriteLine();
        WriteLine("========================================");
        WriteLine("               GAME OVER");
        WriteLine("========================================");
        switch (result)
        {
            case GameResult.Player1Wins:
                WriteLine("Player 1 wins!");
                break;
            case GameResult.Player2Wins:
                WriteLine("Player 2 wins!");
                break;
            case GameResult.Draw:
                WriteLine("It's a draw!");
                break;
            default:
                WriteLine("Game is still in progress.");
                break;
        }
        WriteLine("========================================");
    }

    public void OnHelpRequested(string rulesText)
    {
        WriteLine();
        WriteLine("========================================");
        WriteLine("                  HELP");
        WriteLine("========================================");
        WriteLine();
        WriteLine("GAME COMMANDS");
        WriteLine("----------------------------------------");
        WriteLine("help        Show commands and rules");
        WriteLine("undo        Undo the previous full turn");
        WriteLine("redo        Redo the previous full turn");
        WriteLine("save        Save the current game");
        WriteLine("load        Load a saved game");
        WriteLine("quit        Quit the game");
        WriteLine("pass        Pass when no legal Reversi move exists");
        WriteLine();
        WriteLine("GAME RULES");
        WriteLine("----------------------------------------");
        WriteLine(rulesText);
        WriteLine("========================================");
        WriteLine();
    }

}
