using static System.Console;

public class GameSelection
{
    public static Game? SelectGame()
    {
        int choice;
        
        while(true)
        {
            // Prompt to select a Game Family or Load saved game
            choice = GetGameChoice();

            // If choose to load saved game
            if (choice == 3)
            {
                Game? loaded = LoadGame();
                if (loaded == null) continue;  // LoadGame 內按 0 回主選單
                return loaded;

                // TBD: Do we want the program to reprompt Menu Selection when there is no game to load? 
            }
            break;
        }

        // If select a Game Family
        GameConfig config = new GameConfig();
        config.GameFamily = (GameFamily)(choice - 1);

        // Prompt to select a Game Variant
        if (config.GameFamily == GameFamily.Gomoku)
        {
            config.GomokuVariant = GetGomokuVariant();
        }
        else
        {
            config.ReversiVariant = GetReversiVariant();
        }

        // Prompt to select a Game Mode and Computer Type if choosing to verse a Computer
        config.GameMode = GetGameMode(); 
        if (config.GameMode == GameMode.HumanVsComputer)
        {
            config.ComputerType = GetComputerType();
        }

        return GameFactory.CreateGame(config);
    }
        
    static int PromptForInput(string message, int optionCount, int minOption = 1)
    {
        // Prompt for user input until valid numerical input
        while (true) 
        {
            Write(message);
            string? str = ReadLine();

            if (int.TryParse(str, out int input) && input >= minOption && input <= optionCount)
            {
                return input;
            }

            WriteLine($"Invalid input. Please enter a number from {minOption} to {optionCount}.");
        }
    }   

    static int GetGameChoice()
    {
        WriteLine("========== MAIN MENU ==========");
        WriteLine("1. New Gomoku Game");
        WriteLine("2. New Reversi Game");
        WriteLine("3. Load Game");
        WriteLine("===============================");
        WriteLine();

        return PromptForInput("Select Game: ", 3);
    }

    static GomokuVariant GetGomokuVariant()
    {
        WriteLine("------- GOMOKU VARIANT --------");
        WriteLine("1. Gomoku Standard");
        WriteLine("2. Gomoku Plus");
        WriteLine("3. Gomoku Fog");
        WriteLine("-------------------------------");
        WriteLine();

        int input = PromptForInput("Select Gomoku variant: ", 3);
        return (GomokuVariant)(input - 1);
    }

    static ReversiVariant GetReversiVariant()
    {
        WriteLine("------- REVERSI VARIANT -------");
        WriteLine("1. Standard Reversi");
        WriteLine("2. Anti Reversi");
        WriteLine("3. Corner Reversi");
        WriteLine("-------------------------------");
        WriteLine();

        int input = PromptForInput("Select Reversi variant: ", 3);
        return (ReversiVariant)(input - 1);   
    }

    static GameMode GetGameMode()
    {
        WriteLine("---------- GAME MODE ----------");
        WriteLine("1. Human vs. Human");
        WriteLine("2. Human vs. Computer");
        WriteLine("-------------------------------");
        WriteLine();

        int input = PromptForInput("Select Mode: ", 2);
        return (GameMode)(input - 1);
    }

    static ComputerType GetComputerType()
    {
        WriteLine("--------- AI DIFFICULTY --------");
        WriteLine("1. Dumb AI");
        WriteLine("2. Smart AI");
        WriteLine("-------------------------------");
        WriteLine();

        int input = PromptForInput("Select Computer Level: ", 2);
        return (ComputerType)(input - 1);
    }

    public static Game? LoadGame()
    {
        GameStore gameStore = new GameStore();

        string[] saveFiles = gameStore.GetSaveFiles();

        if (saveFiles.Length == 0)
        {
            throw new FileNotFoundException("No saved games were found.");
        }

        WriteLine("-----------------------");
        WriteLine("========== SAVED GAMES ==========");

        for (int i = 0; i < saveFiles.Length; i++)
        {
            WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(saveFiles[i])}");
        }
        WriteLine("0. Back");
        WriteLine("=================================");
        WriteLine();

        int choice = PromptForInput("Select saved game: ",saveFiles.Length, minOption: 0);
        if(choice == 0)
        {
            return null;
        }
        
        string saveName =Path.GetFileNameWithoutExtension(saveFiles[choice - 1]);

        // Read JSON.
        SaveData data = gameStore.Load(saveName);

        // Restore the saved configuration.
        GameConfig config = data.ToGameConfig();

        // Factory creates the correct game type.
        Game game = GameFactory.CreateGame(config);

        // Replay saved move history.
        gameStore.RestoreFromSave(game, data);
        // Display loaded game information.

        WriteLine();
        WriteLine("------- LOADED GAME -------");
        WriteLine($"Game: {data.GameFamily}");
        WriteLine($"Variant: {data.Variant}");
        WriteLine($"Mode: {data.GameMode}");
        WriteLine($"Turn: {data.TurnCount}");
        WriteLine($"Current Player: Player {data.CurrentPlayerNumber}");
        WriteLine("---------------------------");
        WriteLine();

        return game;
    }

    public static GameConfig ParseCliArgs(string[] args, out string script)
    {
        GameConfig config = new GameConfig();
        script = "";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--game")
            {
                config.GameFamily = args[++i].ToLower() == "gomoku"
                    ? GameFamily.Gomoku : GameFamily.Reversi;
            }
            else if (args[i] == "--variant")
            {
                string v = args[++i].ToLower();
                if (config.GameFamily == GameFamily.Gomoku)
                    config.GomokuVariant = v == "plus" ? GomokuVariant.Plus
                        : v == "fog" ? GomokuVariant.Fog : GomokuVariant.Standard;
                else
                    config.ReversiVariant = v == "anti" ? ReversiVariant.Anti
                        : v == "corner" ? ReversiVariant.Corner : ReversiVariant.Standard;
            }
            else
            {
                script = args[i];
            }
        }

        config.GameMode = GameMode.HumanVsHuman;
        return config;
    }

}