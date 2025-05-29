using System.Data.SQLite;

public class PuzzleRefinery
{
    private static int[] startPosComb = { 0, 1, 2, 3 }; // used to resume searching
    private static int[] startTypeComb = { 0, 0, 0, 0, 0, 0 }; // used to resume searching
    private static int[] currentPosComb = new int[4];
    private static int[] currentTypeComb = new int[6];
    private static int currentPermIndex = 0; // used to resume searching
    private static int totalPuzzles = 0;
    private static int validPuzzles = 0;
    private static DateTime searchStartTime;
    private static bool pauseSearch = false;
    private static string connectionString = "Data Source=puzzles.db;Version=3;";

    public static async Task InitiateSearch()
    {
        using var connection = new SQLiteConnection(connectionString);
        connection.Open();
        var transaction = connection.BeginTransaction();

        Task receivingInput = Task.Run(() =>
        {
            while (!pauseSearch)
            {
                ConsoleKeyInfo keyInput = Console.ReadKey(true);
                if (keyInput.Key == ConsoleKey.Spacebar) // if spacebar, pauseSearch = true
                {
                    Console.WriteLine("\nSearching paused");
                    pauseSearch = true;
                }
            }
        });

        Task searching = Task.Run(() =>
        {
            searchStartTime = DateTime.Now;
            var progressTimer = new System.Timers.Timer(10000); // for every 10s in background
            progressTimer.Elapsed += (sender, e) => PrintProgress(); // printProgress()
            progressTimer.Start();

            try
            {
                foreach (Vector2Int[] posComb in PositionCombination()) // for every posComb
                {
                    UpdatePosComb(posComb);
                    if (pauseSearch) break;

                    foreach (string[] typeComb in TypeCombination()) // for every typeComb
                    {
                        UpdateTypeComb(typeComb);
                        currentPermIndex = 0; // reset perm index
                        if (pauseSearch) break;

                        foreach (string[] typePerm in Permutations(typeComb)) // for every typePerm
                        {
                            currentPermIndex++; // update perm index

                            Board multiBoard = new Board(4); // generate multiBoard
                            for (int i = 0; i < 4; i++)
                            {
                                var piece = new Piece(typePerm[i], posComb[i].x, posComb[i].y); // place pieces
                                multiBoard.RegisterPiece(piece);
                            }

                            var (isValid, solution) = PuzzleSolver(multiBoard); // solve multiBoard
                            if (pauseSearch) break;
                            if (isValid) // if valid
                            {
                                validPuzzles++; // update validPuzzles
                                byte[] boardData = Transformer.ToHuffmanBinary(multiBoard); // transform multiBoard
                                DatabaseManager.SavePuzzleToDatabase(boardData, connection, transaction);
                            }
                            if (validPuzzles % 1000 == 0)
                            {
                                transaction.Commit();
                                transaction.Dispose();
                                transaction = connection.BeginTransaction();
                            }
                            totalPuzzles++; // update totalPuzzles
                        }
                    }
                }
            }

            finally
            {
                transaction?.Commit();
                transaction?.Dispose();
                progressTimer.Stop();
                progressTimer.Dispose();
                Console.WriteLine($"\n총 처리된 퍼즐: {totalPuzzles:N0}");
                Console.WriteLine($"유효한 퍼즐: {validPuzzles:N0}");
            }
        });

        await Task.WhenAny(receivingInput, searching); // when any of the 2 tasks end, end code
    }

    // position and pieceType combinations
    public static IEnumerable<Vector2Int[]> PositionCombination() // for 4 pieces & 4X4 board
    {
        for (int a = startPosComb[0]; a < 16; a++)
        {
            if (pauseSearch) yield break;
            for (int b = Math.Max(a + 1, startPosComb[1]); b < 16; b++)
            {
                for (int c = Math.Max(b + 1, startPosComb[2]); c < 16; c++)
                {
                    for (int d = Math.Max(c + 1, startPosComb[3]); d < 16; d++)
                    {
                        {
                            yield return new Vector2Int[]
                            {
                                new Vector2Int(a % 4, a / 4),
                                new Vector2Int(b % 4, b / 4),
                                new Vector2Int(c % 4, c / 4),
                                new Vector2Int(d % 4, d / 4)
                            };
                        }
                    }
                }
            }
        }
    }

