using static System.Console;

public interface IGameObserver
{
    void OnGameStarted(Game game);
    void OnTurnChanged(Game game);
    void OnBoardChanged(Game game);
    void OnMessage(string message);
    void OnGameOver(Game game, int winnerNumber);
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
        // WriteLine("Type 'help' to view commands and rules.");有介紹的功能嗎？
        // WriteLine();
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
                ? 'X'
                : 'O';
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

    public void OnGameOver(Game game, int winnerNumber)
    {
        WriteLine();
        WriteLine("========================================");
        WriteLine("               GAME OVER");
        WriteLine("========================================");
        WriteLine($"Player {winnerNumber} wins!");
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
