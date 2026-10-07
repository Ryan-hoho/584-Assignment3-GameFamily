using static System.Console;

public class Board
{
    public int Size { get; }
    private char[,] board;

    // Constructor
    public Board(int s)
    {
        Size = s;
        board = new char[s,s];
        // Set all the board array elements to blank chars
        for (int i = 0; i < s; i++)
        {
            for (int j = 0; j < s; j++)
            {
                board[i,j] = ' ';
            }
        }
    }

    //Print current state of board
    public void GetBoard()
    {
        // Print columns number
        Write("     "); 
        for (int i = 1; i < Size + 1; i++)
        {
            Write(i + "   ");     
        }
        WriteLine(); 

        // Print board
        string row_div = "+";
        string col_div = "|";
        for (int i = 1; i < Size + 1; i++)
        {
            row_div += "---+";
        }
        for (int i = 0; i < Size; i++)
        {
            WriteLine("   " + row_div);
            if (i >= 9) // Two digits row have 1 less space
                Write(i + 1 + " " + col_div);
            else  
                Write(i + 1 + "  " + col_div);
            for (int j = 0; j < Size; j++)
            {
                Write(" " + board[i,j] + " " + col_div); 
            }
            WriteLine();
        }
        WriteLine("   " + row_div);
    }

    public void PlayMove(char s, int r, int c)
    {
        board[r-1, c-1] = s;
    }
        public void RemovePiece(int r, int c)
    {
        board[r - 1, c - 1] = ' ';
    }


    public char GetCellInfo(int r, int c)
    {
        return board[r-1,c-1];
    }
}
