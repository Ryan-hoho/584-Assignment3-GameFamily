public static class FlankingRules
{
    private static readonly (int dr, int dc)[] directions =
    {
        (-1, -1), // upper-left
        (-1,  0), // up
        (-1,  1), // upper-right
        ( 0, -1), // left
        ( 0,  1), // right
        ( 1, -1), // lower-left
        ( 1,  0), // down
        ( 1,  1)  // lower-right
    };

    public static List<(int row, int col)> FindFlips(
        Board board,
        int row,
        int col,
        char currentSymbol)
    {
        List<(int row, int col)> allFlips =
            new List<(int row, int col)>();

        // The target position must be inside the board.
        if (!IsOnBoard(board, row, col))
        {
            return allFlips;    //If outside --> Illegal Place --> return 'Empty' flips
        }

        // A disk can only be placed on an empty cell.
        if (board.GetCellInfo(row, col) != ' ')
        {
            return allFlips;    //If has been occupied --> Illegal Place --> return 'Empty' flips
        }

        // Reversi uses X for Player 1 and O for Player 2.
        if (currentSymbol != Piece.ordinary[0] &&
            currentSymbol != Piece.ordinary[1])
        {
            return allFlips;
        }

        char opponentSymbol;

        if (currentSymbol == Piece.ordinary[0])
        {
            opponentSymbol = Piece.ordinary[1];
        }
        else
        {
            opponentSymbol = Piece.ordinary[0];
        }

        // Check all eight directions from the proposed move.
        foreach ((int dr, int dc) in directions)
        {
            List<(int row, int col)> lineFlips =
                new List<(int row, int col)>();

            int currentRow = row + dr;
            int currentCol = col + dc;

            while (IsOnBoard(board, currentRow, currentCol))
            {
                char cell =
                    board.GetCellInfo(currentRow, currentCol);

                // Opponent disk: remember it and continue looking.
                if (cell == opponentSymbol)
                {
                    lineFlips.Add((currentRow, currentCol));

                    currentRow += dr;
                    currentCol += dc;

                    continue;
                }

                // Own disk after at least one opponent disk:
                // the opponent disks are successfully flanked.
                if (cell == currentSymbol &&
                    lineFlips.Count > 0)
                {
                    allFlips.AddRange(lineFlips);
                }

                // Empty cell or own disk means this direction is finished.
                break;
            }
        }

        return allFlips;
    }

    private static bool IsOnBoard(
        Board board,
        int row,
        int col)
    {
        return row >= 1 &&
               row <= board.Size &&
               col >= 1 &&
               col <= board.Size;
    }
}