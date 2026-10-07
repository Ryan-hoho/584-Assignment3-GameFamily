using static System.Console;

public abstract class Player
{
    public abstract string GetMove(GameConfig config, Board board);
}

public class HumanPlayer : Player
{
    public override string GetMove(GameConfig config, Board board)
    {
        Write("Enter your move or command ('help' for available commands): ");
        return ReadLine() ?? string.Empty;
            
    }
}

public class DumbAI : Player
{
    public override string GetMove(GameConfig config, Board board)
    {
        Random rnd = new Random();
        char c;
        if (config.GameFamily == GameFamily.Reversi)
        {
            c = Piece.reversiDisk[0];
        }
        else if (config.GomokuVariant == GomokuVariant.Plus)
        {
            c = Piece.gomokuStone[rnd.Next(0, Piece.gomokuStone.Length)];
        }
        else
        {
            c = Piece.gomokuStone[0];
        }
        string rowVal = Convert.ToString(rnd.Next(1, board.Size + 1));
        string colVal = Convert.ToString(rnd.Next(1, board.Size + 1));
        string move = c + rowVal + ":" + colVal;
        return move;
    }
}

public class SmartAI : Player
{
    public override string GetMove(GameConfig config, Board board)
    {
        throw new NotImplementedException("Smart AI is not implemented yet.");
    }
}
