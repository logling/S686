class Program
{

    static void Main()
    {
        Console.WriteLine("hi");

        PuzzleGenerator p = new PuzzleGenerator(4);
        p.GenerateGame();
    }
}