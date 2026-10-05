using static System.Console;

public abstract class Game
{
    // Properties 
    private GameConfig config;
    public int turnCount { get; private set; }
    public Board board { get; private set; }
    public Player p1 { get; private set; }
    public Player p2 { get; private set; }

    // Constructor 
    public Game(GameConfig config, Board board)
    {
        this.config = config;
        this.board = board;
        p1 = new HumanPlayer();
        p2 = CreatePlayer2();
        turnCount = 1;
    }

    private Player CreatePlayer2()
    {
        if (config.GameMode == GameMode.HumanVsHuman)
        {
            return new HumanPlayer();
        }
        else if (config.ComputerType == ComputerType.DumbAI)
        {
            return new DumbAI();
        }
        else
        {
            return new SmartAI();
        }
    }

    public void Start()
    {
        // Initialise variables
        Player currentP;
        char piece;
        int rowVal = 0;
        int colVal = 0;
        bool hasWinner = false;
        
        DisplayGameStart();
        DisplayBoard();

        while(hasWinner == false)
        {
            // Track current player
            if (turnCount % 2 != 0)
            {
                currentP = p1;
                WriteLine($"Turn {turnCount}: Player 1");
            }
            else
            {
                currentP = p2;
                WriteLine($"Turn {turnCount}: Player 2");
            }
            
            bool validMove = false;
            while(validMove == false)
            {
                string? move = currentP.GetMove(config, board);
                if (move == "help")
                {
                    // TO-DO 
                    // Display help menu
                    continue;
                }

                // Extract piece type and coordinates for validation
                piece = move[0];
                string[] coor = move.Substring(1).Split(':');
                int.TryParse(coor[0], out rowVal);
                int.TryParse(coor[1], out colVal);

                // Check that piece type and move is valid
                if (ValidatePieceType(piece))
                {
                    if (ValidateMove(piece, rowVal, colVal))
                    {
                        PlayMove(currentP, piece, rowVal, colVal);
                        validMove = true;
                    }
                }
            }

            DisplayBoard();

            if (CheckForWinner(rowVal, colVal))
            {
                WriteLine("***GAME OVER***");
                if (currentP == p1)
                    WriteLine($"Player 1 Wins!");
                else
                    WriteLine($"Player 2 Wins!");
                hasWinner = true;
            }

            turnCount++;
        }
    }

    private void DisplayGameStart()
    {
        WriteLine("-----------------------");
        WriteLine("GAME START");

        // Display Game Variant
        if (config.GameFamily == GameFamily.Gomoku)
        {
            WriteLine($"Game Variant: {config.GameFamily} {config.GomokuVariant}");
        }
        else
        {
            WriteLine($"Game Variant: {config.ReversiVariant} {config.GameFamily}");
        }

        // Display Game Mode
        if (config.GameMode == GameMode.HumanVsHuman)
        {
            WriteLine($"Game Mode: Human Vs Human");
        }
        else
        {
            WriteLine($"Game Mode: Human Vs {config.ComputerType}");
        }
    }

    protected virtual void DisplayBoard()
    {
        board.GetBoard();
    }

    protected abstract bool ValidatePieceType(char c);

    protected virtual bool ValidateMove(char c, int row, int col)
    {
        if (board.GetCellInfo(row, col) == ' ')
        {
            return true;
        }
        else
        {
            WriteLine("Invalid move. Cannot play move on an occupied cell.");
            return false;
        }
    }

    protected virtual void PlayMove(Player player, char piece, int row, int col)
    {
        if (player == p1)
        {
            board.PlayMove('X', row, col);
        }
        else
        {
            board.PlayMove('O', row, col);
        }
    }

    public abstract bool CheckForWinner(int r, int c);
}


public class GomokuGame : Game
{
    public GomokuGame(GameConfig config) : base(config, new Board(10))
    {
    }

    //private readonly GomokuRules rule = new GomokuRules();

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

    public override bool CheckForWinner(int r, int c)
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


public class GomokuPlus : GomokuGame
{
    public GomokuPlus(GameConfig config) : base(config)
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
}


public class ReversiGame : Game
{
    public ReversiGame(GameConfig config) : base(config, new Board(8))
    {
    }

    protected override bool ValidatePieceType(char c)
    {
        if (c == 'P')
        {
            return true;
        }
        else
        {
            WriteLine("Invalid Disk type. Use [P] for disk.");
            return false;
        }
    }

    public override bool CheckForWinner(int r, int c)
    {
        // TO-DO
        throw new NotImplementedException("Reversi CheckForWinner is not yet implemented");
    }
}