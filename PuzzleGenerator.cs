public class PuzzleGenerator
{
    private Random rand = new Random();
    private Board board;
    public string ID = "";
    public int key = 0;

    public PuzzleGenerator(int inputPieceNum)
    {
        board = new Board(inputPieceNum);
    }

    //
    // common
    private static void Shuffle<T>(List<T> inputList)
    {
        Random rand = new Random();

        for (int i = inputList.Count - 1; i > 0; i--) // Fisher-Yates shuffle
        {
            int k = rand.Next(i + 1);
            T value = inputList[k];
            inputList[k] = inputList[i];
            inputList[i] = value;
        }
    }

    //
    // partition
    public List<List<int>> GetAllPartitions(int n) // n is target of integer partition
    {
        List<List<int>> result = new List<List<int>>();
        PartitionRecursion(n, n, new List<int>(), result);
        SortPartitions(result);
        return result;
    }

    private void PartitionRecursion(int n, int max, List<int> current, List<List<int>> result)
    { // n is target number, max is needed later, current is precursor of result
        if (n == 0)
        {
            result.Add(new List<int>(current));
            return;
        }

        for (int i = Math.Min(max, n); i >= 1; i--)
        {
            current.Add(i);
            PartitionRecursion(n - i, i, current, result);
            current.RemoveAt(current.Count - 1);
        }
    }

    private void SortPartitions(List<List<int>> partitions)
    {
        foreach (var partition in partitions)
        {
            partition.Sort((a, b) => b.CompareTo(a));
        }
    }

    private void PrintPartitions(List<List<int>> partitions)
    {
        foreach (var partition in partitions)
        {
            Console.WriteLine("(" + string.Join(",", partition) + ")");
        }
    }


    //
    // path
    private string RandomPieceType() // get random pieceType according to pieceNum rules
    {
        List<string> pieceTypeCandidate = new List<string> { "king", "queen", "rook", "bishop", "knight", "pawn" };
        string randomPieceType;

        if (board.N >= 4 && board.N <= 8)
        {
            while (true)
            {
                randomPieceType = pieceTypeCandidate[rand.Next(pieceTypeCandidate.Count)]; // get random pieceType from candidate list

                if (randomPieceType == "king" || randomPieceType == "queen") // for king and queen, only 1 allowed
                    pieceTypeCandidate.Remove(randomPieceType);

                if (board.CountPiece(randomPieceType) < 2) // when less than 2 duplicates, get pieceType
                    return randomPieceType;
                else
                    pieceTypeCandidate.Remove(randomPieceType);
            }
        }
        else if (board.N == 9)
        {
            while (true)
            {
                randomPieceType = pieceTypeCandidate[rand.Next(pieceTypeCandidate.Count)]; // get random pieceType from candidate list

                if (board.CountPieces() == 8 && pieceTypeCandidate.Contains("king")) // always 1 king
                    return "king";

                if (randomPieceType == "king" || randomPieceType == "queen") // for king and queen, only 1 allowed
                    pieceTypeCandidate.Remove(randomPieceType);

                if (board.CountPiece(randomPieceType) < 2) // when less than 2 duplicates, get pieceType
                    return randomPieceType;
                else
                    pieceTypeCandidate.Remove(randomPieceType);
            }
        }
        else if (board.N == 10) // always 1 king, 1 queen, 2x other pieces
        {
            while (true)
            {
                randomPieceType = pieceTypeCandidate[rand.Next(pieceTypeCandidate.Count)]; // get random pieceType from candidate list

                if (board.CountPieces() == 9 && pieceTypeCandidate.Contains("king")) // always 1 king
                    return "king";
                if (board.CountPieces() == 9 && pieceTypeCandidate.Contains("queen")) // always 1 queen
                    return "queen";

                if (randomPieceType == "king" || randomPieceType == "queen") // for king and queen, only 1 allowed
                    pieceTypeCandidate.Remove(randomPieceType);

                if (board.CountPiece(randomPieceType) < 2)
                    return randomPieceType;
                else
                    pieceTypeCandidate.Remove(randomPieceType);
            }
        }
        else // else if (board.N == 11)
        {
            while (true)
            {
                randomPieceType = pieceTypeCandidate[rand.Next(pieceTypeCandidate.Count)]; // get random pieceType from candidate list

                if (randomPieceType == "king") // only 1 king allowed
                    pieceTypeCandidate.Remove(randomPieceType);

                else if (randomPieceType == "queen") // only 2 queen allowed
                {
                    if (board.CountPiece(randomPieceType) < 2)
                        return "queen";
                    else
                        pieceTypeCandidate.Remove("queen");
                }

                else if (randomPieceType == "pawn") // only 5 pawns allowed
                {
                    if (board.CountPiece(randomPieceType) < 5)
                        return "pawn";
                    else
                        pieceTypeCandidate.Remove("pawn");
                }

                else // for other kind, 4 allowed
                {
                    if (board.CountPiece(randomPieceType) < 4)
                        return randomPieceType;
                    else
                        pieceTypeCandidate.Remove(randomPieceType);
                }
            }
        }
    }

    private List<string> PieceTypeCandidates() // get candidates of pieceType for current board
    {
        List<string> candidates = new List<string> { "king", "queen", "rook", "bishop", "knight", "pawn" };

        if (board.N >= 4 && board.N <= 10)
        {
            if (board.CountPiece("king") >= 1) candidates.Remove("king");
            if (board.CountPiece("queen") >= 1) candidates.Remove("queen");
            if (board.CountPiece("rook") >= 2) candidates.Remove("rook");
            if (board.CountPiece("bishop") >= 2) candidates.Remove("bishop");
            if (board.CountPiece("knight") >= 2) candidates.Remove("knight");
            if (board.CountPiece("pawn") >= 2) candidates.Remove("pawn");
        }
        else // else if (board.N == 11)
        {
            if (board.CountPiece("king") >= 1) candidates.Remove("king");
            if (board.CountPiece("queen") >= 2) candidates.Remove("queen");
            if (board.CountPiece("rook") >= 4) candidates.Remove("rook");
            if (board.CountPiece("bishop") >= 4) candidates.Remove("bishop");
            if (board.CountPiece("knight") >= 4) candidates.Remove("knight");
            if (board.CountPiece("pawn") >= 5) candidates.Remove("pawn");
        }

        Shuffle(candidates);
        return candidates;
    }

    private void GenerateGame() // with getallpartitions()
    {
        List<List<int>> partitions = GetAllPartitions(board.N); // get partitions
        Shuffle(partitions);

        for (int i = 0; i < partitions.Count; i++) // for every partition :
        {
            board = new Board(board.N);
            List<Move> key = new List<Move>();
            List<Vector2Int> padPos = new List<Vector2Int>();

            for (int j = 0; j < partitions[i].Count; j++) // for every moveCount :
            {
                padPos = GetPadPos(key);
                List<Move> frogPath = GeneratePath(partitions[i][j], padPos);
                key.AddRange(frogPath); // get frog path

                padPos = GetPadPos(frogPath);
                // generate lily pad for all padPos at this step
                // check board
                // if invalid, check which lily pad can act as frog, and change it, and repeat process until valid board
            }
        }
    }

    private List<Move> GeneratePath(int moveCount, List<Vector2Int> frogOrigins)
    {
        List<Move> path = new List<Move>(); // path of frog
        List<string> candidates = PieceTypeCandidates();

        for (int i = 0; i < frogOrigins.Count; i++)
        {
            for (int j = 0; j < candidates.Count; j++)
            {
                Piece frog = new Piece(candidates[j], frogOrigins[i].x, frogOrigins[i].y);
                board.RegisterPiece(frog);

                bool killSwitch = false;
                FindPathRecur(board.Clone(), frog, path, moveCount, ref killSwitch);

                // if correct, return path
                // else, remove frog, remove candidate
            }
        }
        return path; // fail safe
    }

    private void FindPathRecur(Board currentBoard, Piece piece, List<Move> currentPath, int moveCountLeft, ref bool killSwitch)
    {
        if (killSwitch) return;
        else if (moveCountLeft == 0)
        {
            // if frog is toad, check path validity, maybe by using dummy as lily pad
            return;
        }

        List<Move> possibleMoves = currentBoard.GetValidMoves(piece, true);
        if (possibleMoves.Count == 0) return;
        Shuffle(possibleMoves);

        foreach (Move move in possibleMoves)
        {
            Board newBoard = currentBoard.Clone();
            newBoard.ExecuteMove(move);
            currentPath.Add(move);
            Piece movedPiece = newBoard.Grid[move.f.x, move.f.y]!;
            FindPathRecur(newBoard, movedPiece, currentPath, moveCountLeft - 1, ref killSwitch);
            if (killSwitch) return;
            currentPath.RemoveAt(currentPath.Count - 1);
        }
    }

    private List<Vector2Int> GetPadPos(List<Move> path) // get pad positions of a frog path
    {
        List<Vector2Int> padPos = new List<Vector2Int>();

        if (path.Count() == 0) // if first frog, padPos is whole board
        {
            for (int y = 0; y < board.Size; y++)
                for (int x = 0; x < board.Size; x++)
                    padPos.Add(new Vector2Int(x, y));
        }
        else
        {
            for (int i = 0; i < path.Count(); i++)
                padPos.Add(path[i].i);
        }

        Shuffle(padPos);
        return padPos;
    }

    //
    // check unique solution
    public (bool isUniqueSolution, List<Move> key) PuzzleSolver(Board board)
    {
        List<Move> key = new List<Move>(); // answer moveSet
        List<Move> moveSet = new List<Move>(); // multiverse board moveSet
        List<Vector2Int> endingPos = new List<Vector2Int>();
        bool killSwitch = false;

        CheckBoardRecursion(board, key, moveSet, endingPos, ref killSwitch);

        return (endingPos.Count == 1, key);
    }

    private void CheckBoardRecursion(Board board, List<Move> key, List<Move> moveSet, List<Vector2Int> endingPos, ref bool killSwitch)
    {
        if (killSwitch)
            return;

        if (board.CountPieces() == 1)
        {
            Piece lastPiece = board.GetLastPiece(); // need to check ending position

            if (endingPos.Count == 0) // get first key
            {
                endingPos.Add(new Vector2Int(lastPiece.x, lastPiece.y));
                key.AddRange(moveSet);
            }
            else if (lastPiece.x != endingPos[0].x || lastPiece.y != endingPos[0].y) // if different ending, killSwitch on
            {
                endingPos.Add(new Vector2Int(lastPiece.x, lastPiece.y));
                killSwitch = true;
            }
            moveSet.Clear(); // reset moveSet
            return;
        }

        List<Move> allValidMoves = board.GetAllValidMoves();

        if (allValidMoves.Count == 0) return;// dead end

        foreach (Move move in allValidMoves) // try all possible moves
        {
            Board newBoard = board.Clone();
            newBoard.ExecuteMove(move);
            moveSet.Add(move);
            CheckBoardRecursion(newBoard, key, moveSet, endingPos, ref killSwitch);
            if (moveSet.Count > 0) // roll back needed for next move
                moveSet.RemoveAt(moveSet.Count - 1);
        }
    }

    //
    // result

}