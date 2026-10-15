public class ReversiFlowAndVictoryTests
{
    [Fact]
    public void HasLegalMove_WhenOnlyOCanMove_XMustPass()
    {
        Board board = new Board(8);

        // Fill board with X.
        for (int row = 1; row <= 8; row++)
        {
            for (int col = 1; col <= 8; col++)
            {
                board.PlayMove('X', row, col);
            }
        }

        // Create:
        // O X .
        board.PlayMove('O', 1, 1);
        board.PlayMove(' ', 1, 3);

        Assert.False(ReversiRules.HasLegalMove(board, 'X'));
        Assert.True(ReversiRules.HasLegalMove(board, 'O'));
        Assert.False(ReversiRules.NoMovesForEitherPlayer(board));
    }

    [Fact]
    public void NoMovesForEitherPlayer_FullBoard_ReturnsTrue()
    {
        Board board = new Board(8);

        for (int row = 1; row <= 8; row++)
        {
            for (int col = 1; col <= 8; col++)
            {
                board.PlayMove('X', row, col);
            }
        }

        Assert.True(ReversiRules.NoMovesForEitherPlayer(board));
    }

    [Fact]
    public void DiskMajority_XHasMoreDisks_ReturnsXWins()
    {
        Board board = CreateSixtyXFourOBoard();

        IReversiWinStrategy strategy = new DiskMajority();

        ReversiResult result = strategy.Evaluate(board);

        Assert.Equal(ReversiResult.XWins, result);
    }

    [Fact]
    public void DiskMinority_OHasFewerDisks_ReturnsOWins()
    {
        Board board = CreateSixtyXFourOBoard();

        IReversiWinStrategy strategy = new DiskMinority();

        ReversiResult result = strategy.Evaluate(board);

        Assert.Equal(ReversiResult.OWins, result);
    }

    [Fact]
    public void CornerDominance_XOwnsThreeCorners_ReturnsXWinsImmediately()
    {
        Board board = new Board(8);

        // X owns three corners.
        board.PlayMove('X', 1, 1);
        board.PlayMove('X', 1, 8);
        board.PlayMove('X', 8, 1);

        // Give O many more disks overall.
        for (int row = 2; row <= 7; row++)
        {
            for (int col = 2; col <= 7; col++)
            {
                board.PlayMove('O', row, col);
            }
        }

        IReversiWinStrategy strategy = new CornerDominance();

        ReversiResult result = strategy.Evaluate(board);

        Assert.Equal(ReversiResult.XWins, result);
    }

    [Fact]
    public void CornerDominance_NoThreeCorners_UsesMajorityFallback()
    {
        Board board = CreateSixtyXFourOBoard();

        // O owns two corners, X owns the other two.
        // Nobody has three corners.
        IReversiWinStrategy strategy = new CornerDominance();

        ReversiResult result = strategy.Evaluate(board);

        Assert.Equal(ReversiResult.XWins, result);
    }

    [Fact]
    public void CheckForWinner_StandardReversi_UsesDiskMajority()
    {
        Board board = CreateSixtyXFourOBoard();

        GameConfig config = new GameConfig
        {
            GameFamily = GameFamily.Reversi,
            ReversiVariant = ReversiVariant.Standard,
            GameMode = GameMode.HumanVsHuman
        };

        Player p1 = new HumanPlayer();
        Player p2 = new HumanPlayer();

        ReversiGame game =
            new ReversiGame(config, board, p1, p2);

        GameResult result = game.CheckForWinner(0, 0);

        Assert.Equal(GameResult.Player1Wins, result);
    }

    [Fact]
    public void CheckForWinner_AntiReversi_UsesDiskMinority()
    {
        Board board = CreateSixtyXFourOBoard();

        GameConfig config = new GameConfig
        {
            GameFamily = GameFamily.Reversi,
            ReversiVariant = ReversiVariant.Anti,
            GameMode = GameMode.HumanVsHuman
        };

        Player p1 = new HumanPlayer();
        Player p2 = new HumanPlayer();

        AntiReversi game =
            new AntiReversi(config, board, p1, p2);

        GameResult result = game.CheckForWinner(0, 0);

        Assert.Equal(GameResult.Player2Wins, result);
    }

    [Fact]
    public void CheckForWinner_CornerReversi_UsesCornerDominance()
    {
        Board board = new Board(8);

        board.PlayMove('X', 1, 1);
        board.PlayMove('X', 1, 8);
        board.PlayMove('X', 8, 1);

        GameConfig config = new GameConfig
        {
            GameFamily = GameFamily.Reversi,
            ReversiVariant = ReversiVariant.Corner,
            GameMode = GameMode.HumanVsHuman
        };

        Player p1 = new HumanPlayer();
        Player p2 = new HumanPlayer();

        CornerReversi game =
            new CornerReversi(config, board, p1, p2);

        GameResult result = game.CheckForWinner(0, 0);

        Assert.Equal(GameResult.Player1Wins, result);
    }

    private Board CreateSixtyXFourOBoard()
    {
        Board board = new Board(8);

        for (int row = 1; row <= 8; row++)
        {
            for (int col = 1; col <= 8; col++)
            {
                board.PlayMove('X', row, col);
            }
        }

        // X = 60, O = 4
        // Two O disks are corners so neither player owns 3 corners.
        board.PlayMove('O', 1, 1);
        board.PlayMove('O', 1, 8);
        board.PlayMove('O', 1, 2);
        board.PlayMove('O', 1, 3);

        return board;
    }
}