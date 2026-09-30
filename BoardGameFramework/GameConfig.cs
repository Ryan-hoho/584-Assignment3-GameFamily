public enum GameType
{
    Gomoku, 
    Reversi,
    Load
}

public enum GomokuVariant
{
    Standard,
    Plus,
    Fog
}

public enum ReversiVariant
{
    Standard,
    Anti,
    Corner
}

public enum GameMode
{
    HumanVsHuman,
    HumanVsComputer
}

public enum ComputerType
{
    DumbAI,
    SmartAI
}

public class GameConfig
{
    // Properties
    public GameType GameType { get; set; }
    public GomokuVariant? GomokuVariant { get; set; }
    public ReversiVariant? ReversiVariant { get; set; }
    public GameMode GameMode { get; set; }
    public ComputerType? ComputerType { get; set; }
}