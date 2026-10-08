using System.Text.Json;

// =========================================================
// Save Data
// Stores all data required to restore a saved game.
public class SaveData
{
    // Game configuration.
    public string GameFamily { get; set; } = "";
    public string Variant { get; set; } = "";
    public string GameMode { get; set; } = "";

    // Computer player type.
    // Empty for Human vs Human games.
    public string ComputerType { get; set; } = "";

    // Stores completed moves in execution order.
    // Example: Gomoku  -> O4:3. Reversi -> P4:3
    public List<string> MoveHistory { get; set; } = new();

    public int TurnCount { get; set; }
    public int CurrentPlayerNumber { get; set; }

    // Additional variant-specific data such as
    // GomokuPlus inventory and Fog perspective
    // can be added when those features are completed.
    public GameConfig ToGameConfig()
    {
        GameConfig config = new GameConfig();

        if (!Enum.TryParse(GameFamily, out GameFamily family))
        {
            throw new SaveFileDamagedException($"Invalid game family: {GameFamily}");
        }

        config.GameFamily = family;

        if (!Enum.TryParse(GameMode, out GameMode mode))
        {
            throw new SaveFileDamagedException($"Invalid game mode: {GameMode}");
        }

        config.GameMode = mode;

        if (family == global::GameFamily.Gomoku)
        {
            if (!Enum.TryParse(Variant, out GomokuVariant variant))
            {
                throw new SaveFileDamagedException($"Invalid Gomoku variant: {Variant}");
            }

            config.GomokuVariant = variant;
        }
        else
        {
            if (!Enum.TryParse(Variant, out ReversiVariant variant))
            {
                throw new SaveFileDamagedException(
                    $"Invalid Reversi variant: {Variant}");
            }

            config.ReversiVariant = variant;
        }

        if (mode == global::GameMode.HumanVsComputer)
        {
            if (!Enum.TryParse(ComputerType, out ComputerType computerType))
            {
                throw new SaveFileDamagedException(
                    $"Invalid computer type: {ComputerType}");
            }

            config.ComputerType = computerType;
        }

        return config;
    }

}


// =========================================================
// Save File Exception
// =========================================================

// Used when a save file exists but its data
// cannot be restored correctly.
public class SaveFileDamagedException : Exception
{
    public SaveFileDamagedException(string message)
        : base(message)
    {
    }
}


// =========================================================
// Game Store

// Responsibilities:
// SaveData -> JSON
// JSON -> SaveData
// Create save directory
// Check save existence
// List available save files
public class GameStore
{
    // All save files are stored inside this folder.
    private readonly string saveFolder = "Saves";
    private readonly CommandParser parser;
    public GameStore(CommandParser parser)
    {
        this.parser = parser;
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }

    public GameStore()
    {
        parser = new CommandParser();
        // Automatically create the save folder
        // when GameStore is created.
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }

