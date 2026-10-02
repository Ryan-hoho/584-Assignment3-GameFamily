public class Piece
{
    public char Letter { get; set; }
    public char Symbol { get; set; }
    public int RowVal { get; set; }
    public int ColVal { get; set; }
    public Player player { get; set; }

    // Piece types arrays
    public static readonly char[] gomokuStone = {'O', 'H', 'E'};
    public static readonly char[] reversiDisk = {'P'};
    public static readonly char[] ordinary = {'X','O'};
    public static readonly char[] gomokuHeavy = {'@','#'};

    public Piece(Player player, char c, char s, int row, int col)
    {
        this.player = player;
        Letter = c;
        Symbol = s;
        RowVal = row;
        ColVal = col;
    }


}