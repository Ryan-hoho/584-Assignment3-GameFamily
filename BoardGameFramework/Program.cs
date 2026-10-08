class BoardGameFramework
{
    static void Main(string[] args)
    {
        // Automated command parsing & testing mode
        if (args.Length != 0)
        {
            Console.WriteLine("CLI mode started");
            GameConfig config = GameSelection.ParseCliArgs(args, out string script);
            Game game = GameFactory.CreateGame(config);
            new TestRunner().Run(game, script);
        }

        // Game Selection Mode
        else
        {
            Game? game = GameSelection.SelectGame();
            while (game != null)
            {
                game = game.Start();
            }
        }
    }
}
