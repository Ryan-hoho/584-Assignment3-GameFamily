using static System.Console;

public class Game
{
    // Properties 
    private GameConfig config;
    public int Turn { get; private set; }
    public Board board { get; private set; }
    public Player p1 { get; private set; }
    public Player p2 { get; private set; }

    // Constructor 
    public Game(GameConfig config)
    {
        this.config = config;
        board = CreateBoard();
        p1 = new HumanPlayer();
        p2 = CreatePlayer2();
        Turn = 1;
    }

    public void Start()
    {
        // Initiate variables
        Player currentP;
        char c;
        int rowVal = 0;
        int colVal = 0;
        bool hasWinner = false;
        
        DisplayGameStart();

        while(hasWinner == false)
        {
            // Track current player
            if (Turn % 2 != 0)
            {
                currentP = p1;
                WriteLine($"Turn {Turn}: Player 1");
            }
            else
            {
                currentP = p2;
                WriteLine($"Turn {Turn}: Player 2");
            }

            // Validate input syntax and check if move is legal
            bool validMove = false;
            while(validMove == false)
            {
                // Get move from player and validate syntax where applicable
                string? move = currentP.GetMove(config, board);

                // Extract stone/disk type and coordinates for validation
                c = move[0];
                string[] coor = move.Substring(1).Split(':');
                int.TryParse(coor[0], out rowVal);
                int.TryParse(coor[1], out colVal);

                if (config.GomokuVariant != GomokuVariant.Plus)
                {
                    if (currentP == p1)
                    {
                        board.PlayMove(Piece.ordinary[0], rowVal, colVal);
                    }
                    else
                    {
                        board.PlayMove(Piece.ordinary[1], rowVal, colVal);
                    }
                    validMove = true;
                }
                // Separate validation logic for Gomoku Plus because of special stones rule
                else
                {
                    // TO-DO
                    // ValidateMove();
                }
            }

            // Print updated board
            board.DisplayBoard();

            // Check for winner
            if (config.GameType == GameType.Gomoku)
            {
                if (CheckForGomokuWinner(rowVal, colVal))
                {
                    WriteLine("***GAME OVER***");
                    if (currentP == p1)
                        WriteLine($"Player 1 Wins!");
                    else
                        WriteLine($"Player 2 Wins!");
                    hasWinner = true;
                }
            }

            // Increment turn after each loop
            Turn++;
        }
    }

    /*private bool ValidateMove(Player p, char l, int r, int c)
    {
        // TO-DO
    }*/

    private bool CheckForGomokuWinner(int r, int c)
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


    private Board CreateBoard()
    {
        if (config.GameType == GameType.Gomoku)
        {
            return new Board(10);
        }
        else
        {
            return new Board(8);
        }
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

    private void DisplayGameStart()
    {
        WriteLine("-----------------------");
        WriteLine("GAME START");

        // Display Game Variant
        if (config.GameType == GameType.Gomoku)
        {
            WriteLine($"Game Variant: {config.GameType} {config.GomokuVariant}");
        }
        else
        {
            WriteLine($"Game Variant: {config.ReversiVariant} {config.GameType}");
        }

        // Display Game Mode
        if (config.GameMode == GameMode.HumanVsHuman)
        {
            WriteLine($"Game Mode: {config.GameMode}");
        }
        else
        {
            WriteLine($"Game Mode: HumanVs{config.ComputerType}");
        }

        // if (config.GomokuVariant == GomokuVariant.GomokuFog)
            // display Fog Board
        // else display Board
        board.DisplayBoard();
    }
}