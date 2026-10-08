using static System.Console;

public class GameSelection
{
    public static Game? SelectGame()
    {
        while(true){
            int choice = GetGameChoice();
            
            if (choice == 0) return null;  // Exit
            // Load an existing saved game.
            if (choice == 3)
            {
                Game? loaded = LoadGame();
                if (loaded == null) continue;  // LoadGame 內按 0 回主選單
                return loaded;
            }

            GameFamily selectedFamily =(GameFamily)(choice - 1);

            // =========================================
            // Variant Menu
            // =========================================
            while (true)
            {
                int variant;
                if (selectedFamily == GameFamily.Gomoku)
                {
                    variant = GetGomokuVariant();
                }
                else
                {
                    variant = GetReversiVariant();
                }
                // Back to Main Menu
                if (variant == 0)
                {
                    break;
                }
                // =========================================
                // Mode Menu
                // =========================================
                while (true)
                {
                    int mode = GetGameMode();
                    // Back to Variant Menu
                    if (mode == 0)
                    {
                        break;
                    }
                    // Human vs Human
                    if ((GameMode)(mode - 1) == GameMode.HumanVsHuman)
                    {
                        GameConfig config = new GameConfig();
                        config.GameFamily = selectedFamily;
                        config.GameMode = GameMode.HumanVsHuman;
                        if (selectedFamily == GameFamily.Gomoku)
                        {
                            config.GomokuVariant =(GomokuVariant)(variant - 1);
                        }
                        else
                        {
                            config.ReversiVariant =(ReversiVariant)(variant - 1);
                        }
                        return GameFactory.CreateGame(config);
                    }

                    // =========================================
                    // Computer Type Menu
                    // =========================================
                    while (true)
                    {
                        int ai = GetComputerType();
                        // Back to Mode Menu
                        if (ai == 0){ break;}

                        GameConfig config = new GameConfig();
                        config.GameFamily = selectedFamily;
                        config.GameMode = GameMode.HumanVsComputer;
                        config.ComputerType =(ComputerType)(ai - 1);

                        if (selectedFamily == GameFamily.Gomoku)
                        {
                            config.GomokuVariant =(GomokuVariant)(variant - 1);
                        }
                        else
                        {
                            config.ReversiVariant =(ReversiVariant)(variant - 1);
                        }
                        return GameFactory.CreateGame(config);
                    }
                }
            }
        }
    }
        
        static int PromptForInput(string message, int optionCount)
        {
            // Prompt for user input until valid numerical input
            while (true) 
            {
                Write(message);
                string? str = ReadLine();
                if (int.TryParse(str, out int input) && input >= 0 && input <= optionCount)
                {
                    return input;
                }
                WriteLine($"Invalid input. Please enter a number from 0 to {optionCount}.");
            }
        }   

        static int GetGameChoice()
        {
            WriteLine("========== MAIN MENU ==========");
            WriteLine("1. New Gomoku Game");
            WriteLine("2. New Reversi Game");
            WriteLine("3. Load Game");
            WriteLine("0. Exit");
            WriteLine("===============================");
            WriteLine();


            return PromptForInput("Select Game: ", 3);
        }

    static int GetGomokuVariant()
    {
        WriteLine("------- GOMOKU VARIANT --------");
        WriteLine("1. Gomoku Standard");
        WriteLine("2. Gomoku Plus");
        WriteLine("3. Gomoku Fog");
        WriteLine("0. Back");
        WriteLine("-------------------------------");
        WriteLine();

        return PromptForInput("Select Gomoku variant: ", 3);
        
    }

    static int GetReversiVariant()
    {
        WriteLine("------- REVERSI VARIANT -------");
        WriteLine("1. Standard Reversi");
        WriteLine("2. Anti Reversi");
        WriteLine("3. Corner Reversi");
        WriteLine("0. Back");
        WriteLine("-------------------------------");
        WriteLine();

        return PromptForInput("Select Reversi variant: ", 3);   
    }

    static int GetGameMode()
    {
        WriteLine("---------- GAME MODE ----------");
        WriteLine("1. Human vs. Human");
        WriteLine("2. Human vs. Computer");
        WriteLine("0. Back");
        WriteLine("-------------------------------");
        WriteLine();

        return PromptForInput("Select Mode: ", 2);
    }

    static int GetComputerType()
    {
        WriteLine("--------- AI DIFFICULTY --------");
        WriteLine("1. Dumb AI");
        WriteLine("2. Smart AI");
        WriteLine("0. Back");
        WriteLine("-------------------------------");
        WriteLine();

        return PromptForInput("Select Computer Level: ", 2);
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

        int choice = PromptForInput("Select saved game: ",saveFiles.Length);
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