class Program
{
    static void Main()
    {
        Console.WriteLine("hi");
        PuzzleGenerator p = new PuzzleGenerator(4);
        p.GenerateGame();
        Board board = p.GetBoard();

        Console.WriteLine($"보드 크기: {board.Size}x{board.Size}");
        for (int y = 0; y < board.Size; y++)
        {
            for (int x = 0; x < board.Size; x++)
            {
                if (board.Grid[x, y] != null)
                {
                    string pieceType = board.Grid[x, y]!.pieceType;
                    char symbol = pieceType[0]; // 첫 글자만 사용
                    Console.Write($"[{symbol}]");
                }
                else
                {
                    Console.Write("[ ]");
                }
            }
            Console.WriteLine();
        }
    }
}