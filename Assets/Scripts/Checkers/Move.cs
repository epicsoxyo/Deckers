public struct Move
{

    public int x;
    public int y;

    public Move(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Move operator *(int a, Move b)
    {
        int x = a * b.x;
        int y = a * b.y;

        return new Move(x, y);
    }

    public static Move operator *(Move a, int b)
    {
        return b * a;
    }

}