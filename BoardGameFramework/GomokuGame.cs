using static System.Console;

public class GomokuGame : Game
{
    public GomokuGame(
        GameConfig config, Board board, Player p1, Player p2) 
        : base(config, board, p1, p2)
    {
    }

    protected override void PlayMove(Player player, char piece, int row, int col)
    {
        char symbol = (player == p1) ? Piece.ordinary[0] : Piece.ordinary[1];
        MoveCommand command = new GomokuMoveCommand(board, symbol, row, col);
        history.Execute(command);
    }

    protected override bool ValidatePieceType(char c)
    {
        if (c == Piece.gomokuStone[0])
        {
            return true;
        }
        else
        {
            WriteLine("Invalid Stone type. Only Ordinary stones [O] allowed.");
            return false;
        }
    }

    public override GameResult CheckForWinner(int r, int c)
    {
        // Check if player 1 wins
        (int dr, int dc)[] directions = {(1,0), (0,1), (1,1), (1,-1)};
        if (board.GetCellInfo(r,c) == Piece.ordinary[0] || board.GetCellInfo(r,c) == Piece.gomokuHeavy[0])
        {
            foreach ((int dr, int dc) in directions)
            {
                int point = 1;
                // Check one direction from the current stone position
                int nr = r + dr;
                int nc = c + dc;
                while (nr > 0 && nr < board.Size + 1 && nc > 0 && nc < board.Size + 1)
                {
                    if (board.GetCellInfo(nr,nc) == Piece.ordinary[0] || board.GetCellInfo(nr,nc) == Piece.gomokuHeavy[0])
                    {
                        point++;
                    }   
                    else
                    {
                        break;
                    }
                    nr += dr;
                    nc += dc;
                }
                // Check the opposite direction
                nr = r - dr;
                nc = c - dc;
                while (nr > 0 && nr < board.Size + 1 && nc > 0 && nc < board.Size + 1)
                {
                    if (board.GetCellInfo(nr,nc) == Piece.ordinary[0] || board.GetCellInfo(nr,nc) == Piece.gomokuHeavy[0])
                    {
                        point++;
                    }    
                    else
                    {
                        break;
                    }
                    nr -= dr;
                    nc -= dc;
                }
                if (point == 5 || point > 5)
                {
                    return GameResult.Player1Wins;
                }
            }
            return GameResult.InProgress;
        }
        else if (board.GetCellInfo(r,c) == Piece.ordinary[1] || board.GetCellInfo(r,c) == Piece.gomokuHeavy[1])
        {
            foreach ((int dr, int dc) in directions)
            {
                int point = 1;
                // Check one direction from the current stone position
                int nr = r + dr;
                int nc = c + dc;
                while (nr > 0 && nr < board.Size + 1 && nc > 0 && nc < board.Size + 1)
                {
                    if (board.GetCellInfo(nr,nc) == Piece.ordinary[1] || board.GetCellInfo(nr,nc) == Piece.gomokuHeavy[1])
                    {
                        point++;
                    }
                    else
                    {
                        break;
                    }
                    nr += dr;
                    nc += dc;
                }
                // Check the opposite direction
                nr = r - dr;
                nc = c - dc;
                while (nr > 0 && nr < board.Size + 1 && nc > 0 && nc < board.Size + 1)
                {
                    if (board.GetCellInfo(nr,nc) == Piece.ordinary[1] || board.GetCellInfo(nr,nc) == Piece.gomokuHeavy[1])
                    {
                        point++;
                    }
                    else
                    {
                        break;
                    }
                    nr -= dr;
                    nc -= dc;
                }
                if (point == 5 || point > 5)
                {
                    return GameResult.Player2Wins;
                }
            }
            return GameResult.InProgress;
        }
        else
        {
            return GameResult.InProgress;
        }
    }

    protected override string GetRulesText()
    {
        return
            "Gomoku Rules:\n" +
            "- Players take turns placing stones.\n" +
            "- The first player to connect five stones in a row wins.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "O5:5        Place an ordinary stone";
    }
}

