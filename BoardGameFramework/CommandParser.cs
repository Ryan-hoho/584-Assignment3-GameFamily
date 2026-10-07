// 定義玩家輸入的 command 類型。
// Move 是一般下棋動作；其他則是遊戲控制指令。
public enum CommandType
{
    Move,Undo,Redo,Save,Load,Help,Quit
}


// CommandParser 解析完成後，
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

    // Utility command 使用。
    // 例如 undo / redo / help。
    public ParsedCommand(CommandType type)
    {
        Type = type;
        PieceCode = null;
        Row = null;
        Column = null;
    }

    // Move command 使用。
    // 例如 O5:3 或 P3:4。
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


// 玩家輸入格式錯誤時使用。
// 例如：
// O5
// Oabc:3
// P3:
// 空白輸入
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

        // 移除前後空格。
        // 例如 "  undo  " -> "undo"
        string command = input.Trim();

        // -------------------------------------------------
        // 2. Utility Commands
        // -------------------------------------------------
        // Utility command 必須先判斷。
        // 否則 "undo" 會被當成 Move，然後程式會嘗試解析 "ndo" 為座標
        // 使用 ToLower() 統一轉成小寫，
        // 所以 UNDO / Undo / undo 都會被視為同一個 command。

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

        }


        // -------------------------------------------------
        // 3. Move Command
        // -------------------------------------------------
        // 目前新版的格式：
        // Gomoku:
        // O5:3
        // Gomoku Plus:
        // H5:3
        // E5:3
        // Reversi:
        // P5:3

        // Move 至少要包含：
        // Piece + Row + : + Column
        // 最短例如 O1:1。
        if (command.Length < 4)
        {
            throw new InvalidCommandFormatException(
                $"Invalid command format: '{input}'. Example: O3:2"
            );
        }


        // 第一個字元代表棋子 / action。
        char pieceCode =
            char.ToUpper(command[0]);


        // 移除第一個字元。
        // O5:3
        //  ↓
        // 5:3
        string position =
            command.Substring(1);


        // 使用 ":" 分開 Row / Column。
        string[] coordinates =
            position.Split(':');

        if (coordinates.Length != 2)
        {
            throw new InvalidCommandFormatException(
                $"Invalid command format: '{input}'. Example: O3:2"
            );
        }

        // Row 必須是整數。
        if (!int.TryParse(
                coordinates[0],
                out int row))
        {
            throw new InvalidCommandFormatException(
                "Row must be a number."
            );
        }

        // Column 必須是整數。
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
        // 它不判斷：
        // O 是否適合 Reversi
        // P 是否適合 Gomoku
        // Cell 是否 occupied
        // Reversi 是否能形成 flank
        //
        // 這些都是 Game Rules 的責任。
        return new ParsedCommand(
            CommandType.Move,
            pieceCode,
            row,
            column
        );
    }
}