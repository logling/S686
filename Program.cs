class Program
{
    static void Main()
    {

    }

    private void printGrid(Board board)
    {
        Console.WriteLine("printing grid...");
        int count = 0;

        for (int i = 0; i < board.Size; i++)
        {
            for (int j = 0; j < board.Size; j++)
            {
                if (board.Grid[i, j] != null)
                {
                    Console.WriteLine($"Piece at ({i}, {j}): {board.Grid[i, j]?.pieceType}");
                    count++;
                }
            }
        }

        if (count == 0)
            Console.WriteLine("no piece");
    }

    private void generatePiece(Board board)
    {
        Random rand = new Random();
        Piece piece = new Piece("pawn", rand.Next(board.Size), rand.Next(board.Size));
        Console.WriteLine($"new piece {piece.pieceType}, at ({piece.x},{piece.y})");
        board.RegisterPiece(piece);
    }
}