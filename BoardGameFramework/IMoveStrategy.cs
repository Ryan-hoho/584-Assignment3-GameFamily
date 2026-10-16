public interface IMoveStrategy
{
    string ChooseMove(Board board, char symbol);
}

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

public class GomokuPlusRandomStrategy : IMoveStrategy
{
    const char EMPTYCELL = ' ';
    private readonly Random random = new Random();
    private readonly StoneInventory inventory;

    public GomokuPlusRandomStrategy(StoneInventory inventory)
    {
        this.inventory = inventory;
    }

    public string ChooseMove(Board board, char symbol)
    {
        int player = (symbol == Piece.ordinary[0]) ? 0 : 1;
        char opponentStone = Piece.ordinary[1 - player];
        List<string> legalMoves = new List<string>();
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                char cell = board.GetCellInfo(row, col);
                if (cell == EMPTYCELL)
                {
                    legalMoves.Add($"{Piece.gomokuStone[0]}{row}:{col}");
                    if (inventory.HeavyLeft(player) > 0)
                    {
                        legalMoves.Add($"{Piece.gomokuStone[1]}{row}:{col}");
                    }
                }
                else if (cell == opponentStone && inventory.EraserLeft(player) > 0)
                {
                    legalMoves.Add($"{Piece.gomokuStone[2]}{row}:{col}");
                }
            }
        }
        if (legalMoves.Count == 0)
        {
            throw new InvalidOperationException("There is no legal move.");
        }
        return legalMoves[random.Next(0, legalMoves.Count)];
    }
}