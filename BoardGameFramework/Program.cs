class BoardGameFramework
{
    static void Main(string[] args)
    {
        IGameObserver view = new ConsoleView();
        // Automated command parsing & testing mode
        if (args.Length != 0)
        {
            Console.WriteLine("CLI mode started");
            GameConfig config = GameSelection.ParseCliArgs(args, out string script);
            Game game = GameFactory.CreateGame(config);
            game.Subscribe(view);
            new TestRunner().Run(game, script);
            
        }

        // Game Selection Mode
        else
        {
            Game? game = GameSelection.SelectGame();
            game?.Subscribe(view);
            while (game != null)
            {
                Game? next = game.Start();
                game.Unsubscribe(view);  // added: the finished game drops the view
                // A loaded game is a new object, so subscribe the view again.
                if (next != null && next != game)
                {
                    next.Subscribe(view);
                }
                game = next;
            }
        }
    }
}
