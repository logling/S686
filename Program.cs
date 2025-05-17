class Program
{
    static void Main()
    {
        // just testing smth
        PuzzleGenerator p = new PuzzleGenerator();
        List<List<int>> partitions = p.GetAllPartitions(4);

        PuzzleGenerator.Shuffle(partitions);
        List<int> partition = partitions[0];
    }

    // don't mind these
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