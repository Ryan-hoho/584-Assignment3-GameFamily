public class AntiReversiStrategy : ReversiGreedyStrategy
{
    // Larger than any possible flip count, so a corner is only taken when nothing else is legal
    const int CORNERPENALTY = 100;

    protected override int Score(Board board, int row, int col, int flips)
    {
        // Fewer flips is better, so the score is the negative flip count.
        int score = 0 - flips;
        if (IsCorner(board, row, col))
        {
            score = score - CORNERPENALTY;
        }
        return score;
    }
}
