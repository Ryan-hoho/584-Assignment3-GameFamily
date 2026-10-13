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
        if (c == 'O')
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
    public GomokuPlus(
        GameConfig config, Board board, Player p1, Player p2) 
        : base(config, board, p1, p2)
    {
    }
    
    protected override bool ValidatePieceType(char c)
    {
        if (c == 'O' || c == 'H' || c == 'E')
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
        // TO-DO
        throw new NotFiniteNumberException("GomokuPlus ValidateMove is not yet implemented.");
    }

    protected override void PlayMove(Player player, char piece, int row, int col)
    {
        // TO-DO
        throw new NotImplementedException("GomokuPlus PlayMove is not yet implemented.");
    }
    protected override string GetRulesText()
    {
        return
            "Gomoku Plus Rules:\n" +
            "- Players take turns placing stones.\n" +
            "- The first player to connect five stones in a row wins.\n" +
            "\n" +
            "MOVE FORMAT\n" +
            "O5:5        Place an ordinary stone\n" +
            "H5:5        Heavy stone\n" +
            "E5:5        Eraser stone";
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
