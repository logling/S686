public class PuzzleGenerator
{
    Random rand = new Random();
    //
    // partition
    public List<List<int>> GetAllPartitions(int n)
    {
        List<List<int>> result = new List<List<int>>();
        PartitionRecursion(n, n, new List<int>(), result);
        SortPartitions(result);
        PrintPartitions(result);
        return result;
    }

    private void PartitionRecursion(int n, int max, List<int> current, List<List<int>> result)
    {
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

    public static void Shuffle<T>(List<T> inputList)
    {
        Random rand = new Random();

        for (int i = inputList.Count - 1; i > 0; i--)
        {
            int k = rand.Next(i + 1);
            T value = inputList[k];
            inputList[k] = inputList[i];
            inputList[i] = value;
        }
    }

    //
    // path
    private void FirstLoop()
    {

    }

    private Piece TakeRandomPiece(List<Piece> pieces)
    {
        Piece piece = pieces[rand.Next(pieces.Count)];
        pieces.Remove(piece);
        return piece;
    }

    private Move TakeRandomMove(List<Move> moves)
    {
        Move move = moves[rand.Next(moves.Count)];
        moves.Remove(move);
        return move;
    }

    private string RandomPieceType(Board board, List<string> pieceTypes) // get random pieceType according to pieceNum rules
    {
        string randomPieceType;

        if (board.N >= 4 && board.N <= 8)
        {
            while (true)
            {
                randomPieceType = pieceTypes[rand.Next(pieceTypes.Count)]; // get random pieceType from candidates list

                if (randomPieceType == "king" || randomPieceType == "queen") // for king and queen, only 1 allowed
                    pieceTypes.Remove(randomPieceType);
                if (board.CountPiece(randomPieceType) < 2) // when less than 2 duplicates, get pieceType
                    return randomPieceType;
                else
                    pieceTypes.Remove(randomPieceType);
            }
        }
        else if (board.N == 9)
        {
            while (true)
            {
                randomPieceType = pieceTypes[rand.Next(pieceTypes.Count)]; // get random pieceType from candidates list

                if (board.CountPieces() == 8 && pieceTypes.Contains("king")) // always 1 king
                    return "king";
                if (randomPieceType == "king" || randomPieceType == "queen") // for queen, only 1 allowed
                    pieceTypes.Remove(randomPieceType);
                if (board.CountPiece(randomPieceType) < 2)
                    return randomPieceType;
                else
                    pieceTypes.Remove(randomPieceType);
            }
        }
        else if (board.N == 10) // always 1 king, 1 queen, 2x other pieces
        {
            while (true)
            {
                randomPieceType = pieceTypes[rand.Next(pieceTypes.Count)]; // get random pieceType from candidates list

                if (board.CountPieces() == 8 && pieceTypes.Contains("king")) // always 1 king
                    return "king";
                if (randomPieceType == "king" || randomPieceType == "queen") // for queen, only 1 allowed
                    pieceTypes.Remove(randomPieceType);
                if (board.CountPiece(randomPieceType) < 2)
                    return randomPieceType;
                else
                    pieceTypes.Remove(randomPieceType);
            }
        }
        else // else if (board.N == 11)
        {
            while (true)
            {
                randomPieceType = pieceTypes[rand.Next(pieceTypes.Count)]; // get random pieceType from candidates list

                if (randomPieceType == "king") // only 1 king allowed
                {
                    pieceTypes.Remove("king");
                    return "king";
                }
                else if (randomPieceType == "queen") // only 2 queen allowed
                {
                    if (board.CountPiece(randomPieceType) < 2)
                        return "queen";
                    else
                        pieceTypes.Remove("queen");
                }
                else if (randomPieceType == "pawn") // only 5 pawns allowed
                {
                    if (board.CountPiece(randomPieceType) < 5)
                        return "pawn";
                    else
                        pieceTypes.Remove("pawn");
                }
                else // for other kind, 4 allowed
                {
                    if (board.CountPiece(randomPieceType) < 4)
                        return randomPieceType;
                    else
                        pieceTypes.Remove(randomPieceType);
                }
            }
        }
    }

    private List<Move> RandomPath(Board board, string inputPieceType, int moveN, bool isFirst = false)
    {
        List<Move> moveCandidates = new List<Move>();
        List<Move> moveBanned = new List<Move>();

        if (isFirst)
        {
            while (true)
            {
                int x = rand.Next(board.Size);
                int y = rand.Next(board.Size);
                Piece movingPiece = new Piece(inputPieceType, x, y);

                for (int i = 0; i < moveN;)
                {
                    moveCandidates = board.GetValidMoves(movingPiece, true);
                    if (moveCandidates.Count() == 0) break;
                    Move chosenMove = TakeRandomMove(moveCandidates);
                }
                break;
            }
        }
        else
        {
            while (true)
            {

                break;
            }
        }
    }

    private List<Move> GetValidPath(Board board, Piece piece, List<Move> givenMove, int moveN)
    {
        List<Move> path = new List<Move>();
        bool killSwitch = false;

        pathRecur(board, piece, path, givenMove, 0, moveN, ref killSwitch);

        return path;
    }
    private void pathRecur(Board board, Piece piece, List<Move> path, List<Move> givenMove, int moveCount, int moveN, ref bool killSwitch)
    {
        if (killSwitch) return;
        if (moveCount == moveN) return;

        List<Move> currentValidMoves = board.Get
        foreach (Move move in givenMove)
        {
            Board newBoard = board.Clone();
            newBoard.ExecuteMove(move);
            moveCount++;
            path.Add(move);
            pathRecur(newBoard, piece, )


        }
    }
    //
    // check unique solution
    public (bool isUniqueSolution, List<Move> key) PuzzleSolver(Board board)
    {
        List<Move> key = new List<Move>();
        List<Move> moveSet = new List<Move>();
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
            Piece lastPiece = board.GetLastPiece();

            if (endingPos.Count == 0)
            {
                endingPos.Add(new Vector2Int(lastPiece.x, lastPiece.y));
                key.AddRange(moveSet);
            }
            else if (lastPiece.x != endingPos[0].x || lastPiece.y != endingPos[0].y)
            {
                endingPos.Add(new Vector2Int(lastPiece.x, lastPiece.y));
                killSwitch = true;
            }
            moveSet.Clear();
            return;
        }

        List<Move> allValidMoves = board.GetAllValidMoves();
        if (allValidMoves.Count == 0)
            return;

        foreach (Move move in allValidMoves)
        {
            Board newBoard = board.Clone();
            newBoard.ExecuteMove(move);
            moveSet.Add(move);
            CheckBoardRecursion(newBoard, key, moveSet, endingPos, ref killSwitch);
            if (moveSet.Count > 0)
                moveSet.RemoveAt(moveSet.Count - 1);
        }
    }
}