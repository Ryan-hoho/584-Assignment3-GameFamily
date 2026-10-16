public class FogVisibilityTests
{
    [Fact]
    public void Fog_OwnStone_IsVisibleWithItsNeighbours()
    {
        Board board = new Board(10);
        board.PlayMove('X', 5, 5);

        char[,] view = new FogVisibility().GetVisibleBoard(board, 'X');

        // view is 0-based, board is 1-based
        Assert.Equal('X', view[4, 4]);
        Assert.Equal(' ', view[3, 3]);
        Assert.Equal(' ', view[5, 5]);
    }

    [Fact]
    public void Fog_CellsFarFromOwnStones_AreHidden()
    {
        Board board = new Board(10);
        board.PlayMove('X', 5, 5);
        board.PlayMove('O', 9, 9);

        char[,] view = new FogVisibility().GetVisibleBoard(board, 'X');

        Assert.Equal('?', view[8, 8]);
        Assert.Equal('?', view[0, 0]);
    }

    [Fact]
    public void Fog_OpponentStoneNextToOwnStone_IsVisible()
    {
        Board board = new Board(10);
        board.PlayMove('X', 5, 5);
        board.PlayMove('O', 5, 6);

        char[,] view = new FogVisibility().GetVisibleBoard(board, 'X');

        Assert.Equal('O', view[4, 5]);
    }

    [Fact]
    public void Fog_EachPlayer_SeesOnlyAroundOwnStones()
    {
        Board board = new Board(10);
        board.PlayMove('X', 1, 1);
        board.PlayMove('O', 10, 10);

        char[,] xView = new FogVisibility().GetVisibleBoard(board, 'X');
        char[,] oView = new FogVisibility().GetVisibleBoard(board, 'O');

        Assert.Equal('?', xView[9, 9]);
        Assert.Equal('?', oView[0, 0]);
        Assert.Equal('O', oView[9, 9]);
    }

    [Fact]
    public void Fog_GetVisibleBoard_DoesNotChangeTrueBoard()
    {
        Board board = new Board(10);
        board.PlayMove('X', 5, 5);
        board.PlayMove('O', 9, 9);

        new FogVisibility().GetVisibleBoard(board, 'X');

        Assert.Equal('O', board.GetCellInfo(9, 9));
        Assert.Equal(' ', board.GetCellInfo(1, 1));
    }

    [Fact]
    public void FullVisibility_ShowsEveryCell()
    {
        Board board = new Board(10);
        board.PlayMove('X', 1, 1);
        board.PlayMove('O', 10, 10);

        char[,] view = new FullVisibility().GetVisibleBoard(board, 'X');

        Assert.Equal('X', view[0, 0]);
        Assert.Equal('O', view[9, 9]);
        Assert.Equal(' ', view[4, 4]);
    }
}