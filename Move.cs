public class Move
{
    public Vector2Int i, f; // initial, final position

    public Move(Vector2Int inputI, Vector2Int inputF)
    {
        i = inputI;
        f = inputF;
    }

    public Move reverseMove()
    {
        return new Move(f, i);
    }
}