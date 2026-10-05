public class FullVisibility : IVisibilityStrategy
{
    public char[,] GetVisibleBoard(Board board, char symbol)
    {
        char[,] view = new char[board.Size, board.Size];
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                view[row - 1, col - 1] = board.GetCellInfo(row, col);
            }
        }
        return view;
    }
}