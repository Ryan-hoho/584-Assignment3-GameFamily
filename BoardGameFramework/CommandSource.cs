public interface ICommandSource
{
    string? GetNextCommand();
    bool HasMore();
}






public class ConsoleCommandSource : ICommandSource
{
    public string? GetNextCommand() => Console.ReadLine();
    public bool HasMore() => true; // 互動模式持續等輸入
}

public class ScriptCommandSource : ICommandSource
{
    private readonly Queue<string> commands;

    public ScriptCommandSource(string script)
    {
        commands = new Queue<string>(
            script.Split(',').Select(s => s.Trim())
        );
    }

    public string? GetNextCommand() => commands.Count > 0 ? commands.Dequeue() : null;
    public bool HasMore() => commands.Count > 0;
}