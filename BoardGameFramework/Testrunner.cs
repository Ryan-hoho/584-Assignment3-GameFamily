public interface ICommandParser
{
    MoveCommand Parse(string input, Game game);
}

public class ScriptReportView
{
    public void Report(Board board, string activePlayer, string resultText)
    {
        board.GetBoard();
        Console.WriteLine($"Active Player: {activePlayer}");
        Console.WriteLine($"Result: {resultText}");
    }
}