public interface IGameFactory
{
    Game CreateGame(GameConfig config);
}

public class GomokuFactory : IGameFactory
{
    public Game CreateGame(GameConfig config)
    {
        if (config.GomokuVariant == GomokuVariant.Standard)
        {
            return new GomokuGame(config);
        }
        else if (config.GomokuVariant == GomokuVariant.Plus)
        {
            return new GomokuPlus(config); 
        }
        else
        {
            return new GomokuFog(config); 
        } 
    }
}

public class ReversiFactory : IGameFactory
{
    public Game CreateGame(GameConfig config)
    {
        if (config.ReversiVariant == ReversiVariant.Standard)
        {
            return new ReversiGame(config); 
        }
        else if (config.ReversiVariant == ReversiVariant.Anti)
        {
            return new AntiReversi(config);
        }
        else
        {
            return new CornerReversi(config);
        }
    }
}

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
            throw new ArgumentException("Invalide Game Family.");
        }

        return factory.CreateGame(config);
    }
}