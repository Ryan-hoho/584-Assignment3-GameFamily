public enum ReversiResult
{
    InProgress,
    XWins,
    OWins,
    Draw
}

public interface IReversiWinStrategy
{
    ReversiResult Evaluate(Board board);
}

//Win Strategy 1 —— Standard Reversi
public class DiskMajority : IReversiWinStrategy
{
    public ReversiResult Evaluate(Board board)
    {
        // Standard Reversi only decides a winner when neither player can move.
        if (!ReversiRules.NoMovesForEitherPlayer(board))
        {
            return ReversiResult.InProgress;
        }

        int xCount = ReversiRules.CountDisks(board, 'X');
        int oCount = ReversiRules.CountDisks(board, 'O');

        if (xCount > oCount)
        {
            return ReversiResult.XWins;
        }

        if (oCount > xCount)
        {
            return ReversiResult.OWins;
        }

        return ReversiResult.Draw;
    }
}

//Win Strategy 2 —— Anti-Reversi
public class DiskMinority : IReversiWinStrategy
{
    public ReversiResult Evaluate(Board board)
    {
        // Anti-Reversi only decides a winner when neither player can move.
        if (!ReversiRules.NoMovesForEitherPlayer(board))
        {
            return ReversiResult.InProgress;
        }

        int xCount = ReversiRules.CountDisks(board, 'X');
        int oCount = ReversiRules.CountDisks(board, 'O');

        // Anti-Reversi: the player with fewer disks wins.
        if (xCount < oCount)
        {
            return ReversiResult.XWins;
        }

        if (oCount < xCount)
        {
            return ReversiResult.OWins;
        }

        return ReversiResult.Draw;
    }
}

//Win Strategy 3 —— CornerDominance
public class CornerDominance : IReversiWinStrategy
{
    public ReversiResult Evaluate(Board board)
    {
        int xCorners = CountCorners(board, 'X');
        int oCorners = CountCorners(board, 'O');

        // Sudden-death condition: capturing 3 out of 4 corners wins immediately.
        if (xCorners >= 3)
        {
            return ReversiResult.XWins;
        }

        if (oCorners >= 3)
        {
            return ReversiResult.OWins;
        }

        // If no one has 3 corners yet and normal Reversi has not ended, the game continues.
        if (!ReversiRules.NoMovesForEitherPlayer(board))
        {
            return ReversiResult.InProgress;
        }

        // Fallback: normal majority count.
        int xCount = ReversiRules.CountDisks(board, 'X');
        int oCount = ReversiRules.CountDisks(board, 'O');

        if (xCount > oCount)
        {
            return ReversiResult.XWins;
        }

        if (oCount > xCount)
        {
            return ReversiResult.OWins;
        }

        return ReversiResult.Draw;
    }

    private int CountCorners(
        Board board,
        char symbol)
    {
        int count = 0;

        int last = board.Size;

        if (board.GetCellInfo(1, 1) == symbol)
        {
            count++;
        }

        if (board.GetCellInfo(1, last) == symbol)
        {
            count++;
        }

        if (board.GetCellInfo(last, 1) == symbol)
        {
            count++;
        }

        if (board.GetCellInfo(last, last) == symbol)
        {
            count++;
        }

        return count;
    }
}