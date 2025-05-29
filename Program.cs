class Program
{
    static void Main()
    {
        Console.WriteLine("initializing...\n");
        DatabaseManager.InitializeDatabase();
        PuzzleRefinery.InitiateSearch().Wait();
    }
}