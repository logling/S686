public class TestMethods
{
    public static void PositionCombination()
    {
        Console.WriteLine("=== PositionCombination() 테스트 ===");
        
        var allCombinations = PuzzleRefinery.PositionCombination().ToList(); // test subject combination
        Console.WriteLine($"전체 조합 개수: {allCombinations.Count}개\n");
        
        Console.WriteLine("처음 20개:");
        for (int i = 0; i < Math.Min(20, allCombinations.Count); i++)
        {
            var combo = allCombinations[i];
            Console.Write($"  {i + 1:D2}: ");
            for (int j = 0; j < combo.Length; j++)
            {
                Console.Write($"({combo[j].x},{combo[j].y})");
                if (j < combo.Length - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }
        
        Console.WriteLine("\n마지막 20개:");
        int start = Math.Max(0, allCombinations.Count - 20);
        for (int i = start; i < allCombinations.Count; i++)
        {
            var combo = allCombinations[i];
            Console.Write($"  {i + 1:D4}: ");
            for (int j = 0; j < combo.Length; j++)
            {
                Console.Write($"({combo[j].x},{combo[j].y})");
                if (j < combo.Length - 1) Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
    
    public static void TypeCombination()
    {
        Console.WriteLine("=== TypeCombination() 테스트 ===");
        
        var allCombinations = PuzzleRefinery.TypeCombination().ToList(); // test subject combination
        Console.WriteLine($"전체 조합 개수: {allCombinations.Count}개\n");
        
        Console.WriteLine("처음 20개:");
        for (int i = 0; i < Math.Min(20, allCombinations.Count); i++)
        {
            var combo = allCombinations[i];
            Console.WriteLine($"  {i + 1:D2}: [{string.Join(", ", combo)}]");
        }
        
        Console.WriteLine("\n마지막 20개:");
        int start = Math.Max(0, allCombinations.Count - 20);
        for (int i = start; i < allCombinations.Count; i++)
        {
            var combo = allCombinations[i];
            Console.WriteLine($"  {i + 1:D2}: [{string.Join(", ", combo)}]");
        }
    }
    
    public static void Permutations()
    {
        Console.WriteLine("=== Permutations() 테스트 ===");
        
        string[] test = { "rook", "rook", "knight", "pawn" }; // test subject list
        Console.WriteLine($"테스트 배열: [{string.Join(", ", test)}]");
        
        var allPermutations = PuzzleRefinery.Permutations(test).ToList();
        Console.WriteLine($"전체 순열 개수: {allPermutations.Count}개\n");
        
        Console.WriteLine("처음 20개:");
        for (int i = 0; i < Math.Min(20, allPermutations.Count); i++)
        {
            var perm = allPermutations[i];
            Console.WriteLine($"  {i + 1:D2}: [{string.Join(", ", perm)}]");
        }
        
        if (allPermutations.Count > 20)
        {
            Console.WriteLine("\n마지막 20개:");
            int start = Math.Max(0, allPermutations.Count - 20);
            for (int i = start; i < allPermutations.Count; i++)
            {
                var perm = allPermutations[i];
                Console.WriteLine($"  {i + 1:D2}: [{string.Join(", ", perm)}]");
            }
        }
        else
        {
            Console.WriteLine("\n전체 개수가 20개 이하입니다.");
        }
    }
}