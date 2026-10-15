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
        int x = Count(game.board, Piece.ordinary[0]);
        int o = Count(game.board, Piece.ordinary[1]);

        WriteLine($"Active player: {(game.currentPlayer == game.p1 ? "Player 1" : "Player 2")}");
        
        if (game is GomokuPlus)
        {
            int heavyx = Count(game.board, Piece.gomokuHeavy[0]);
            int heavyo = Count(game.board, Piece.gomokuHeavy[1]);
            WriteLine($"Stones: Player 1 : "+
                    $"{Piece.ordinary[0]}={x}, {Piece.gomokuHeavy[0]}={heavyx}, " + 
                    $"Player 2 : "+
                    $"{Piece.ordinary[1]}={o}, {Piece.gomokuHeavy[1]}={heavyo}");
        }
        else
        {
            WriteLine($"Stones: Player 1 : " + $"{Piece.ordinary[0]}={x}, " + 
                    $"Player 2 : " + $"{Piece.ordinary[1]}={o}");
        }

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