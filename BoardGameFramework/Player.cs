using static System.Console;

public abstract class Player
{
    public abstract string GetMove(Board board);
}

public class HumanPlayer : Player
{
    public override string GetMove(Board board)
    {
        Write("Enter your move or command ('help' for available commands): ");
        return ReadLine() ?? string.Empty;
    }
}

public class ComputerPlayer : Player
{
    private readonly IMoveStrategy strategy;
    private readonly char symbol;
    
    public ComputerPlayer(IMoveStrategy strategy, char symbol)
    {
        this.strategy = strategy;
        this.symbol = symbol;
    }

    public override string GetMove(Board board)
    {
        return strategy.ChooseMove(board, symbol);
    }
}
