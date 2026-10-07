public class ConsoleView : IGameObserver
{
    public void OnStateChanged(Game game)
    {
        game.board.GetBoard();
    }

    public void RenderResult(string resultText)
    {
        Console.WriteLine(resultText);
    }
}

public interface IGameObserver
{
    void OnStateChanged(Game game);
}