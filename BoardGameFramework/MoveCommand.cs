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


public class ReversiMoveCommand : MoveCommand  //待修正
{
    private readonly Board board;
    private readonly char symbol;
    private readonly int row;
    private readonly int column;

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
        board.PlayMove(symbol, row, column);
    }

    public override void Undo()
    {
        board.RemovePiece(row, column);
    }
}
