public interface IVisibilityStrategy
{
    char[,] GetVisibleBoard(Board board, char symbol);
}