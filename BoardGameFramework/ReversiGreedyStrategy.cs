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
                    int score = Score(board, row, col, flips, symbol);
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

    protected virtual int Score(Board board, int row, int col, int flips, char symbol)
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
    const int CORNERPENALTY = 100;
    protected override int Score(Board board, int row, int col, int flips, char symbol)
    {
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
    const int CORNERBONUS = 100;
    const int THREATPENALTY = 1000;
    const int CORNERSTOWIN = 3;

    protected override int Score(Board board, int row, int col, int flips, char symbol)
    {
        int score = base.Score(board, row, col, flips, symbol);
        if (IsCorner(board, row, col))
        {
            score = score + CORNERBONUS;
        }
        if (LeavesCornerThreat(board, row, col, symbol))
        {
            score = score - THREATPENALTY;
        }
        return score;
    }

    private bool LeavesCornerThreat(Board board, int row, int col, char symbol)
    {
        Board copy = CopyBoard(board);
        ReversiRules.PlaceDisk(copy, row, col, symbol);
        char opponent = OpponentOf(symbol);
        if (CountCorners(copy, opponent) < CORNERSTOWIN - 1)
        {
            return false;
        }
        int[] edges = { 1, copy.Size };
        foreach (int cornerRow in edges)
        {
            foreach (int cornerCol in edges)
            {
                if (FlankingRules.FindFlips(copy, cornerRow, cornerCol, opponent).Count > 0)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int CountCorners(Board board, char player)
    {
        int count = 0;
        int[] edges = { 1, board.Size };
        foreach (int cornerRow in edges)
        {
            foreach (int cornerCol in edges)
            {
                if (board.GetCellInfo(cornerRow, cornerCol) == player)
                {
                    count++;
                }
            }
        }
        return count;
    }

    private Board CopyBoard(Board board)
    {
        Board copy = new Board(board.Size);
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                copy.PlayMove(board.GetCellInfo(row, col), row, col);
            }
        }
        return copy;
    }

    private char OpponentOf(char player)
    {
        if (player == Piece.ordinary[0])
        {
            return Piece.ordinary[1];
        }
        return Piece.ordinary[0];
    }
}