using static System.Console;

public class ReversiGame : Game
{
    public ReversiGame(GameConfig config) : base(config, new Board(8))
    {
    }

    protected override bool ValidatePieceType(char c)
    {
        if (c == 'P')
        {
            return true;
        }
        else
        {
            WriteLine("Invalid Disk type. Use [P] for disk.");
            return false;
        }
    }

    public override bool CheckForWinner(int r, int c)
    {
        // TO-DO
        throw new NotImplementedException("Reversi CheckForWinner is not yet implemented");
    }
}


public class AntiReversi : ReversiGame
{
    public AntiReversi(GameConfig config) : base(config)
    {
    }
}


public class CornerReversi : ReversiGame
{
    public CornerReversi(GameConfig config) : base(config)
    {
    }
}