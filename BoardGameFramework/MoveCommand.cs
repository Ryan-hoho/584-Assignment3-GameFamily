// ├── MoveCommand.cs
// │   ├── MoveCommand
// │   ├── GomokuMoveCommand
// │   └── ReversiMoveCommand
// │   └── PassMoveCommand

public abstract class MoveCommand
{
    // Execute the move. The implementation is handled by the 3 types of command
    public abstract void Execute();

    // Each Command must know how to restore the state before Execute().
    public abstract void Undo();

    // Redo typically re-executes the same Command.
    public virtual void Redo()
    {
        Execute();
    }    
    public abstract string ToCommandString();

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
    public override string ToCommandString()
    {
        return $"O{row}:{column}";
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
            (symbol == Piece.ordinary[0]) ? Piece.ordinary[1] : Piece.ordinary[0];

        foreach ((int flipRow, int flipCol) in flippedDisks)
        {
            board.PlayMove(
                opponentSymbol,
                flipRow,
                flipCol);
        }
    }
    public override void Redo()

    {
        // Restore the placed disk.
        board.PlayMove(symbol, row, column);
        // Flip the same disks again.
        foreach ((int flipRow, int flipCol) in flippedDisks)
        {
            board.PlayMove(
                symbol,
                flipRow,
                flipCol
            );
        }
    }


    public override string ToCommandString()
    {
        return $"P{row}:{column}";
    }
}
// apply pass command into history
public class PassCommand : MoveCommand 
{
    public override void Execute() { }
    public override void Undo() { }
    public override string ToCommandString()
    {
        return "PASS";
    }
}