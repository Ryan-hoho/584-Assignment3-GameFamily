using static System.Console;

class BoardGameFramework
{
    static void Main(string[] args)
    {
        // Automated command parsing & testing mode
        if (args.Length != 0)
        {
            // TO-DO
        }


        // Normal game selection mode
        else
        {
            GameConfig config = GameSelection.Select();
            //Game.Start();
            
        }
    }
}
