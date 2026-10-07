public class CommandHistory
{
    // 儲存已經執行完成的 MoveCommand。
    // Stack 最上面的 Command 就是最近執行的 Move。
    private Stack<MoveCommand> undoStack = new Stack<MoveCommand>();

    // 儲存已經被 Undo 的 MoveCommand，
    // 讓玩家之後可以使用 Redo 恢復。
    private Stack<MoveCommand> redoStack = new Stack<MoveCommand>();


    // =========================================================
    // Execute
    // =========================================================

    public void Execute(MoveCommand command)
    {
        // 執行這一步 Move。
        command.Execute();

        // 執行成功後，加入 Undo history。
        undoStack.Push(command);

        // 如果 Undo 之後做了新的 Move，
        // 舊的 Redo 路徑就不能再使用。
        // 例如：
        // P1 -> A
        // P2 -> B
        // Undo
        // P1 -> C
        // 此時不能再 Redo A/B。
        redoStack.Clear();
    }

    // =========================================================
    // Undo
    // =========================================================

    public bool Undo()
    {
        // 一個完整 Turn 需要：
        // Player 1 Move
        // Player 2 Move
        // 因此如果少於兩個 Command，
        // 代表目前沒有完整 Turn 可以 Undo。
        if (undoStack.Count < 2)
        {
            return false;
        }

        // ---------------------------------------------
        // Undo latest action
        // ---------------------------------------------
        // 因為 Stack 是 Last In First Out，
        // Player 2 的 Move 是最後執行的，
        // 所以必須先 Undo Player 2。

        MoveCommand latestCommand = undoStack.Pop();
        latestCommand.Undo();
        redoStack.Push(latestCommand);

        // ---------------------------------------------
        // Undo previous action
        // ---------------------------------------------

        MoveCommand previousCommand = undoStack.Pop();
        previousCommand.Undo();
        redoStack.Push(previousCommand);

        // 成功 Undo 一個完整 Turn。
        return true;
    }

    // =========================================================
    // Redo
    // =========================================================

    public bool Redo()
    {
        // Redo 同樣需要完整的一個 Turn。
        // 因此必須至少有兩個被 Undo 的 Command。
        if (redoStack.Count < 2)
        {
            return false;
        }

        // ---------------------------------------------
        // Redo Previous action
        // ---------------------------------------------
        // Undo 時：
        // P2 被 Push 進 redoStack
        // P1 再被 Push 進 redoStack
        // 所以現在 Stack 最上面會是 P1。
        // Redo 時剛好先恢復 P1。

        MoveCommand previousCommand = redoStack.Pop();
        previousCommand.Redo();
        undoStack.Push(previousCommand);


        // ---------------------------------------------
        // Redo latest action
        // ---------------------------------------------

        MoveCommand latestCommand = redoStack.Pop();
        latestCommand.Redo();
        undoStack.Push(latestCommand);


        // 成功恢復一個完整 Turn。
        return true;
    }


    // =========================================================
    // Clear
    // =========================================================
    public void Clear()
    {
        // 清除所有 Undo / Redo history。
        // 之後 Load 新遊戲時可能會使用。
        undoStack.Clear();
        redoStack.Clear();
    }

    // =========================================================
    // Get Undo History
    // =========================================================
    public IReadOnlyList<MoveCommand> GetUndoHistory()
    {
        // Stack.ToArray() 的順序是：
        // newest -> oldest
        // 但是 Save 時通常希望：
        // oldest -> newest
        // 所以使用 Reverse() 轉回執行順序。
        return undoStack
            .Reverse()
            .ToList();
    }

    // =========================================================
    // Restore History
    // =========================================================
    public void RestoreFromSave(
        IEnumerable<MoveCommand> previousCommands)
    {
        // Load 遊戲前先清除舊 history。
        undoStack.Clear();
        redoStack.Clear();

        // previousCommands 預期按照：
        // oldest -> newest
        // 的順序傳入。
        foreach (MoveCommand command in previousCommands)
        {
            undoStack.Push(command);
        }
    }
}