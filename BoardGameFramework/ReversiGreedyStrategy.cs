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
