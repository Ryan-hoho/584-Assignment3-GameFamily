public static class ReversiRules
{
    public static void SetUpInitialBoard(Board board)
    {
        board.PlayMove('O', 4, 4);
        board.PlayMove('X', 4, 5);
        board.PlayMove('X', 5, 4);
        board.PlayMove('O', 5, 5);
    }
    public static bool PlaceDisk(
    Board board,
    int row,
    int col,
    char currentSymbol)
    {
        List<(int row, int col)> flips =
            FlankingRules.FindFlips(
                board,
                row,
                col,
                currentSymbol);

        if (flips.Count == 0)
        {
            return false;
        }

        // Place the new disk.
        board.PlayMove(currentSymbol, row, col);

        // Flip all captured opponent disks.
        foreach ((int flipRow, int flipCol) in flips)
        {
            board.PlayMove(
                currentSymbol,
                flipRow,
                flipCol);
        }

        return true;
    }

    //Get all candidate 'LegalMove' by repeatedly invoking 'FindFlips()' to each cell(8*8)
    public static List<(int row, int col)> FindLegalMoves(
    Board board,
    char currentSymbol)
    {
        List<(int row, int col)> legalMoves =
            new List<(int row, int col)>();

        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                List<(int row, int col)> flips =
                    FlankingRules.FindFlips(
                        board,
                        row,
                        col,
                        currentSymbol);

                if (flips.Count > 0)
                {
                    legalMoves.Add((row, col));
                }
            }
        }

        return legalMoves;
    }
}