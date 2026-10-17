public class MoveStrategyTests
{
    // ---------- Gomoku ----------

    [Fact]
    public void GomokuRandom_OneEmptyCell_ReturnsOrdinaryMoveOnThatCell()
    {
        Board board = CreateFullGomokuBoard();
        board.RemovePiece(4, 7);

        string move = new GomokuRandomStrategy().ChooseMove(board, 'O');

        Assert.Equal("O4:7", move);
    }

    [Fact]
    public void GomokuRandom_FullBoard_ThrowsInvalidOperationException()
    {
        Board board = CreateFullGomokuBoard();

        Assert.Throws<InvalidOperationException>(
            () => new GomokuRandomStrategy().ChooseMove(board, 'O'));
    }

    [Fact]
    public void GomokuSmart_OwnFourInARow_PlaysWinningMove()
    {
        Board board = new Board(10);
        board.PlayMove('O', 1, 1);
        board.PlayMove('O', 1, 2);
        board.PlayMove('O', 1, 3);
        board.PlayMove('O', 1, 4);

        string move = new GomokuSmartStrategy().ChooseMove(board, 'O');

        Assert.Equal("O1:5", move);
    }

    [Fact]
    public void GomokuSmart_OpponentFourInAColumn_BlocksIt()
    {
        Board board = new Board(10);
        board.PlayMove('X', 2, 5);
        board.PlayMove('X', 3, 5);
        board.PlayMove('X', 4, 5);
        board.PlayMove('X', 5, 5);

        string move = new GomokuSmartStrategy().ChooseMove(board, 'O');

        Assert.Contains(move, new[] { "O1:5", "O6:5" });
    }

    [Fact]
    public void GomokuSmart_BrokenFourOnDiagonal_BlocksTheGap()
    {
        Board board = new Board(10);
        board.PlayMove('X', 2, 2);
        board.PlayMove('X', 3, 3);
        board.PlayMove('X', 5, 5);
        board.PlayMove('X', 6, 6);

        string move = new GomokuSmartStrategy().ChooseMove(board, 'O');

        Assert.Equal("O4:4", move);
    }

    [Fact]
    public void GomokuSmart_CanWinAndBlock_PrefersWinning()
    {
        Board board = new Board(10);
        // Opponent four in row 9
        board.PlayMove('X', 9, 1);
        board.PlayMove('X', 9, 2);
        board.PlayMove('X', 9, 3);
        board.PlayMove('X', 9, 4);
        // Own four in row 2
        board.PlayMove('O', 2, 6);
        board.PlayMove('O', 2, 7);
        board.PlayMove('O', 2, 8);
        board.PlayMove('O', 2, 9);

        string move = new GomokuSmartStrategy().ChooseMove(board, 'O');

        Assert.Contains(move, new[] { "O2:5", "O2:10" });
    }

    [Fact]
    public void GomokuSmart_HeavyStoneInLine_CountsAsOwnStone()
    {
        Board board = new Board(10);
        board.PlayMove('O', 7, 1);
        board.PlayMove('#', 7, 2);
        board.PlayMove('O', 7, 4);
        board.PlayMove('#', 7, 5);

        string move = new GomokuSmartStrategy().ChooseMove(board, 'O');

        Assert.Equal("O7:3", move);
    }

    [Fact]
    public void GomokuSmart_ChooseMove_DoesNotChangeBoard()
    {
        Board board = new Board(10);
        board.PlayMove('X', 5, 5);
        board.PlayMove('O', 6, 6);
        string before = Snapshot(board);

        new GomokuSmartStrategy().ChooseMove(board, 'O');

        Assert.Equal(before, Snapshot(board));
    }

    [Fact]
    public void GomokuPlusRandom_SpecialStonesLeft_OffersOrdinaryHeavyAndEraserMoves()
    {
        // Only valid moves for O: O3:3, H3:3 (empty cell) and E7:7 (opponent's ordinary stone).
        // The opponent's Heavy stone at 8:8 must never be offered as an Eraser target.
        Board board = CreateFullGomokuBoard('O');
        board.RemovePiece(3, 3);
        board.PlayMove('X', 7, 7);
        board.PlayMove('@', 8, 8);
        GomokuPlusRandomStrategy strategy = new GomokuPlusRandomStrategy(new StoneInventory());

        List<string> moves = new List<string>();
        for (int i = 0; i < 200; i++)
        {
            moves.Add(strategy.ChooseMove(board, 'O'));
        }

        Assert.Contains("O3:3", moves);
        Assert.Contains("H3:3", moves);
        Assert.Contains("E7:7", moves);
        Assert.DoesNotContain("E8:8", moves);
    }

    [Fact]
    public void GomokuPlusRandom_NoSpecialStonesLeft_OnlyOffersOrdinaryMove()
    {
        Board board = CreateFullGomokuBoard('O');
        board.RemovePiece(3, 3);
        board.PlayMove('X', 7, 7);
        StoneInventory inventory = new StoneInventory();
        inventory.UseHeavy(1);
        inventory.UseHeavy(1);
        inventory.UseEraser(1);
        inventory.UseEraser(1);

        string move = new GomokuPlusRandomStrategy(inventory).ChooseMove(board, 'O');

        Assert.Equal("O3:3", move);
    }

    // ---------- Reversi ----------

    [Fact]
    public void ReversiRandom_InitialBoard_ReturnsOneOfFourLegalMoves()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        string move = new ReversiRandomStrategy().ChooseMove(board, 'X');

        Assert.Contains(move, new[] { "P3:4", "P4:3", "P5:6", "P6:5" });
    }

    [Fact]
    public void ReversiRandom_NoLegalMove_ReturnsPass()
    {
        Board board = new Board(8);
        board.PlayMove('X', 1, 1);

        string move = new ReversiRandomStrategy().ChooseMove(board, 'X');

        Assert.Equal("PASS", move);
    }

    [Fact]
    public void ReversiGreedy_TwoOptions_PicksMoveWithMostFlips()
    {
        Board board = new Board(8);
        // P4:4 flips two (4:5, 4:6); P6:1 flips one (6:2)
        board.PlayMove('O', 4, 5);
        board.PlayMove('O', 4, 6);
        board.PlayMove('X', 4, 7);
        board.PlayMove('O', 6, 2);
        board.PlayMove('X', 6, 3);

        string move = new ReversiGreedyStrategy().ChooseMove(board, 'X');

        Assert.Equal("P4:4", move);
    }

    [Fact]
    public void CornerGreedy_CornerAvailable_PrefersCornerOverMoreFlips()
    {
        Board board = new Board(8);
        // P1:1 is a corner and flips one (1:2); P4:4 flips two (4:5, 4:6)
        board.PlayMove('O', 1, 2);
        board.PlayMove('X', 1, 3);
        board.PlayMove('O', 4, 5);
        board.PlayMove('O', 4, 6);
        board.PlayMove('X', 4, 7);

        string move = new CornerGreedyStrategy().ChooseMove(board, 'X');

        Assert.Equal("P1:1", move);
    }

    [Fact]
    public void CornerGreedy_OpponentThreatensThirdCorner_BlocksIt()
    {
        // Lin's case: P4:4 flips two, but leaves O free to take corner 8:1 (its third)
        Board board = new Board(8);
        board.PlayMove('O', 1, 1);
        board.PlayMove('O', 1, 8);
        board.PlayMove('X', 8, 2);
        board.PlayMove('O', 8, 3);
        board.PlayMove('O', 4, 5);
        board.PlayMove('O', 4, 6);
        board.PlayMove('X', 4, 7);

        string move = new CornerGreedyStrategy().ChooseMove(board, 'X');

        Assert.Equal("P8:4", move);
    }

    [Fact]
    public void AntiReversi_TwoOptions_PicksMoveWithFewestFlips()
    {
        Board board = new Board(8);
        board.PlayMove('O', 4, 5);
        board.PlayMove('O', 4, 6);
        board.PlayMove('X', 4, 7);
        board.PlayMove('O', 6, 2);
        board.PlayMove('X', 6, 3);

        string move = new AntiReversiStrategy().ChooseMove(board, 'X');

        Assert.Equal("P6:1", move);
    }

    [Fact]
    public void AntiReversi_CornerAvailable_AvoidsCorner()
    {
        Board board = new Board(8);
        // P1:1 corner flips one; P4:4 flips two, but Anti avoids corners
        board.PlayMove('O', 1, 2);
        board.PlayMove('X', 1, 3);
        board.PlayMove('O', 4, 5);
        board.PlayMove('O', 4, 6);
        board.PlayMove('X', 4, 7);

        string move = new AntiReversiStrategy().ChooseMove(board, 'X');

        Assert.Equal("P4:4", move);
    }

    // ---------- Helpers ----------

    private Board CreateFullGomokuBoard()
    {
        return CreateFullGomokuBoard('X');
    }

    private Board CreateFullGomokuBoard(char symbol)
    {
        Board board = new Board(10);
        for (int row = 1; row <= 10; row++)
        {
            for (int col = 1; col <= 10; col++)
            {
                board.PlayMove(symbol, row, col);
            }
        }
        return board;
    }

    private string Snapshot(Board board)
    {
        string cells = "";
        for (int row = 1; row <= board.Size; row++)
        {
            for (int col = 1; col <= board.Size; col++)
            {
                cells += board.GetCellInfo(row, col);
            }
        }
        return cells;
    }
}