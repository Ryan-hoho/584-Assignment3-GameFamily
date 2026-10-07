// ├── MoveCommand.cs
// │   ├── MoveCommand
// │   ├── GomokuMoveCommand
// │   └── ReversiMoveCommand

public abstract class MoveCommand
{
    // 執行這一步棋。
    // 具體怎麼執行，由 GomokuMoveCommand 或
    // ReversiMoveCommand 自己決定。
    public abstract void Execute();

    // 復原這一步棋。
    // Command 必須自己知道如何恢復 Execute 前的狀態。
    public abstract void Undo();

    // Redo 通常就是重新執行同一個 Command。
    // 如果未來某種 move 有特殊 redo 行為，
    // subclass 仍然可以 override。
    public virtual void Redo()
    {
        Execute();
    }
}

public class GomokuMoveCommand : MoveCommand
{
    private readonly Board board;
    private readonly char symbol;
    private readonly int row;
    private readonly int column;

    public GomokuMoveCommand(
        Board board,
        char symbol,
        int row,
        int column)
    {
        this.board = board;
        this.symbol = symbol;
        this.row = row;
        this.column = column;
    }

    public override void Execute()
    {
        board.PlayMove(symbol, row, column);
    }

    public override void Undo()
    {
        board.RemovePiece(row, column);
    }


}


public class ReversiMoveCommand : MoveCommand
{
    private readonly Board board;
    private readonly char symbol;
    private readonly int row;
    private readonly int column;

    private List<(int row, int col)> flippedDisks =
        new List<(int row, int col)>();

    public ReversiMoveCommand(
        Board board,
        char symbol,
        int row,
        int column)
    {
        this.board = board;
        this.symbol = symbol;
        this.row = row;
        this.column = column;
    }

    public override void Execute()
    {
        flippedDisks =
            FlankingRules.FindFlips(
                board,
                row,
                column,
                symbol);

        if (flippedDisks.Count == 0)
        {
            throw new InvalidOperationException(
                "Cannot execute an invalid Reversi move.");
        }

        ReversiRules.PlaceDisk(
            board,
            row,
            column,
            symbol);
    }

    public override void Undo()
    {
        // Remove the disk that was originally placed.
        board.RemovePiece(row, column);

        // Restore all disks that were flipped by this move.
        char opponentSymbol =
            (symbol == 'X') ? 'O' : 'X';

        foreach ((int flipRow, int flipCol) in flippedDisks)
        {
            board.PlayMove(
                opponentSymbol,
                flipRow,
                flipCol);
        }
    }
}
