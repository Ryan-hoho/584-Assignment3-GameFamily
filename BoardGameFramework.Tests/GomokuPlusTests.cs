public class GomokuPlusTests
{
    // ---------- StoneInventory ----------

    [Fact]
    public void Inventory_NewGame_EachPlayerHasTwoHeavyAndTwoEraser()
    {
        StoneInventory inventory = new StoneInventory();

        Assert.Equal(2, inventory.HeavyLeft(0));
        Assert.Equal(2, inventory.EraserLeft(0));
        Assert.Equal(2, inventory.HeavyLeft(1));
        Assert.Equal(2, inventory.EraserLeft(1));
    }

    [Fact]
    public void Inventory_UseAllHeavyStones_ThirdUseThrows()
    {
        StoneInventory inventory = new StoneInventory();
        inventory.UseHeavy(0);
        inventory.UseHeavy(0);

        Assert.Equal(0, inventory.HeavyLeft(0));
        Assert.Equal(2, inventory.HeavyLeft(1));
        Assert.Throws<InvalidOperationException>(() => inventory.UseHeavy(0));
    }

    // ---------- Commands ----------

    [Fact]
    public void HeavyCommand_ExecuteThenUndo_RestoresBoardAndInventory()
    {
        Board board = new Board(10);
        StoneInventory inventory = new StoneInventory();
        HeavyStoneCommand command = new HeavyStoneCommand(board, inventory, 0, 3, 4);

        command.Execute();
        Assert.Equal('@', board.GetCellInfo(3, 4));
        Assert.Equal(1, inventory.HeavyLeft(0));

        command.Undo();
        Assert.Equal(' ', board.GetCellInfo(3, 4));
        Assert.Equal(2, inventory.HeavyLeft(0));
    }

    [Fact]
    public void EraserCommand_ExecuteThenUndo_RestoresErasedStoneAndInventory()
    {
        Board board = new Board(10);
        board.PlayMove('O', 2, 2);
        StoneInventory inventory = new StoneInventory();
        EraserCommand command = new EraserCommand(board, inventory, 0, 2, 2);

        command.Execute();
        Assert.Equal(' ', board.GetCellInfo(2, 2));
        Assert.Equal(1, inventory.EraserLeft(0));

        command.Undo();
        Assert.Equal('O', board.GetCellInfo(2, 2));
        Assert.Equal(2, inventory.EraserLeft(0));
    }

    [Fact]
    public void Commands_ToCommandString_UseSaveFormat()
    {
        Board board = new Board(10);
        StoneInventory inventory = new StoneInventory();

        Assert.Equal("H3:4", new HeavyStoneCommand(board, inventory, 0, 3, 4).ToCommandString());
        Assert.Equal("E5:6", new EraserCommand(board, inventory, 1, 5, 6).ToCommandString());
    }

    // ---------- Game rules ----------

    [Fact]
    public void Plus_EraserOnOpponentOrdinaryStone_RemovesIt()
    {
        Game game = CreatePlusGame();
        PlayTurn(game, 'O', 1, 1);   // P1
        PlayTurn(game, 'O', 2, 2);   // P2

        bool applied = game.TryApplyMove('E', 2, 2);   // P1 erases

        Assert.True(applied);
        Assert.Equal(' ', game.board.GetCellInfo(2, 2));
    }

    [Fact]
    public void Plus_EraserOnHeavyStone_IsRejected()
    {
        Game game = CreatePlusGame();
        PlayTurn(game, 'O', 1, 1);   // P1
        PlayTurn(game, 'H', 2, 2);   // P2 Heavy

        bool applied = game.TryApplyMove('E', 2, 2);

        Assert.False(applied);
        Assert.Equal('#', game.board.GetCellInfo(2, 2));
    }

    [Fact]
    public void Plus_EraserOnOwnStone_IsRejected()
    {
        Game game = CreatePlusGame();
        PlayTurn(game, 'O', 1, 1);   // P1
        PlayTurn(game, 'O', 2, 2);   // P2

        bool applied = game.TryApplyMove('E', 1, 1);

        Assert.False(applied);
        Assert.Equal('X', game.board.GetCellInfo(1, 1));
    }

    [Fact]
    public void Plus_ThirdHeavyStone_IsRejected()
    {
        Game game = CreatePlusGame();
        PlayTurn(game, 'H', 1, 1);   // P1
        PlayTurn(game, 'O', 9, 9);   // P2
        PlayTurn(game, 'H', 1, 2);   // P1
        PlayTurn(game, 'O', 9, 8);   // P2

        bool applied = game.TryApplyMove('H', 1, 3);

        Assert.False(applied);
        Assert.Equal(' ', game.board.GetCellInfo(1, 3));
    }

    [Fact]
    public void Plus_HeavyStonesInLineOfFive_CountTowardsWin()
    {
        Game game = CreatePlusGame();
        PlayTurn(game, 'H', 1, 1);
        PlayTurn(game, 'O', 9, 9);
        PlayTurn(game, 'H', 1, 2);
        PlayTurn(game, 'O', 9, 8);
        PlayTurn(game, 'O', 1, 3);
        PlayTurn(game, 'O', 9, 7);
        PlayTurn(game, 'O', 1, 4);
        PlayTurn(game, 'O', 9, 6);

        game.TryApplyMove('O', 1, 5);

        Assert.Equal(GameResult.Player1Wins, game.CheckForWinner(1, 5));
    }

    // ---------- Helpers ----------

    private Game CreatePlusGame()
    {
        GameConfig config = new GameConfig();
        config.GameFamily = GameFamily.Gomoku;
        config.GomokuVariant = GomokuVariant.Plus;
        config.GameMode = GameMode.HumanVsHuman;
        return GameFactory.CreateGame(config);
    }

    // Plays a move for the current player, then hands the turn to the other player
    private void PlayTurn(Game game, char piece, int row, int col)
    {
        Assert.True(game.TryApplyMove(piece, row, col));
        game.AdvanceTurn();
    }
}