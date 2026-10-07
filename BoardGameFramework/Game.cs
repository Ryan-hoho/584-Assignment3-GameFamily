using static System.Console;

public abstract class Game
{
    // Properties 
    private GameConfig config;
    private CommandParser parser;
    protected CommandHistory history;
    
    public int turnCount { get; private set; }
    public Board board { get; private set; }
    public Player p1 { get; private set; }
    public Player p2 { get; private set; }
    
    // player record
    public Player currentPlayer { get; private set; }

    // Constructor 
    public Game(GameConfig config, Board board)
    {
        this.config = config;
        this.board = board;
        p1 = new HumanPlayer();
        p2 = CreatePlayer2();
        turnCount = 1;
        currentPlayer = p1;

        parser = new CommandParser();
        history = new CommandHistory();
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
        char piece;
        int rowVal = 0;
        int colVal = 0;
        bool hasWinner = false;
        
        DisplayGameStart();
        DisplayBoard();

        while(hasWinner == false)
        {
            // Track current player P1+P2 = 1 turn for history
            if (currentPlayer == p1)
            {
                WriteLine($"Turn {turnCount}: Player 1");
            }
            else
            {
                WriteLine($"Turn {turnCount}: Player 2");
            }
            
            bool validMove = false;
            while(validMove == false)
            {
                string move = currentPlayer.GetMove(config, board);
                try{
                    ParsedCommand parsed = parser.Parse(move);
                    if (parsed.Type == CommandType.Help)
                    {
                        DisplayHelp();
                        continue;
                    }
                    
                    if (parsed.Type == CommandType.Undo)
                    {
                        bool undoSuccess = history.Undo();
                        if (!undoSuccess)
                        {
                            WriteLine("Nothing to undo. A full turn (both players) is required before undo is available.");
                        }
                        else
                        {
                            // Undo one full turn (two moves).
                            // Keep the current player unchanged.
                            turnCount--;
                            WriteLine("Undo successful.");
                            DisplayBoard();
                            // Keep the active player unchanged after undo.
                            // Display the updated turn and player.
                            if (currentPlayer == p1)
                            {
                                WriteLine($"Turn {turnCount}: Player 1");
                            }
                            else
                            {
                                WriteLine($"Turn {turnCount}: Player 2");
                            }
                        }
                        continue;
                    }
                    if (parsed.Type == CommandType.Redo)
                    {
                        bool redoSuccess = history.Redo();
                        if (!redoSuccess)
                        {
                            WriteLine("Nothing to redo.");
                        }
                        else
                        {
                            turnCount++;
                            WriteLine("Redo successful.");
                            DisplayBoard();
    
                            if (currentPlayer == p1)
                            {
                                WriteLine($"Turn {turnCount}: Player 1");
                            }
                            else
                            {
                                WriteLine($"Turn {turnCount}: Player 2");
                            }
                        }
                        continue;
                    }
                    if (parsed.Type == CommandType.Save)
                    {
                        WriteLine("Save is not implemented yet.");
                        continue;
                    }
                    if (parsed.Type == CommandType.Load)
                    {
                        WriteLine("Load is not implemented yet.");
                        continue;
                    }
                    if (parsed.Type == CommandType.Quit)
                    {
                        WriteLine("Game ended.");
                        return;
                    }
                    if (parsed.Type == CommandType.Move)
                    {
                        piece = parsed.PieceCode!.Value;
                        rowVal = parsed.Row!.Value;
                        colVal = parsed.Column!.Value;
                        // shared Board boundary validation。
                        if (rowVal < 1 || rowVal > board.Size ||
                            colVal < 1 || colVal > board.Size)
                        {
                            WriteLine("Invalid move. Coordinates are off-grid.");
                            continue;
                        }

                        // Game / Rules for rule's validation。
                        if (ValidatePieceType(piece))
                        {
                            if (ValidateMove(piece, rowVal, colVal))
                            {
                                PlayMove(
                                    currentPlayer,
                                    piece,
                                    rowVal,
                                    colVal
                                );
                                validMove = true;
                            }
                        }
                    }
                }
                catch (InvalidCommandFormatException ex)
                {
                    WriteLine(ex.Message);
                }
            }

            DisplayBoard();

            if (CheckForWinner(rowVal, colVal))
            {
                WriteLine("***GAME OVER***");
                if (currentPlayer == p1)
                   {WriteLine($"Player 1 Wins!");} 
                else
                    {WriteLine($"Player 2 Wins!");}
                hasWinner = true;
            }

            if (!hasWinner){
                // Player 1 finished move, stay at same turn for player 2
                if (currentPlayer == p1)
                {
                    currentPlayer = p2;
                }
                // Player 2 finished move, change player
                else
                {
                    currentPlayer = p1;
                    turnCount++;
                }
            }
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

    protected virtual void DisplayHelp()
    {
        WriteLine();
        WriteLine("=== Commands ===");
        WriteLine("help        - Display help");
        WriteLine("undo        - Undo the previous full turn");
        WriteLine("redo        - Redo the previous full turn");
        WriteLine("save        - Save the current game");
        WriteLine("load        - Load a saved game");
        WriteLine("quit        - Quit the game");
        WriteLine();
        WriteLine("=== Game Rules ===");
        WriteLine(GetRulesText());
    }


    protected virtual void DisplayBoard()
    {
        board.GetBoard();
    }

    protected abstract string GetRulesText();
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


    protected abstract void PlayMove(Player player, char piece, int row, int col);
    public abstract bool CheckForWinner(int r, int c);



}

