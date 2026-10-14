using static System.Console;

public class TestRunner
{
    private readonly CommandParser parser = new CommandParser();

    public void Run(Game game, string script)
    {
        // bool gameOver = false;
        GameResult result = GameResult.InProgress;

        foreach (string raw in script.Split(','))
        {
            string text = raw.Trim();
            try
            {
                ParsedCommand cmd = parser.Parse(text);
                int row =0, col =0;
                bool applied;

                if (cmd.Type == CommandType.Pass)
                {
                    applied = game.TryPass();
                }

                else if (cmd.Type == CommandType.Move)
                {
                    row = cmd.Row!.Value;
                    col = cmd.Column!.Value;
                    applied = game.TryApplyMove(cmd.PieceCode!.Value, row, col);
                }
                else
                {
                    WriteLine($"Skipped (not a move): {text}");
                    continue;   
                }
                if (!applied)
                {
                    WriteLine($"Illegal move skipped: {text}");
                    continue;
                }

                result = game.CheckForWinner(row, col);
                if (result != GameResult.InProgress) { break; }
                game.AdvanceTurn();
            }
            catch (InvalidCommandFormatException ex)
            {
                WriteLine($"Invalid command '{text}': {ex.Message}");
            }
        }

        game.RenderBoard();
        Report(game, result);
    }

    private void Report(Game game, GameResult result)
    {
        int x = Count(game.board, 'X'), o = Count(game.board, 'O');
        WriteLine($"Active player: {(game.currentPlayer == game.p1 ? "Player 1" : "Player 2")}");
        WriteLine($"Stones: X={x}, O={o}");
        switch (result)
        {
            case GameResult.Player1Wins:
                WriteLine("Winner: Player 1");
                break;
            case GameResult.Player2Wins:
                WriteLine("Winner: Player 2");
                break;
            case GameResult.Draw:
                WriteLine("Result: Draw");
                break;
            default:
                WriteLine("Status: in progress");
                break;
        }
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