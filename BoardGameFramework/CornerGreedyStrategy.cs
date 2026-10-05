public class CornerGreedyStrategy : ReversiGreedyStrategy
{
    // Larger than any possible flip count, so a corner always beats a non-corner move
    const int CORNERBONUS = 100;
    protected override int Score(Board board, int row, int col, int flips)
    {
        int score = base.Score(board, row, col, flips);
        if (IsCorner(board, row, col))
        {
            score = score + CORNERBONUS;
        }
        return score;
    }
}
