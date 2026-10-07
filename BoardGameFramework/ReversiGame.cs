using static System.Console;

public class ReversiGame : Game
{
    public ReversiGame(GameConfig config) : base(config, new Board(8))
    {
    }
    protected override void PlayMove(Player player, char piece, int row, int col)
    {
        
        char symbol = (player == p1) ? 'X' : 'O';
        MoveCommand command = new ReversiMoveCommand(board, symbol, row, col);
        history.Execute(command);
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
    protected override string GetRulesText() // just for test, need to update!!
    {
        return
            "Reversi Rules:\n" +
            "- Players take turns placing disks.\n" +
            "- A valid move must flank at least one opponent disk.\n" +
            "- Flanked opponent disks are flipped.\n" +
            "- If no legal move is available, the player must pass.";
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