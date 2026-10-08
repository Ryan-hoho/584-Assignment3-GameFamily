using static System.Console;

public class GameSelection
{
    public static Game SelectGame()
    {
        int choice = GetGameChoice();

        // Load an existing saved game.
        if (choice == 3)
        {
            return LoadGame();
        }

        // Create configuration for a new game.
        GameConfig config = new GameConfig();
        config.GameFamily = (GameFamily)(choice - 1);
        if (config.GameFamily == GameFamily.Gomoku)
        {
            config.GomokuVariant = GetGomokuVariant();
        }
        else if (config.GameFamily == GameFamily.Reversi)
        {
            config.ReversiVariant = GetReversiVariant();
        }

        config.GameMode = GetGameMode();
        if (config.GameMode == GameMode.HumanVsComputer)
        {
            config.ComputerType = GetComputerType();
        }

        // Factory creates the selected new game.
        return GameFactory.CreateGame(config);
    }
        
        static int PromptForInput(string message, int optionCount)
        {
            // Prompt for user input until valid numerical input
            while (true) 
            {
                Write(message);
                string? str = ReadLine();
                if (int.TryParse(str, out int input) && input >= 1 && input <= optionCount)
                {
                    return input;
                }
                WriteLine($"Invalid input. Please enter a number from 1 to {optionCount}.");
            }
        }   

        static int GetGameChoice()
        {
            WriteLine("-----------------------");
            WriteLine("1. New Gomoku Game");
            WriteLine("2. New Reversi Game");
            WriteLine("3. Load Game");

            return PromptForInput("Select Game: ", 3);
        }

    static GomokuVariant GetGomokuVariant()
    {
        WriteLine("-----------------------");
        WriteLine("1. Gomoku Standard");
        WriteLine("2. Gomoku Plus");
        WriteLine("3. Gomoku Fog");

        int input = PromptForInput("Select Gomoku variant: ", 3);
        return (GomokuVariant)(input-1);
    }

    static ReversiVariant GetReversiVariant()
    {
        WriteLine("-----------------------");
        WriteLine("1. Standard Reversi");
        WriteLine("2. Anti Reversi");
        WriteLine("3. Corner Reversi");

        int input = PromptForInput("Select Reversi variant: ", 3);
        return (ReversiVariant)(input-1);
    }

    static GameMode GetGameMode()
    {
        WriteLine("-----------------------");
        WriteLine("1. Human vs. Human");
        WriteLine("2. Human vs. Computer");

        int input = PromptForInput("Select Mode: ", 2);
        return (GameMode)(input-1);
    }

    static ComputerType GetComputerType()
    {
        WriteLine("-----------------------");
        WriteLine("1. Dumb AI");
        WriteLine("2. Smart AI");

        int input = PromptForInput("Select Computer Level: ", 2);
        return (ComputerType)(input-1);
    }
    public static Game LoadGame()
    {
        GameStore gameStore = new GameStore();

        string[] saveFiles = gameStore.GetSaveFiles();

        if (saveFiles.Length == 0)
        {
            throw new FileNotFoundException(
                 "No saved games were found.");
        }

        WriteLine("-----------------------");
        WriteLine("Saved Games");

        for (int i = 0; i < saveFiles.Length; i++)
        {
            WriteLine($"{i + 1}. {Path.GetFileNameWithoutExtension(saveFiles[i])}");
        }

        int choice = PromptForInput(
            "Select saved game: ",
            saveFiles.Length
        );
        string saveName =
            Path.GetFileNameWithoutExtension(
                saveFiles[choice - 1]
            );

        // Read JSON.
        SaveData data = gameStore.Load(saveName);

        // Restore the saved configuration.
        GameConfig config = data.ToGameConfig();

        // Factory creates the correct game type.
        Game game = GameFactory.CreateGame(config);

        // Replay saved move history.
        gameStore.RestoreFromSave(game, data);

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