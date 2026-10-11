public class GomokuSmartStrategy : IMoveStrategy
{
    const char EMPTYCELL = ' ';
    const int FIVEINAROW = 5;
    private readonly int[] ROWSTEPS = { 0, 1, 1, 1 };
    private readonly int[] COLSTEPS = { 1, 0, 1, -1 };
    private readonly IMoveStrategy fallback = new GomokuRandomStrategy();
    public string ChooseMove(Board board, char symbol)
    {
        string move = FindFiveInARow(board, symbol);
        if (move != string.Empty)
        {
            return move;
        }
        move = FindFiveInARow(board, OpponentOf(symbol));
        if (move != string.Empty)
        {
            return move;
        }
        return fallback.ChooseMove(board, symbol);
    }
    private string FindFiveInARow(Board board, char player)
    {
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                if (board.GetCellInfo(row, col) == EMPTYCELL && CompletesFive(board, row, col, player))
                {
                    return $"{Piece.gomokuStone[0]}{row}:{col}";
                }
            }
        }
        return string.Empty;
    }
    private bool CompletesFive(Board board, int row, int col, char player)
    {
        for (int i = 0; i < ROWSTEPS.Length; i++)
        {
            int count = 1;
            count = count + CountInDirection(board, row, col, ROWSTEPS[i], COLSTEPS[i], player);
            count = count + CountInDirection(board, row, col, 0 - ROWSTEPS[i], 0 - COLSTEPS[i], player);
            if (count >= FIVEINAROW)
            {
                return true;
            }
        }
        return false;
    }
    private int CountInDirection(Board board, int row, int col, int rowStep, int colStep, char player)
    {
        char heavy = HeavySymbolOf(player);
        int count = 0;
        int r = row + rowStep;
        int c = col + colStep;
        while (r >= 1 && r <= board.Size && c >= 1 && c <= board.Size)
        {
            char cell = board.GetCellInfo(r, c);
            if (cell != player && cell != heavy)
            {
                return count;
            }
            count++;
            r = r + rowStep;
            c = c + colStep;
        }
        return count;
    }
    private char HeavySymbolOf(char player)
    {
        if (player == Piece.ordinary[0])
        {
            return Piece.gomokuHeavy[0];
        }
        return Piece.gomokuHeavy[1];
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