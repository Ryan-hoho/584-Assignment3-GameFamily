using static System.Console;

public class TestRunner
{
    private readonly CommandParser parser = new CommandParser();

    public void Run(Game game, string script)
    {
        bool gameOver = false;

        foreach (string raw in script.Split(','))
        {
            string text = raw.Trim();
            try
            {
                ParsedCommand cmd = parser.Parse(text);
                if (cmd.Type != CommandType.Move)
                {
                    WriteLine($"Skipped (not a move): {text}");
                    continue;
                }

                int row = cmd.Row!.Value, col = cmd.Column!.Value;
                if (!game.TryApplyMove(cmd.PieceCode!.Value, row, col))
                {
                    WriteLine($"Illegal move skipped: {text}");
                    continue;
                }

                if (game.CheckForWinner(row, col)) { gameOver = true; break; }
                game.AdvanceTurn();
            }
            catch (InvalidCommandFormatException ex)
            {
                WriteLine($"Invalid command '{text}': {ex.Message}");
            }
        }

        game.RenderBoard();
        Report(game, gameOver);
    }

    private void Report(Game game, bool gameOver)
    {
        int x = Count(game.board, 'X'), o = Count(game.board, 'O');
        WriteLine($"Active player: {(game.currentPlayer == game.p1 ? "Player 1" : "Player 2")}");
        WriteLine($"Stones: X={x}, O={o}");
        WriteLine(gameOver
            ? $"Winner: {(game.currentPlayer == game.p1 ? "Player 1" : "Player 2")}"
            : "Status: in progress");
    }

    private int Count(Board b, char s)
    {
        int n = 0;
        for (int r = 1; r <= b.Size; r++)
            for (int c = 1; c <= b.Size; c++)
                if (b.GetCellInfo(r, c) == s) n++;
        return n;
    }
}