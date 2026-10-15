using static System.Console;

public class ReversiGame : Game
{
    private readonly IReversiWinStrategy winStrategy;
    protected override void PlayMove(Player player, char piece, int row, int col)
    {
        char symbol = (player == p1) ? 'X' : 'O';
        MoveCommand command = new ReversiMoveCommand(board, symbol, row, col);
        history.Execute(command);
    }

    //By default: Standard Reversi --> DiskMajority
    public ReversiGame(
        GameConfig config, Board board, Player p1, Player p2)
        : this(config, board, p1, p2, new DiskMajority())
    {
    }
    // Allows derived Reversi variants to provide their own win strategy.: AntiReversi --> DiskMinority / CornerReversi --> CornerDominance
    protected ReversiGame(
        GameConfig config,
        Board board,
        Player p1,
        Player p2,
        IReversiWinStrategy winStrategy)
        : base(config, board, p1, p2)
    {
        this.winStrategy = winStrategy;
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

    // Result Mapping
    public override GameResult CheckForWinner(int r, int c)
    {
        ReversiResult result = winStrategy.Evaluate(board);

        return result switch
        {
            ReversiResult.XWins => GameResult.Player1Wins,
            ReversiResult.OWins => GameResult.Player2Wins,
            ReversiResult.Draw => GameResult.Draw,
            _ => GameResult.InProgress
        };
    }

    protected override string GetRulesText()
    {
        return
            "Reversi Rules:\n" +
            "- Players take turns placing disks.\n" +
            "- A valid move must flank at least one opponent disk.\n" +
            "- Flanked opponent disks are flipped.\n" +
            "- If no legal move is available, the player must pass.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "P3:4        Place a disk\n" +
            "PASS        Pass when no legal move exists";
    }
    public override bool TryPass()
    {
        char symbol = currentPlayer == p1 ? 'X' : 'O';
        if (ReversiRules.HasLegalMove(board, symbol))
        {   
            return false;
        }
        history.Execute(new PassCommand());
        return true;
    }
    public override void ReplayPass(Player player)
    {
        // Determine which player's PASS is being replayed.
        char symbol = player == p1 ? 'X' : 'O';
        if (ReversiRules.HasLegalMove(board, symbol))
        {
            throw new InvalidOperationException(
                "Cannot replay an illegal PASS."
            );
        }
        history.Execute(new PassCommand());
    }

}

public class AntiReversi : ReversiGame
{
    public AntiReversi(
        GameConfig config, Board board, Player p1, Player p2)
        : base(config, board, p1, p2, new DiskMinority())
    {
    }

    protected override string GetRulesText()
    {
        return
            "Anti-Reversi Rules:\n" +
            "- Standard Reversi movement and flipping rules apply.\n" +
            "- The player with fewer disks wins.\n" +
            "- Equal disk counts result in a draw.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "P3:4        Place a disk\n" +
            "PASS        Pass when no legal move exists";
    }
}


public class CornerReversi : ReversiGame
{
    public CornerReversi(
        GameConfig config, Board board, Player p1, Player p2)
        : base(config, board, p1, p2, new CornerDominance())
    {
    }
    
    protected override string GetRulesText()
    {
        return
            "Corner Reversi Rules:\n" +
            "- Standard Reversi movement and flipping rules apply.\n" +
            "- Controlling 3 of the 4 corners results in an immediate win.\n" +
            "- Otherwise, the player with more disks wins at normal game end.\n" +
            "- Equal disk counts result in a draw.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "P3:4        Place a disk\n" +
            "PASS        Pass when no legal move exists";
    }
}

