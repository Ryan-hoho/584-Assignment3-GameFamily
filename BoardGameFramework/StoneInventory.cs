public class StoneInventory
{
    const int STARTCOUNT = 2;
    private readonly int[] heavyLeft = { STARTCOUNT, STARTCOUNT };
    private readonly int[] eraserLeft = { STARTCOUNT, STARTCOUNT };

    public int HeavyLeft(int player)
    {
        return heavyLeft[player];
    }

    public int EraserLeft(int player)
    {
        return eraserLeft[player];
    }

    public void UseHeavy(int player)
    {
        if (heavyLeft[player] == 0)
        {
            throw new InvalidOperationException("No Heavy stones left.");
        }
        heavyLeft[player]--;
    }

    public void ReturnHeavy(int player)
    {
        heavyLeft[player]++;
    }

    public void UseEraser(int player)
    {
        if (eraserLeft[player] == 0)
        {
            throw new InvalidOperationException("No Eraser stones left.");
        }
        eraserLeft[player]--;
    }

    public void ReturnEraser(int player)
    {
        eraserLeft[player]++;
    }
}