public class ReversiGreedyStrategy : IMoveStrategy
{
    const string PASS = "PASS";
    public string ChooseMove(Board board, char symbol)
    {
        string bestMove = PASS;
        int bestScore = 0;
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                int flips = FlankingRules.FindFlips(board, row, col, symbol).Count;
                if (flips > 0)
                {
                    int score = Score(board, row, col, flips);
                    if (bestMove == PASS || score > bestScore)
                    {
                        bestScore = score;
                        bestMove = $"{Piece.reversiDisk[0]}{row}:{col}";
                    }
                }
            }
        }
        return bestMove;
    }

    protected virtual int Score(Board board, int row, int col, int flips)
    {
        return flips;
    }

    protected bool IsCorner(Board board, int row, int col)
    {
        if (row != 1 && row != board.Size)
        {
            return false;
        }
        if (col != 1 && col != board.Size)
        {
            return false;
        }
        return true;
    }
}

public class AntiReversiStrategy : ReversiGreedyStrategy
{
    // Larger than any possible flip count, so a corner is only taken when nothing else is legal
    const int CORNERPENALTY = 100;

    protected override int Score(Board board, int row, int col, int flips)
    {
        // Fewer flips is better, so the score is the negative flip count.
        int score = 0 - flips;
        if (IsCorner(board, row, col))
        {
            score = score - CORNERPENALTY;
        }
        return score;
    }
}

public class CornerGreedyStrategy : ReversiGreedyStrategy
{
    // Larger than any possible flip count, so a corner always beats a non-corner move
    const int CORNERBONUS = 100;
    protected override int Score(Board board, int row, int col, int flips)
    {
        int score = base.Score(board, row, col, flips);
        if (IsCorner(board, row, col))
        {
            score = score + CORNERBONUS;
        }
        return score;
    }
}
