using static System.Console;

public class Player
{
    public virtual string GetMove(GameConfig config, Board board)
    {
        return "";
    }
}

public class HumanPlayer : Player
{
    public override string GetMove(GameConfig config, Board board)
    {
        string? input = string.Empty;
        bool validInput = false;
        while (validInput == false)
        {
            Write("Enter your move or 'help' to access Help Menu: ");
            input = ReadLine() ?? string.Empty;
            if (string.IsNullOrEmpty(input))
            {
                WriteLine("Input is empty.");
            }
            else if (input == "help")
            {
                GetHelpMenu(config);
            }
            else
            {
                // Separate stone and coordinations
                char c = input[0];
                string[] coor = input.Substring(1).Split(':');

                // Validate input syntax
                if (ValidateCharacterType(config, c) == false)
                {
                    WriteLine("Invalid input. Please select a valid stone/disk type. Syntax: Stone[Row]:[Col]");
                }
                else if (coor.Length != 2)
                {
                    WriteLine("Invalid input. Syntax: Stone[Row]:[Col]");
                }
                else if (int.TryParse(coor[0], out int rowVal) == false || int.TryParse(coor[1], out int colVal) == false)
                {
                    WriteLine("Invalid input. Coordinates should be numbers. Syntax: Stone[Row]:[Col]");
                }
                else if (rowVal < 1 || rowVal > board.Size || colVal < 1 || colVal > board.Size)
                {
                    WriteLine("Invalid move. Coordinates are off-grid.");
                }
                else if (board.GetCellInfo(rowVal, colVal) != ' ')
                {
                    WriteLine("Invalid move. Cannot play on an occupied cell.");
                }
                else
                {
                    validInput = true;
                }
            }
        }
        return input;
    }

    private bool ValidateCharacterType(GameConfig config, char c)
    {
        if (config.GameFamily == GameFamily.Reversi && Piece.reversiDisk.Contains(c))
        {
            return true;
        }
        else if ((config.GomokuVariant == GomokuVariant.Standard ||
                config.GomokuVariant == GomokuVariant.Fog) && c == Piece.gomokuStone[0])
        {
            return true;
        }
        else if (config.GomokuVariant == GomokuVariant.Plus && Piece.gomokuStone.Contains(c))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void GetHelpMenu(GameConfig config)
    {
        // TO-DO
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
    // TO-DO
}