    public static IEnumerable<string[]> TypeCombination() // for 4 pieces & 4 piece rules
    {
        bool isFirstCombination = true;

        for (int king = startTypeComb[0]; king <= 1; king++) // piece type rules
        {
            if (pauseSearch) yield break;
            for (int queen = isFirstCombination ? startTypeComb[1] : 0; queen <= 1; queen++) // condition needed to reset
            {
                for (int rook = isFirstCombination ? startTypeComb[2] : 0; rook <= 2; rook++)
                {
                    for (int bishop = isFirstCombination ? startTypeComb[3] : 0; bishop <= 2; bishop++)
                    {
                        for (int knight = isFirstCombination ? startTypeComb[4] : 0; knight <= 2; knight++)
                        {
                            for (int pawn = isFirstCombination ? startTypeComb[5] : 0; pawn <= 2; pawn++)
                            {
                                {
                                    isFirstCombination = false;

                                    if (king + queen + rook + bishop + knight + pawn != 4) // total piece num
                                        continue;

                                    var pieces = new string[4];
                                    int index = 0;

                                    for (int i = 0; i < king; i++) pieces[index++] = "king";
                                    for (int i = 0; i < queen; i++) pieces[index++] = "queen";
                                    for (int i = 0; i < rook; i++) pieces[index++] = "rook";
                                    for (int i = 0; i < bishop; i++) pieces[index++] = "bishop";
                                    for (int i = 0; i < knight; i++) pieces[index++] = "knight";
                                    for (int i = 0; i < pawn; i++) pieces[index++] = "pawn";

                                    yield return pieces;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    // permutations
    public static IEnumerable<T[]> Permutations<T>(T[] items)
        where T : IEquatable<T>
    {
        var cnt = new Dictionary<T, int>();
        foreach (var item in items)
        {
            cnt[item] = cnt.GetValueOrDefault(item, 0) + 1;
        }

        var result = new T[items.Length];
        var uniqueItems = cnt.Keys.ToArray();
        int index = 0;

        foreach (var perm in PermutationsRecur(0, items.Length, result, cnt, uniqueItems))
        {
            if (index >= currentPermIndex) // return starting from permIndex
                yield return (T[])perm.Clone();
            index++;
        }
    }

    public static IEnumerable<T[]> PermutationsRecur<T>(int i, int n, T[] p, Dictionary<T, int> cnt, T[] uniqueItems)
        where T : IEquatable<T>
    {
        if (pauseSearch) yield break;

        if (i == n)
        {
            yield return p;
            yield break;
        }

        foreach (var item in uniqueItems)
        {
            if (cnt[item] > 0)
            {
                p[i] = item;
                cnt[item]--;

                foreach (var result in PermutationsRecur(i + 1, n, p, cnt, uniqueItems))
                {
                    yield return result;
                }

                cnt[item]++;
            }
        }
    }

    // update variable
    private static void UpdatePosComb(Vector2Int[] posComb) // for 4X4 board
    {
        for (int i = 0; i < 4; i++)
        {
            currentPosComb[i] = posComb[i].y * 4 + posComb[i].x;
        }
    }

    private static void UpdateTypeComb(string[] typeComb)
    {
        Array.Fill(currentTypeComb, 0); // reset currentTypeComb

        foreach (string piece in typeComb) // count each pieceType and update
        {
            switch (piece)
            {
                case "king": currentTypeComb[0]++; break;
                case "queen": currentTypeComb[1]++; break;
                case "rook": currentTypeComb[2]++; break;
                case "bishop": currentTypeComb[3]++; break;
                case "knight": currentTypeComb[4]++; break;
                case "pawn": currentTypeComb[5]++; break;
            }
        }
    }

    // check unique solution
    public static (bool isUniqueSolution, List<Move> foundSolution) PuzzleSolver(Board inputBoard) // check uniqueness of inputBoard, get solution
    {
        List<Move> foundSolution = new List<Move>(); // answer moveSet
        List<Move> moveSet = new List<Move>(); // multiverse board moveSet
        Vector2Int endingPos = new Vector2Int(-1, -1); // unique ending
        bool killSwitch = false;

        CheckBoardRecur(inputBoard, foundSolution, moveSet, ref endingPos, ref killSwitch);

        if (endingPos.x != -1 && !killSwitch) return (true, foundSolution);
        else return (false, foundSolution);
    }

    private static void CheckBoardRecur(Board currentBoard, List<Move> solution, List<Move> moveSet, ref Vector2Int endingPos, ref bool killSwitch)
    {
        if (pauseSearch) return;

        if (currentBoard.CountPieces() == 1) // if reached end,
        {
            Piece lastPiece = currentBoard.GetLastPiece(); // need to check ending position

            if (endingPos.x == -1) // if first time reaching end,
            {
                endingPos.x = lastPiece.x; // update endingPos
                endingPos.y = lastPiece.y;
                solution.AddRange(moveSet);
                return;
            }
            if (lastPiece.x != endingPos.x || lastPiece.y != endingPos.y) // if different ending, invalid, killSwitch on
                killSwitch = true;
            return;
        }

        List<Move> allValidMoves = currentBoard.GetAllValidMoves(); // GetAllValidMoves()

        if (allValidMoves.Count == 0) return; // dead end. don't care

        foreach (Move move in allValidMoves) // for every move,
        {
            Board newBoard = currentBoard.Clone(); // on cloned board
            newBoard.ExecuteMove(move); // executeMove()
            moveSet.Add(move); // update current moveSet
            CheckBoardRecur(newBoard, solution, moveSet, ref endingPos, ref killSwitch); // start next recursion
            if (killSwitch) return; // if killSwitch, escape
            if (moveSet.Count > 0)
                moveSet.RemoveAt(moveSet.Count - 1); // roll back needed for next move
        }
    }

    // report
    private static void PrintProgress()
    {
        TimeSpan elapsed = DateTime.Now - searchStartTime;
        Console.WriteLine($"\n=== progress report ===");
        Console.WriteLine($"PosComb: [{string.Join(",", currentPosComb)}]");
        Console.WriteLine($"TypeComb: [{string.Join(",", currentTypeComb)}]");
        Console.WriteLine($"PermIndex: {currentPermIndex}");
        Console.WriteLine($"Total puzzles: {totalPuzzles:N0}개");
        Console.WriteLine($"Valid Puzzles: {validPuzzles:N0}개");
        Console.WriteLine($"Elapsed time: {elapsed.Hours:D2}:{elapsed.Minutes:D2}:{elapsed.Seconds:D2}");
    }

}