public class Piece
{
    public string pieceType;
    public int x, y;

    public Piece(string type, int inputX, int inputY)
    {
        pieceType = type;
        x = inputX;
        y = inputY;
    }
}