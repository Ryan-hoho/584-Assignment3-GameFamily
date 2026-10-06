public class ReversiRandomStrategy : IMoveStrategy
{
    const string PASS = "PASS";
    private readonly Random random = new Random();
    public string ChooseMove(Board board, char symbol)
    {
        List<string> legalMoves = new List<string>();
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                int flips = FlankingRules.FindFlips(board, row, col, symbol).Count;
                if (flips > 0)
                {
                    legalMoves.Add($"{Piece.reversiDisk[0]}{row}:{col}");
                }
            }
        }
        if (legalMoves.Count == 0)
        {
            return PASS;
        }
        return legalMoves[random.Next(0, legalMoves.Count)];
    }
}