    // Saves SaveData as a JSON file.
    public void Save(
        SaveData data,
        string fileName)
    {
        try
        {
            // Automatically add .json if the user
            // did not include the extension.
            if (!fileName.EndsWith( ".json", StringComparison.OrdinalIgnoreCase))
            {
                fileName += ".json";
            }

            string filePath = Path.Combine( saveFolder, fileName);

            JsonSerializerOptions options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };

            string json = JsonSerializer.Serialize( data, options );

            File.WriteAllText( filePath, json );
        }
        catch (Exception ex)
        {
            throw new IOException("Unable to save the game.",ex);
        }
    }

    // Reads a JSON save file and converts it back into SaveData.
    public SaveData Load(string fileName)
    {
        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".json";
        }

        string filePath = Path.Combine( saveFolder, fileName );
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Save file was not found: {fileName}");
        }

        try
        {
            string json = File.ReadAllText(filePath);

            SaveData? data = JsonSerializer.Deserialize<SaveData>(json);

            if (data == null)
            {
                throw new SaveFileDamagedException("Save file contains no valid game data.");
            }

            ValidateSaveData(data);
            return data;
        }
        catch (JsonException ex)
        {
            throw new SaveFileDamagedException($"Save file contains invalid JSON: {ex.Message}");
        }
    }

    // Checks whether a save file already exists.
    public bool SaveExists(string fileName)
    {
        if (!fileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            fileName += ".json";
        }

        string filePath = Path.Combine( saveFolder, fileName);

        return File.Exists(filePath);
    }

    // Get Save Files
    // Returns all available JSON save files.
    public string[] GetSaveFiles()
    {
        return Directory.GetFiles( saveFolder, "*.json");
    }

    // Used when the program needs to display where save files are stored.
    public string GetSaveFolder()
    {
        return saveFolder;
    }

    // Performs basic validation after loading.
    private void ValidateSaveData(SaveData data)
    {
        if (string.IsNullOrWhiteSpace(data.GameFamily))
        {
            throw new SaveFileDamagedException("Save file is missing the game family.");
        }

        if (string.IsNullOrWhiteSpace(data.Variant))
        {
            throw new SaveFileDamagedException("Save file is missing the game variant.");
        }

        if (string.IsNullOrWhiteSpace(data.GameMode))
        {
            throw new SaveFileDamagedException("Save file is missing the game mode.");
        }

        if (data.TurnCount < 1)
        {
            throw new SaveFileDamagedException("Save file contains an invalid turn count.");
        }

        if (data.CurrentPlayerNumber != 1 && data.CurrentPlayerNumber != 2)
        {
            throw new SaveFileDamagedException("Save file contains an invalid current player.");
        }

        if (data.MoveHistory == null)
        {
            throw new SaveFileDamagedException("Save file contains invalid move history.");
        }
    }
    // =========================================================
    // Validate Save Name
    // =========================================================
    // Checks whether a save name is safe to use.
    // Only letters, numbers, '-' and '_' are allowed.
    public bool IsValidSaveName(string saveName)
    {
        if (string.IsNullOrWhiteSpace(saveName))
        {
            return false;
        }
        foreach (char c in saveName)
        {
            if (!char.IsLetterOrDigit(c) &&
                c != '-' &&
                c != '_')
            {
                return false;
            }
        }
        return true;

    }

    // Collects the current game state and converts it into
    // SaveData that can be written to a JSON file.
    public SaveData CreateSaveData(
        Game game,
        GameConfig config)
    {
        SaveData data = new SaveData();

        // Game configuration

        data.GameFamily = config.GameFamily.ToString();

        if (config.GameFamily == GameFamily.Gomoku)
        {
            data.Variant = config.GomokuVariant?.ToString() ?? "";
        }
        else
        {
            data.Variant = config.ReversiVariant?.ToString() ?? "";
        }

        // Save HumanVsHuman or HumanVsComputer.
        data.GameMode = config.GameMode.ToString();

        // ComputerType is only relevant when the game is Human vs Computer.
        if (config.GameMode == GameMode.HumanVsComputer)
        {
            data.ComputerType = config.ComputerType?.ToString() ?? "";
        }
        else
        {
            data.ComputerType = "";
        }

        // Current game state
        data.TurnCount = game.turnCount;
        data.CurrentPlayerNumber =
            game.currentPlayer == game.p1 ? 1 : 2;
        // Command history
        foreach (MoveCommand command in game.GetMoveHistory())
        {
            data.MoveHistory.Add(command.ToCommandString());
        }
        return data;
        
    }

    // =========================================================
    // Restore Game From Save Data
    // =========================================================

    public void RestoreFromSave(Game game, SaveData data)
    {
        // Start from an empty command history.
        game.ClearHistory();

        // Replay each saved move from oldest to newest.
        Player replayPlayer = game.p1;

        foreach (string moveText in data.MoveHistory)
        {
            ParsedCommand parsed = parser.Parse(moveText);

            if (parsed.Type != CommandType.Move)
            {
                throw new SaveFileDamagedException($"Invalid move in save file: {moveText}");
            }

            char piece = parsed.PieceCode!.Value;
            int row = parsed.Row!.Value;
            int col = parsed.Column!.Value;

            // Check board boundaries.
            if (row < 1 || row > game.board.Size ||
                col < 1 || col > game.board.Size)
            {
                throw new SaveFileDamagedException($"Move is outside the board: {moveText}");
            }

            try
            {
                // PlayMove creates the correct MoveCommand
                // and adds it to CommandHistory.
                game.ReplayMove(
                    replayPlayer,
                    piece,
                    row,
                    col
                );
            }
            catch (Exception ex)
            {
                throw new SaveFileDamagedException($"Unable to replay move: {moveText}. {ex.Message}");
            }

            // Move to the next player.
            replayPlayer =
                replayPlayer == game.p1 ? game.p2 : game.p1;
        }

        game.SetTurnState(
            data.TurnCount,
            data.CurrentPlayerNumber

        );
    }

}