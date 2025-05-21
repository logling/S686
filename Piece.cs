public class Piece
{
    public string pieceType; // type like king or pawn or knight
    public int x, y;
    public bool isFrog;
    public bool isToad;
    public bool isLily;

    public Piece(string type, int inputX, int inputY)
    {
        pieceType = type;
        x = inputX;
        y = inputY;
    }

    public Piece Clone()
    {
        Piece clone = new Piece(pieceType, x, y);
        clone.isFrog = this.isFrog;
        clone.isToad = this.isToad;
        clone.isLily = this.isLily;
        return clone;
    }
}