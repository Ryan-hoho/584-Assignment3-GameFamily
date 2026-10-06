public class ReversiRulesTests
{
    [Fact]
    public void FindFlips_OpeningMove_ReturnsExpectedDisk()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        List<(int row, int col)> flips =
            FlankingRules.FindFlips(
                board,
                3,
                4,
                'X');

        Assert.Single(flips);
        Assert.Contains((4, 4), flips);
    }

    [Fact]
    public void FindLegalMoves_InitialBoard_ReturnsFourMovesForX()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        List<(int row, int col)> moves =
            ReversiRules.FindLegalMoves(
                board,
                'X');

        Assert.Equal(4, moves.Count);

        Assert.Contains((3, 4), moves);
        Assert.Contains((4, 3), moves);
        Assert.Contains((5, 6), moves);
        Assert.Contains((6, 5), moves);
    }

    [Fact]
    public void PlaceDisk_ValidMove_PlacesAndFlipsDisk()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);

        bool success =
            ReversiRules.PlaceDisk(
                board,
                3,
                4,
                'X');

        Assert.True(success);

        Assert.Equal(
            'X',
            board.GetCellInfo(3, 4));

        Assert.Equal(
            'X',
            board.GetCellInfo(4, 4));
    }
}