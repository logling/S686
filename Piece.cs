public class Piece
{
    public string pieceType; // type like king or pawn or knight
    public int x, y; // 0 ~ 5, maybe 6 7

    public Piece(string type, int inputX, int inputY)
    {
        pieceType = type;
        x = inputX;
        y = inputY;
    }

    public Piece Clone() // clone piece
    {
        Piece clone = new Piece(pieceType, x, y);
        return clone;
    }
}