public static class GameFactory
{
    public static Game CreateGame(GameConfig config)
    {
        IGameFactory factory;

        if (config.GameFamily == GameFamily.Gomoku)
        {
            factory = new GomokuFactory();
        }
        else if (config.GameFamily == GameFamily.Reversi)
        {
            factory = new ReversiFactory();
        }
        else
        {
            throw new ArgumentException("Invalid Game Family.");
        }

        return factory.CreateGame(config);
    }
}

public interface IGameFactory
{
    Board CreateBoard();
    IMoveStrategy CreateMoveStrategy(GameConfig config);
    Game CreateGame(GameConfig config);
}

public class GomokuFactory : IGameFactory
{
    public Board CreateBoard()
    {
        return new Board(10);
    }

    public IMoveStrategy CreateMoveStrategy(GameConfig config)
    {
        if (config.ComputerType == ComputerType.DumbAI)
        {
            return new GomokuRandomStrategy();
        }
        else
        {
            return new GomokuSmartStrategy();
        }
    }
    
    public Game CreateGame(GameConfig config)
    {
        Board board = CreateBoard();
        Player p1 = new HumanPlayer();
        Player p2;

        if (config.GameMode == GameMode.HumanVsHuman)
        {
            p2 = new HumanPlayer();
        }
        else
        {
            IMoveStrategy strategy = CreateMoveStrategy(config);
            p2 = new ComputerPlayer(strategy, 'O');
        }
        
        if (config.GomokuVariant == GomokuVariant.Standard)
        {
            return new GomokuGame(config, board, p1, p2);
        }
        else if (config.GomokuVariant == GomokuVariant.Plus)
        {
            return new GomokuPlus(config, board, p1, p2); 
        }
        else
        {
            return new GomokuFog(config, board, p1, p2); 
        } 
    }
}

public class ReversiFactory : IGameFactory
{
    public Board CreateBoard()
    {
        Board board = new Board(8);
        ReversiRules.SetUpInitialBoard(board);
        return board;
    }
    
    public IMoveStrategy CreateMoveStrategy(GameConfig config)
    {
        if (config.ComputerType == ComputerType.DumbAI)
        {
            return new ReversiRandomStrategy();
        }
        else if (config.ReversiVariant == ReversiVariant.Standard)
        {
            return new ReversiGreedyStrategy();
        }
        else if (config.ReversiVariant == ReversiVariant.Anti)
        {
            return new AntiReversiStrategy();
        }
        else if (config.ReversiVariant == ReversiVariant.Corner)
        {
            return new CornerGreedyStrategy();
        }
        else
        {
            throw new ArgumentException("Invalid Reversi variant.");
        }
    }

    public Game CreateGame(GameConfig config)
    {
        Board board = CreateBoard();
        Player p1 = new HumanPlayer();
        Player p2;
        
        if (config.GameMode == GameMode.HumanVsHuman)
        {
            p2 = new HumanPlayer();
        }
        else
        {
            IMoveStrategy strategy = CreateMoveStrategy(config);
            p2 = new ComputerPlayer(strategy, 'O');
        }
        
        if (config.ReversiVariant == ReversiVariant.Standard)
        {
            return new ReversiGame(config, board, p1, p2);
        }
        else if (config.ReversiVariant == ReversiVariant.Anti)
        {
            return new AntiReversi(config, board, p1, p2); 
        }
        else
        {
            return new CornerReversi(config, board, p1, p2); 
        } 
    }
}

