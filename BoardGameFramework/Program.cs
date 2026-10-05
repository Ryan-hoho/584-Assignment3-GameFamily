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
            Game game;

            if (config.GameFamily == GameFamily.Gomoku)
            {
                game = new GomokuGame(config);
            }
            else
            {
                game = new ReversiGame(config);
            }

            game.Start();
        }
    }
}
