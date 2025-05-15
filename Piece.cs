public class Piece
{
    public string pieceType;
    public int x, y;

    public Piece(string type, int inputX, int inputY)
    {
        this.pieceType = type;
        this.x = inputX;
        this.y = inputY;
    }
}