class Test
{
    private Random rand = new Random();

    void Main()
    {
        Console.WriteLine("test start");
        
        for (int i = 1; i < 10000; i++)
        {
            int x = rand.Next(6);
            int y = rand.Next(6);

        }
    }
}