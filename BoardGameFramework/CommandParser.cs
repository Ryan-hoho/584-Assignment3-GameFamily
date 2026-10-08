// Defines the types of commands that a player can enter.
// Move represents a normal gameplay action, while the others are game control commands.
public enum CommandType
{
    Move,Undo,Redo,Save,Load,Help,Quit,Pass
}


// 使用 ParsedCommand 將結果交給 Game。
public class ParsedCommand
{
    public CommandType Type { get; }

    // PieceCode 
    // Gomoku:
    // O = Ordinary
    // H = Heavy
    // E = Eraser
    //
    // Reversi:
    // P = Place Disk
    public char? PieceCode { get; }
    public int? Row { get; }
    public int? Column { get; }

    // Utility command 

    public ParsedCommand(CommandType type)
    {
        Type = type;
        PieceCode = null;
        Row = null;
        Column = null;
    }

    // Move command 
    public ParsedCommand(
        CommandType type,
        char pieceCode,
        int row,
        int column)
    {
        Type = type;
        PieceCode = pieceCode;
        Row = row;
        Column = column;
    }
}


public class InvalidCommandFormatException : Exception
{
    public InvalidCommandFormatException(string message)
        : base(message)
    {
    }
}

public class CommandParser
{
    public ParsedCommand Parse(string input)
    {
        // -------------------------------------------------
        // 1. Empty Input
        // -------------------------------------------------
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new InvalidCommandFormatException(
                "Command cannot be empty."
            );
        }


        string command = input.Trim();

        // -------------------------------------------------
        // 2. Utility Commands
        // -------------------------------------------------

        switch (command.ToLower())
        {
            case "undo":
                return new ParsedCommand(CommandType.Undo);

            case "redo":
                return new ParsedCommand(CommandType.Redo);

            case "save":
                return new ParsedCommand(CommandType.Save);

            case "load":
                return new ParsedCommand(CommandType.Load);

            case "help":
                return new ParsedCommand(CommandType.Help);

            case "quit":
                return new ParsedCommand(CommandType.Quit);
            case "pass":
                return new ParsedCommand(CommandType.Pass);

        }


        // -------------------------------------------------
        // 3. Move Command
        // -------------------------------------------------
        if (command.Length < 4)
        {
            throw new InvalidCommandFormatException(
                $"Invalid command format: '{input}'. Example: O3:2"
            );
        }


        char pieceCode =
            char.ToUpper(command[0]);


        string position =
            command.Substring(1);

        string[] coordinates =
            position.Split(':');

        if (coordinates.Length != 2)
        {
            throw new InvalidCommandFormatException(
                $"Invalid command format: '{input}'. Example: O3:2"
            );
        }

        if (!int.TryParse(
                coordinates[0],
                out int row))
        {
            throw new InvalidCommandFormatException(
                "Row must be a number."
            );
        }

        if (!int.TryParse(
                coordinates[1],
                out int column))
        {
            throw new InvalidCommandFormatException(
                "Column must be a number."
            );
        }


        // Parser 到這裡只負責：
        // 「這個 command 格式是不是正確？」
        // 這些都是 Game Rules 的責任。
        return new ParsedCommand(
            CommandType.Move,
            pieceCode,
            row,
            column
        );
    }
}