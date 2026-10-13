using static System.Console;

public enum GameResult
{
    InProgress, 
    Player1Wins,
    Player2Wins,
    Draw
}

public abstract class Game
{
    // Properties 
    private GameConfig config;
    private CommandParser parser;
    private GameStore gameStore;
    private IGameObserver observer;  //move board presentation to ConsoleView. maybe need to rename .cs
    protected CommandHistory history;

    public int turnCount { get; private set; }
    public Board board { get; private set; }
    public Player p1 { get; private set; }
    public Player p2 { get; private set; }
    public Player currentPlayer { get; private set; }

    // Constructor 
    public Game(GameConfig config, Board board, Player p1, Player p2)
    {
        this.config = config;
        this.board = board;
        this.p1 = p1;
        this.p2 = p2;
        turnCount = 1;
        currentPlayer = p1;

        parser = new CommandParser();
        history = new CommandHistory();
        gameStore = new GameStore(parser);
        // ConsoleView is the current presentation observer.
        observer = new ConsoleView();
    }

    public Game? Start()
    {
        // Initialise variables
        char piece;
        int rowVal = 0;
        int colVal = 0;
        bool gameEnded = false;
        
        // Presentation is delegated to the observer.
        observer.OnGameStarted(this);
        observer.OnBoardChanged(this);

        // Repeatedly prompting for move if game hasn't ended
        while(gameEnded == false)
        {
            // Display the current turn.
            observer.OnTurnChanged(this);

            bool turnCompleted = false;
            while (turnCompleted == false)
            {
                // Prompt for move from player
                string move = currentPlayer.GetMove(board);

                try
                {
                    ParsedCommand parsed = parser.Parse(move);
                    
                    // Help
                    if (parsed.Type == CommandType.Help)
                    {
                        observer.OnHelpRequested(GetRulesText());
                        continue;
                    }

                    // Undo
                    if (parsed.Type == CommandType.Undo)
                    {
                        bool undoSuccess = history.Undo();
                        if (!undoSuccess)
                        {
                            observer.OnMessage(
                                "Nothing to undo. " +
                                "A full turn (both players) " +
                                "is required before undo is available."
                            );
                        }
                        else
                        {
                            turnCount--;
                            observer.OnMessage("Undo successful.");
                            observer.OnBoardChanged(this);
                            observer.OnTurnChanged(this);
                        }
                        continue;
                    }

                    // -----------------------------------------
                    // Redo
                    // -----------------------------------------

                    if (parsed.Type == CommandType.Redo)
                    {
                        bool redoSuccess =
                            history.Redo();
                        if (!redoSuccess)
                        {
                            observer.OnMessage("Nothing to redo.");
                        }

                        else
                        {
                            turnCount++;
                            observer.OnMessage("Redo successful.");
                            observer.OnBoardChanged(this);
                            observer.OnTurnChanged(this);
                        }
                        continue;
                    }

                    // -----------------------------------------
                    // Save
                    // -----------------------------------------
                    // Save-name input remains here temporarily.It can later move to "****TBC" 
                    // ConsoleCommandSource.

                    if (parsed.Type == CommandType.Save)
                    {
                        try
                        {
                            Write("Enter save name: ");
                            string saveName =
                                ReadLine()?.Trim() ?? "";
                            if (string.IsNullOrWhiteSpace(saveName))
                            {
                                observer.OnMessage(
                                    "Save cancelled. " +
                                    "Save name cannot be empty."
                                );
                                continue;
                            }

                            if (!gameStore.IsValidSaveName(saveName))
                            {
                                observer.OnMessage(
                                    "Invalid save name. " +
                                    "Use only letters, numbers, '-' or '_'."
                                );
                                continue;
                            }

                            SaveData saveData =
                                gameStore.CreateSaveData(
                                    this,
                                    config);

                            gameStore.Save(
                                saveData,
                                saveName);

                            observer.OnMessage($"Game saved successfully as '{saveName}'.");
                            observer.OnMessage($"Save folder: {gameStore.GetSaveFolder()}");
                        }
                        catch (Exception ex)
                        {
                            observer.OnMessage($"Unable to save game: {ex.Message}");
                        }
                        continue;
                    }

                    // -----------------------------------------
                    // Load
                    // -----------------------------------------

                    if (parsed.Type == CommandType.Load)
                    {
                        try
                        {
                            Game? loadedGame = GameSelection.LoadGame();
                            if (loadedGame == null)
                            {
                                continue;   // 使用者按 0 返回，留在目前的遊戲
                            }
                            observer.OnMessage("Game loaded successfully.");

                            // Return the loaded game to Program.
                            // Program will start its game lifecycle.
                            return loadedGame;
                        }
                        catch (Exception ex)
                        {
                            observer.OnMessage($"Unable to load game: {ex.Message}");
                            continue;
                        }
                    }

                    // -----------------------------------------
                    // Quit
                    // -----------------------------------------
                    if (parsed.Type == CommandType.Quit)
                    {
                        observer.OnMessage("Game ended.");
                        return null;
                    }
                    // -----------------------------------------
                    // Pass (NEW)
                    // -----------------------------------------
                    if (parsed.Type == CommandType.Pass)
                    {
                        if (TryPass())
                        {
                            turnCompleted = true;
                        }
                        else
                        {
                            observer.OnMessage(
                                this is ReversiGame
                                    ? "PASS is not allowed. Legal moves are available."
                                    : "PASS is only available in Reversi.");
                        }
                        continue;
                    }

                    // -----------------------------------------
                    // Move
                    // -----------------------------------------
                    if (parsed.Type == CommandType.Move)
                    {
                        piece = parsed.PieceCode!.Value;
                        rowVal = parsed.Row!.Value;
                        colVal = parsed.Column!.Value;

                        // Shared board-boundary validation.
                        if (TryApplyMove(piece, rowVal, colVal))
                        {
                            turnCompleted = true;  
                        }  
                        continue;
                    }
                }
                catch (InvalidCommandFormatException ex)
                {
                    observer.OnMessage(ex.Message);
                }
            }

            // Winner Check Logic
            GameResult result = CheckForWinner(rowVal, colVal);

            // If game hasn't ended, advance to the next turn
            if (result == GameResult.InProgress)
            {
                AdvanceTurn();
            }

            // Display updated board or final board
            observer.OnBoardChanged(this);

            // Announce result 
            if (result == GameResult.Player1Wins || result == GameResult.Player2Wins)
            {
                int winnerNumber = result == GameResult.Player1Wins ? 1 : 2;
                observer.OnGameOver(this, winnerNumber);
            }
            else if (result == GameResult.Draw)
            {
                observer.OnMessage("Game over. It's a draw.");
            }
            gameEnded = result != GameResult.InProgress;
        }
        return null;
    }

