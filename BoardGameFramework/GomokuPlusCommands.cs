public class HeavyStoneCommand : MoveCommand
{
    private readonly Board board;
    private readonly StoneInventory inventory;
    private readonly int player;
    private readonly int row;
    private readonly int column;

    public HeavyStoneCommand(Board board, StoneInventory inventory, int player, int row, int column)
    {
        this.board = board;
        this.inventory = inventory;
        this.player = player;
        this.row = row;
        this.column = column;
    }

    public override void Execute()
    {
        inventory.UseHeavy(player);
        board.PlayMove(Piece.gomokuHeavy[player], row, column);
    }

    public override void Undo()
    {
        board.RemovePiece(row, column);
        inventory.ReturnHeavy(player);
    }

    public override string ToCommandString()
    {
        return $"{Piece.gomokuStone[1]}{row}:{column}";
    }
}

public class EraserCommand : MoveCommand
{
    private readonly Board board;
    private readonly StoneInventory inventory;
    private readonly int player;
    private readonly int row;
    private readonly int column;
    private char erasedSymbol;

    public EraserCommand(Board board, StoneInventory inventory, int player, int row, int column)
    {
        this.board = board;
        this.inventory = inventory;
        this.player = player;
        this.row = row;
        this.column = column;
    }

    public override void Execute()
    {
        inventory.UseEraser(player);
        erasedSymbol = board.GetCellInfo(row, column);
        board.RemovePiece(row, column);
    }

    public override void Undo()
    {
        board.PlayMove(erasedSymbol, row, column);
        inventory.ReturnEraser(player);
    }

    public override string ToCommandString()
    {
        return $"{Piece.gomokuStone[2]}{row}:{column}";
    }
}