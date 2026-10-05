public class FogVisibility : IVisibilityStrategy
{
    const char HIDDEN = '?';
    public char[,] GetVisibleBoard(Board board, char symbol)
    {
        char[,] view = new char[board.Size, board.Size];
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                if (IsVisible(board, row, col, symbol))
                {
                    view[row - 1, col - 1] = board.GetCellInfo(row, col);
                }
                else
                {
                    view[row - 1, col - 1] = HIDDEN;
                }
            }
        }
        return view;
    }

    private bool IsVisible(Board board, int row, int col, char symbol)
    {
        for (int r = row - 1; r <= row + 1; r++)
        {
            for (int c = col - 1; c <= col + 1; c++)
            {
                if (r >= 1 && r <= board.Size && c >= 1 && c <= board.Size)
                {
                    if (board.GetCellInfo(r, c) == symbol)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }
}