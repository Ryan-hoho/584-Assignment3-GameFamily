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
        // A full Turn needs：
        // Player 1 Move
        // Player 2 Move
        if (undoStack.Count < 2)
        {
            return false;
        }

        // ---------------------------------------------
        // Undo latest action
        // ---------------------------------------------
        MoveCommand latestCommand = undoStack.Pop();
        latestCommand.Undo();
        redoStack.Push(latestCommand);

        // ---------------------------------------------
        // Undo previous action
        // ---------------------------------------------
        MoveCommand previousCommand = undoStack.Pop();
        previousCommand.Undo();
        redoStack.Push(previousCommand);
        return true;
    }

    // =========================================================
    // Redo
    // =========================================================

    public bool Redo()
    {
        if (redoStack.Count < 2)
        {
            return false;
        }

        // ---------------------------------------------
        // Redo Previous action
        // ---------------------------------------------

        MoveCommand previousCommand = redoStack.Pop();
        previousCommand.Redo();
        undoStack.Push(previousCommand);

        // ---------------------------------------------
        // Redo latest action
        // ---------------------------------------------

        MoveCommand latestCommand = redoStack.Pop();
        latestCommand.Redo();
        undoStack.Push(latestCommand);

        return true;
    }

    // Clear
    public void Clear()
    {
        undoStack.Clear();
        redoStack.Clear();
    }

    // Get Undo History
    public IReadOnlyList<MoveCommand> GetUndoHistory()
    {
        // Stack.ToArray() 的順序是：newest -> oldest
        // 但是 Save 時通常希望：oldest -> newest
        // 所以使用 Reverse() 轉回執行順序。
        return undoStack
            .Reverse()
            .ToList();
    }

    // Restore History
    public void RestoreFromSave(
        IEnumerable<MoveCommand> previousCommands)
    {
        undoStack.Clear();
        redoStack.Clear();

        // previousCommands 預期按照：oldest -> newest
        foreach (MoveCommand command in previousCommands)
        {
            undoStack.Push(command);
        }
    }
}