public class ReversiMoveCommandTests
{
    [Fact]
    public void Execute_ValidMove_PlacesAndFlipsDisk()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        ReversiMoveCommand command =
            new ReversiMoveCommand(
                board,
                'X',
                3,
                4);

        command.Execute();

        Assert.Equal(
            'X',
            board.GetCellInfo(3, 4));

        Assert.Equal(
            'X',
            board.GetCellInfo(4, 4));
    }

    [Fact]
    public void Undo_AfterExecute_RestoresPreviousBoardState()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        ReversiMoveCommand command =
            new ReversiMoveCommand(
                board,
                'X',
                3,
                4);

        command.Execute();
        command.Undo();

        Assert.Equal(
            ' ',
            board.GetCellInfo(3, 4));

        Assert.Equal(
            'O',
            board.GetCellInfo(4, 4));
    }

    [Fact]
    public void Redo_AfterUndo_ReappliesMoveAndFlips()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        ReversiMoveCommand command =
            new ReversiMoveCommand(
                board,
                'X',
                3,
                4);

        command.Execute();
        command.Undo();
        command.Redo();

        Assert.Equal(
            'X',
            board.GetCellInfo(3, 4));

        Assert.Equal(
            'X',
            board.GetCellInfo(4, 4));
    }
}