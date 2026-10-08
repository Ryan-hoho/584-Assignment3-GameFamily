using static System.Console;

public class ReversiGame : Game
{
    protected override void PlayMove(Player player, char piece, int row, int col)
    {
        // PASS 情境要特殊處理（不產生 MoveCommand，或由 parser 在更早階段攔截）
        char symbol = (player == p1) ? 'X' : 'O';
        MoveCommand command = new ReversiMoveCommand(board, symbol, row, col);
        history.Execute(command);
    }
    public ReversiGame(GameConfig config) : base(config, new Board(8))
    {
        ReversiRules.SetUpInitialBoard(board); //add for initial board
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

    protected override bool ValidateMove(char c, int row, int col)
    {
        char symbol = (currentPlayer == p1) ? 'X' : 'O';
        List<(int row, int col)> legalMoves =
            ReversiRules.FindLegalMoves(board, symbol);
        if (legalMoves.Contains((row, col)))
        {
            return true;
        }
        WriteLine(
            "Invalid move. You must place a disk that flips at least one opponent disk."
        );
        return false;
    }



    public override bool CheckForWinner(int r, int c)
    {
        // check just for test
        // throw new NotImplementedException("Reversi CheckForWinner is not yet implemented");
        return false;
    }
    protected override string GetRulesText() // for test
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

