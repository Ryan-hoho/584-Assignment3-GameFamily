public class GomokuRandomStrategy : IMoveStrategy
{
    const char EMPTYCELL = ' ';
    private readonly Random random = new Random();

    public string ChooseMove(Board board, char symbol)
    {
        List<string> legalMoves = new List<string>();
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                if (board.GetCellInfo(row, col) == EMPTYCELL)
                {
                    legalMoves.Add($"{Piece.gomokuStone[0]}{row}:{col}");
                }
            }
        }
        if (legalMoves.Count == 0)
        {
            throw new InvalidOperationException("The board is full. There is no legal move.");
        }
        return legalMoves[random.Next(0, legalMoves.Count)];
    }
}
