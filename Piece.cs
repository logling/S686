public class Piece
{
    public string pieceType; // type like king or pawn or knight
    public int x, y;
    public bool isFrog;
    public bool isToad;

    public Piece(string type, int inputX, int inputY)
    {
        pieceType = type;
        x = inputX;
        y = inputY;
    }
}