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
        WriteLine("-----------------------");
        WriteLine("GAME START");
    }
    public void OnTurnChanged(Game game)
    {
        int playerNumber =
            game.currentPlayer == game.p1 ? 1 : 2;
        WriteLine(
            $"Turn {game.turnCount}: Player {playerNumber}");
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
        WriteLine("====== GAME OVER ======");
        WriteLine($"Player {winnerNumber} Wins!");
    }

    public void OnHelpRequested(string rulesText)
    {
        WriteLine();
        WriteLine("====== Commands ======");
        WriteLine("help        - Display help"
        );
        WriteLine("undo        - Undo the previous full turn");
        WriteLine("redo        - Redo the previous full turn");
        WriteLine("save        - Save the current game");
        WriteLine("load        - Load a saved game");
        WriteLine("quit        - Quit the game");
        WriteLine("pass        - Pass when no legal Reversi move exists");
        WriteLine();
        WriteLine("=== Game Rules ===");
        WriteLine(rulesText);

    }

}