public class GomokuPlus : GomokuGame
{
    private readonly StoneInventory inventory;
    public GomokuPlus(
        GameConfig config, Board board, Player p1, Player p2, StoneInventory inventory) 
        : base(config, board, p1, p2)
    {
        this.inventory = inventory;
    }
    
    protected override bool ValidatePieceType(char c)
    {
        if (Piece.gomokuStone.Contains(c))
        {
            return true;
        }
        else
        {
            WriteLine("Invalid Stone type. Select Ordinary [O], Heavy [H], or Eraser [E] stones.");
            return false;
        }
    }

    protected override bool ValidateMove(char c, int row, int col)
    {
        int player = PlayerIndex(currentPlayer);

        if (c == Piece.gomokuStone[1])
        {
            if (inventory.HeavyLeft(player) == 0)
            {
                NotifyMessage("Invalid move. You have no Heavy stones left.");
                return false;
            }
            return base.ValidateMove(c, row, col);
        }

        if (c == Piece.gomokuStone[2])
        {
            if (inventory.EraserLeft(player) == 0)
            {
                NotifyMessage("Invalid move. You have no Eraser stones left.");
                return false;
            }
            char target = board.GetCellInfo(row, col);
            if (target == Piece.gomokuHeavy[0] || target == Piece.gomokuHeavy[1])
            {
                NotifyMessage("Invalid move. An Eraser cannot remove a Heavy stone.");
                return false;
            }
            if (target != Piece.ordinary[1 - player])
            {
                NotifyMessage("Invalid move. An Eraser must target an opponent's ordinary stone.");
                return false;
            }
            return true;
        }
        return base.ValidateMove(c, row, col);
    }

    protected override void PlayMove(Player player, char piece, int row, int col)
    {
        int index = PlayerIndex(player);

        if (piece == Piece.gomokuStone[1])
        {
            history.Execute(new HeavyStoneCommand(board, inventory, index, row, col));
        }
        else if (piece == Piece.gomokuStone[2])
        {
            history.Execute(new EraserCommand(board, inventory, index, row, col));
        }
        else
        {
            base.PlayMove(player, piece, row, col);
        }
    }

    private int PlayerIndex(Player player)
    {
        return (player == p1) ? 0 : 1;
    }

    protected override string GetRulesText()
    {
        return
            "Gomoku Plus Rules:\n" +
            "- Players take turns placing stones.\n" +
            "- The first player to connect five stones in a row wins.\n" +
            "- Each player has 2 Heavy and 2 Eraser stones.\n" +
            "- A Heavy stone counts as your own stone and cannot be erased.\n" +
            "- An Eraser removes one of your opponent's ordinary stones.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "O5:5        Place an ordinary stone\n" +
            "H5:5        Heavy stone\n" +
            "E5:5        Eraser stone\n" +
            "\n" +
            "SPECIAL STONES LEFT\n" +
            $"Player 1: {inventory.HeavyLeft(0)} Heavy, {inventory.EraserLeft(0)} Eraser\n" +
            $"Player 2: {inventory.HeavyLeft(1)} Heavy, {inventory.EraserLeft(1)} Eraser";
    }
}

public class GomokuFog : GomokuGame
{
    public GomokuFog(
        GameConfig config, Board board, Player p1, Player p2) 
        : base(config, board, p1, p2)
    {
    }

    //Display in consoleview, fog view is controlled  by visibility
    public override IVisibilityStrategy GetVisibilityStrategy()
    {
        return new FogVisibility();
    }

    protected override string GetRulesText()
    {
        return
            "Gomoku Fog Rules:\n" +
            "- Players take turns placing stones.\n" +
            "- The first player to connect five stones in a row wins.\n" +
            "- Parts of the board may be hidden by fog.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "O5:5        Place an ordinary stone";
    }

}
