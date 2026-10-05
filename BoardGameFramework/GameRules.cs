public abstract class GameRules
{
    public abstract bool CheckForWinner(Board board, int row, int col);
}

public class GomokuRules : GameRules
{
    public override bool CheckForWinner(Board board, int r, int c)
    {
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
                    return true;
                }
            }
            return false;
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
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }
}

/*
public class ReversiRules : GameRules
{
    // TO-DO
}
*/