    // =========================================================
    // Template Method Hooks
    // =========================================================
    // Each game family provides its own rule/help text.
    protected abstract string GetRulesText();
    
    // Each game family decides which piece types are valid.
    protected abstract bool ValidatePieceType(char c);
    
    // Default move validation. Subclasses can override this for family-specific rules.
    protected virtual bool ValidateMove(char c, int row, int col)
    {
        if (board.GetCellInfo(row, col) == ' ')
        {
            return true;
        }
        observer.OnMessage("Invalid move. Cannot play move on an occupied cell.");
        return false;
    }

    // Family-specific move execution.
    protected abstract void PlayMove(Player player, char piece, int row, int col);

    // Family-specific winner evaluation.
    public abstract GameResult CheckForWinner(int r, int c);
  
    public virtual bool TryPass()  // pass validation, override in Reversi game
    {
        return false;
    }


    // =========================================================
    // Persistence Support
    // =========================================================
    // Allows GameStore to replay a saved move
    // without exposing PlayMove directly.
    public void ReplayMove(
        Player player,
        char piece,
        int row,
        int col)
    {
        PlayMove(
            player,
            piece,
            row,
            col
        );
    }

    // Restores turn state after replaying saved moves.
    public void SetTurnState(
        int turnCount,
        int currentPlayerNumber)
    {
        this.turnCount = turnCount;
        this.currentPlayer =
            currentPlayerNumber == 1 ? p1 : p2;
    }
    // Provides read-only access to executed commands
    public IReadOnlyList<MoveCommand> GetMoveHistory()
    {
        return history.GetUndoHistory();
    }

    // Allows GameStore to rebuild history during load.
    public void ClearHistory()
    {
        history.Clear();
    }
     // Allows GameStore to replay a saved PASS.
    public virtual void ReplayPass(Player player)
    {
        throw new NotSupportedException("PASS is not supported in this game.");
    }


    // =========================================================
    // TestRunner Support
    // =========================================================
    public bool TryApplyMove(char piece, int row, int col) // shared logic with testrunner
    {
        if (row < 1 || row > board.Size || col < 1 || col > board.Size)
        {
            observer.OnMessage("Invalid move. Coordinates are off-grid.");
            return false;
        }
        if (!ValidatePieceType(piece) || !ValidateMove(piece, row, col))
        {
            return false;
        }
        PlayMove(currentPlayer, piece, row, col);
        return true;
    }

    public void AdvanceTurn()
    {
        if (currentPlayer == p1)
        {
            currentPlayer = p2;
        }
        else
        {
            currentPlayer = p1;
            turnCount++;
        }
    }

    public void RenderBoard()
    {
        observer.OnBoardChanged(this);
    }

    public virtual IVisibilityStrategy GetVisibilityStrategy()
    {
        return new FullVisibility();
    }

}
