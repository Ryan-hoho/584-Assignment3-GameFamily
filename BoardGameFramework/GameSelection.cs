using static System.Console;

public class GameSelection
{
    public static GameConfig Select()
    {
        int choice = GetGameChoice();
        if (choice == 3)
        {
            // TO-DO: Load saved game
            // Temporary stops until loading is implemented
            throw new NotImplementedException("Loading is not yet implemented.");
        }
        
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
        return config;
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
}