public class PuzzleGenerator
{
    private Random rand = new Random(12345);
    private Board board;
    public string ID = "";
    public int key = 0;
    private Vector2Int origin;

    public PuzzleGenerator(int inputPieceNum)
    {
        board = new Board(inputPieceNum);
    }

    // common
    //
    private static void Shuffle<T>(List<T> inputList)
    {
        Random rand = new Random(12345);

        for (int i = inputList.Count - 1; i > 0; i--) // Fisher-Yates shuffle
        {
            int k = rand.Next(i + 1);
            T value = inputList[k];
            inputList[k] = inputList[i];
            inputList[i] = value;
        }
    }

    public Board GetBoard()
    {
        return board;
    }

    private string GetPieceSymbol(string pieceType)
    {
        return pieceType switch
        {
            "king" => "K",
            "queen" => "Q",
            "rook" => "R",
            "bishop" => "B",
            "knight" => "N",  // 나이트는 N으로 표시
            "pawn" => "P",
            "dummy" => "D",
            _ => "?"
        };
    }


    // partition
    //
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


    // path
    //
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

    private List<string> PieceTypeCandidates(Board inputBoard) // get candidates of pieceType for current board
    {
        List<string> candidates = new List<string> { "king", "queen", "rook", "bishop", "knight", "pawn" };

        if (inputBoard.N >= 4 && inputBoard.N <= 10)
        {
            if (inputBoard.CountPiece("king") >= 1) candidates.Remove("king");
            if (inputBoard.CountPiece("queen") >= 1) candidates.Remove("queen");
            if (inputBoard.CountPiece("rook") >= 2) candidates.Remove("rook");
            if (inputBoard.CountPiece("bishop") >= 2) candidates.Remove("bishop");
            if (inputBoard.CountPiece("knight") >= 2) candidates.Remove("knight");
            if (inputBoard.CountPiece("pawn") >= 2) candidates.Remove("pawn");
        }
        else // else if (board.N == 11)
        {
            if (inputBoard.CountPiece("king") >= 1) candidates.Remove("king");
            if (inputBoard.CountPiece("queen") >= 2) candidates.Remove("queen");
            if (inputBoard.CountPiece("rook") >= 4) candidates.Remove("rook");
            if (inputBoard.CountPiece("bishop") >= 4) candidates.Remove("bishop");
            if (inputBoard.CountPiece("knight") >= 4) candidates.Remove("knight");
            if (inputBoard.CountPiece("pawn") >= 5) candidates.Remove("pawn");
        }

        Shuffle(candidates);
        return candidates;
    }

    public void GenerateGame() // with getallpartitions(). later partition will be chosen from a set list
    {
        List<List<int>> partitions = GetAllPartitions(board.N - 1); // get partitions
        Shuffle(partitions);

        for (int i = 0; i < partitions.Count; i++) // for every partition :
        {
            Console.WriteLine("\n--- 게임 생성 정보 ---"); // test print
            Console.WriteLine($"선택된 파티션: {string.Join(",", partitions[i])}");

            Board multiboard = board.Clone();
            List<Move> allPath = new List<Move>();
            List<Vector2Int> padPos = new List<Vector2Int>();

            for (int j = 0; j < partitions[i].Count; j++) // for every frog :
            {
                padPos = GetPadPos(allPath);
                List<Move> frogPath = GeneratePath(multiboard, partitions[i][j], padPos); // get frog path

                // 개구리 경로 정보 출력 (frogPath 획득 후) test print
                Console.WriteLine($"\n개구리 #{j + 1} 정보:");
                Console.WriteLine($"- 움직임 수: {partitions[i][j]}");
                Console.WriteLine($"- 타입: {multiboard.Grid[frogPath.Last().f.x, frogPath.Last().f.y]!.pieceType}");
                Console.WriteLine($"- 시작 위치: ({frogPath[0].i.x}, {frogPath[0].i.y})");
                Console.WriteLine("- 경로:");
                foreach (Move move in frogPath)
                {
                    Console.WriteLine($"  ({move.i.x}, {move.i.y}) -> ({move.f.x}, {move.f.y})");
                }

                padPos = GetPadPos(frogPath);
                Console.WriteLine("\n릴리패드 정보:"); // test print
                for (int k = 0; k < padPos.Count; k++) // for every current lily pad :
                {
                    List<string> lilyTypeCandidates = PieceTypeCandidates(multiboard);
                    Piece lilyPad = new Piece(lilyTypeCandidates[0], padPos[k].x, padPos[k].y); // generate lily pad with random type for now
                    lilyPad.isLily = true;
                    multiboard.RemovePiece(padPos[k].x, padPos[k].y);
                    multiboard.RegisterPiece(lilyPad);
                    Console.WriteLine($"- 릴리패드 #{k + 1}: 타입={lilyPad.pieceType}, 위치=({lilyPad.x}, {lilyPad.y})"); // test print
                }

                (bool isValid, List<Move> partialSol) = PuzzleSolver(multiboard); // check unique solution

                if (!isValid)  // if invalid, check which lily pad moved, and change it, and repeat process until valid board
                {
                    bool killSwitch = false;
                    Board CorrectedBoard = new Board(multiboard.N);

                    Console.WriteLine($"보드 크기: {multiboard.Size}x{multiboard.Size}"); // test print
                    for (int y = multiboard.Size - 1; y >= 0; y--)
                    {
                        for (int x = 0; x < multiboard.Size; x++)
                        {
                            if (multiboard.Grid[x, y] != null)
                            {
                                string pieceType = multiboard.Grid[x, y]!.pieceType;
                                string symbol = GetPieceSymbol(pieceType);

                                // 역할에 따른 색상 추가
                                if (multiboard.Grid[x, y]!.isFrog)
                                    Console.ForegroundColor = ConsoleColor.Green;
                                else if (multiboard.Grid[x, y]!.isToad)
                                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                                else if (multiboard.Grid[x, y]!.isLily)
                                    Console.ForegroundColor = ConsoleColor.Cyan;
                                else
                                    Console.ForegroundColor = ConsoleColor.White;

                                Console.Write($"[{symbol}]");
                                Console.ResetColor();
                            }
                            else
                            {
                                Console.Write("[ ]");
                            }
                        }
                        Console.WriteLine();
                    }

                    ChangeLilyPadRecur(board, CorrectedBoard, ref killSwitch);
                    if (killSwitch) // if corrected board found
                    {
                        multiboard.Copy(CorrectedBoard);
                        board.Copy(multiboard);
                        return;
                    }
                }
            }
        }
        // at this point error occurred.
    }

    private List<Move> GeneratePath(Board inputBoard, int moveCount, List<Vector2Int> frogOrigins)
    {
        List<Move> path = new List<Move>(); // path of frog

        if (inputBoard.CountPieces() == 0) // if first frog :
        {
            for (int i = 0; i < frogOrigins.Count; i++) // for every possible origin :
            {
                List<string> candidates = PieceTypeCandidates(inputBoard);
                origin = frogOrigins[i];

                for (int j = 0; j < candidates.Count; j++) // for every possible pieceType :
                {
                    path = new List<Move>(); // path of frog
                    List<Move> multiPath = new List<Move>(); // multiverse path of frog

                    Piece frog = new Piece(candidates[j], frogOrigins[i].x, frogOrigins[i].y);
                    frog.isFrog = true;
                    frog.isToad = true;
                    inputBoard.RegisterPiece(frog);

                    bool killSwitch = false;
                    Board updatedBoard = FindPathRecur(inputBoard.Clone(), frog, path, multiPath, moveCount, ref killSwitch);

                    if (path.Count() == moveCount)
                    {
                        // 최종 상태를 inputBoard에 복사
                        inputBoard.Copy(updatedBoard);
                        return path;
                    }
                    else
                    {
                        inputBoard.RemovePiece(frog.x, frog.y);
                    }
                }
            }
        }
        else // if later frogs :
        {
            for (int i = 0; i < frogOrigins.Count; i++) // for every possible origin :
            {
                path = new List<Move>(); // path of frog
                List<Move> multiPath = new List<Move>(); // multiverse path of frog

                Piece frog = inputBoard.Grid[frogOrigins[i].x, frogOrigins[i].y]!;
                frog.isFrog = true;
                if (origin.x == frogOrigins[i].x && origin.y == frogOrigins[i].y) frog.isToad = true;
                frog.isLily = false;

                bool killSwitch = false;
                Board updatedBoard = FindPathRecur(inputBoard.Clone(), frog, path, multiPath, moveCount, ref killSwitch);

                if (path.Count() == moveCount)
                {
                    // 최종 상태를 inputBoard에 복사
                    inputBoard.Copy(updatedBoard);
                    return path;
                }
                else
                {
                    frog.isFrog = false;
                    if (origin.x == frogOrigins[i].x && origin.y == frogOrigins[i].y) frog.isToad = false;
                    frog.isLily = true;
                }
            }
        }

        return path; // error
    }

    private Board FindPathRecur(Board currentBoard, Piece piece, List<Move> solution, List<Move> currentPath, int moveCountLeft, ref bool killSwitch)
    {
        if (moveCountLeft == 0) // {recursion escape condition} if done recursion :
        {
            if (piece.isToad) // if frog is toad, check path
            {
                Board toadBoard = new Board(board.N); // toadBoard contains only toad's path and dummies
                toadBoard.RegisterPiece(piece);
                for (int i = 0; i < currentPath.Count(); i++)
                {
                    Piece dummyLilyPad = new Piece("dummy", currentPath[i].i.x, currentPath[i].i.y);
                    toadBoard.RegisterPiece(dummyLilyPad);
                }

                (bool isValid, List<Move> sol) = PuzzleSolver(toadBoard);
                if (!isValid) return currentBoard; // 유효하지 않은 경로
            }
            solution.AddRange(currentPath); // save frog path
            killSwitch = true; // stop further recursion
            return currentBoard; // 최종 보드 상태 반환
        }

        List<Move> possibleMoves = currentBoard.GetValidMoves(piece, true);
        if (possibleMoves.Count == 0) return currentBoard; // 가능한 움직임 없음
        Shuffle(possibleMoves);

        Board resultBoard = currentBoard; // 결과를 저장할 보드

        foreach (Move move in possibleMoves)
        {
            Board newBoard = currentBoard.Clone();
            newBoard.ExecuteMove(move);
            Piece dummy = new Piece("dummy", move.i.x, move.i.y); // dummy has no move
            newBoard.RegisterPiece(dummy);
            currentPath.Add(move);

            Piece movedPiece = newBoard.Grid[move.f.x, move.f.y]!;
            Board updatedBoard = FindPathRecur(newBoard, movedPiece, solution, currentPath, moveCountLeft - 1, ref killSwitch);

            if (killSwitch)
            {
                resultBoard = updatedBoard; // 성공한 경로의 보드 저장
                return resultBoard;
            }

            if (currentPath.Count() > 0)  // roll back needed for next move
                currentPath.RemoveAt(currentPath.Count - 1);
        }

        return resultBoard; // 최종 보드 상태 반환 (성공하지 못했을 경우 원래 상태)
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
            for (int index = 0; index < path.Count(); index++)
                padPos.Add(path[index].i);
        }

        Shuffle(padPos);
        return padPos;
    }


    // check unique solution
    //
    public (bool isUniqueSolution, List<Move> solution) PuzzleSolver(Board inputBoard)
    {
        List<Move> solution = new List<Move>(); // answer moveSet
        List<Move> moveSet = new List<Move>(); // multiverse board moveSet
        List<Vector2Int> endingPos = new List<Vector2Int>();
        bool killSwitch = false;

        CheckBoardRecur(inputBoard, solution, moveSet, endingPos, ref killSwitch);

        return (endingPos.Count == 1, solution);
    }

    private void CheckBoardRecur(Board currentBoard, List<Move> solution, List<Move> moveSet, List<Vector2Int> endingPos, ref bool killSwitch)
    {
        if (currentBoard.CountPieces() == 1)
        {
            Piece lastPiece = currentBoard.GetLastPiece(); // need to check ending position

            if (endingPos.Count == 0) // get first solution
            {
                endingPos.Add(new Vector2Int(lastPiece.x, lastPiece.y));
                solution.AddRange(moveSet);
            }
            else if (lastPiece.x != endingPos[0].x || lastPiece.y != endingPos[0].y) // if different ending, killSwitch on
            {
                endingPos.Add(new Vector2Int(lastPiece.x, lastPiece.y));
                killSwitch = true;
            }
            return;
        }

        List<Move> allValidMoves = currentBoard.GetAllValidMoves();

        if (allValidMoves.Count == 0) return;// dead end

        foreach (Move move in allValidMoves) // try all possible moves
        {
            Board newBoard = currentBoard.Clone();
            newBoard.ExecuteMove(move);
            moveSet.Add(move);
            CheckBoardRecur(newBoard, solution, moveSet, endingPos, ref killSwitch);
            if (killSwitch) return;
            if (moveSet.Count > 0) // roll back needed for next move
                moveSet.RemoveAt(moveSet.Count - 1);
        }
    }

    private void ChangeLilyPadRecur(Board inputBoard, Board CorrectedBoard, ref bool killSwitch2)
    {
        Board multiBoard = inputBoard.Clone();
        Vector2Int multiLilyPos = new Vector2Int(-1, -1); // multiverse lily position
        Vector2Int wrongLilyPos = new Vector2Int(-1, -1); // answer lily position

        bool killSwitch1 = false;

        FindWrongLilyRecur(inputBoard, ref multiLilyPos, ref wrongLilyPos, ref killSwitch1); // find first moving lily
        List<string> candidates = PieceTypeCandidates(inputBoard);

        for (int i = 0; i < candidates.Count(); i++) // for every pieceType
        {
            multiBoard.Grid[wrongLilyPos.x, wrongLilyPos.y]!.pieceType = candidates[i]; // change lily's pieceType

            (bool isValid, List<Move> partialSol) = PuzzleSolver(multiBoard); // check unique solution
            if (isValid)
            {
                CorrectedBoard.Copy(multiBoard);
                killSwitch2 = true;
                return;
            }
            else
            {
                ChangeLilyPadRecur(multiBoard, CorrectedBoard, ref killSwitch2); // find and change next moving lily
                if (killSwitch2) // if solution found
                    return;
            }
        }
        // at this point, all multiverse failed, and killSwitch2 is false
    }

    private void FindWrongLilyRecur(Board currentBoard, ref Vector2Int susLilyPos, ref Vector2Int resLilyPos, ref bool killSwitch)
    {
        if (killSwitch)
            return;

        if (currentBoard.CountPieces() == 1)
        {
            if (susLilyPos.x != -1) // if lily moved and game ended, wrong lily found
            {
                resLilyPos.x = susLilyPos.x;
                resLilyPos.y = susLilyPos.y;
                killSwitch = true;
                return;
            }
            else return; // if no lily moved and game ended, keep searching
        }

        List<Move> allValidMoves = currentBoard.GetAllValidMoves();

        if (allValidMoves.Count == 0) return; // dead end

        foreach (Move move in allValidMoves) // try all possible moves
        {
            if (currentBoard.Grid[move.i.x, move.i.y]!.isLily && susLilyPos.x == -1) // if lily moves and it's first one, suspect this lily
            {
                susLilyPos.x = move.i.x;
                susLilyPos.y = move.i.y;
            }

            Board newBoard = currentBoard.Clone();
            newBoard.ExecuteMove(move);

            FindWrongLilyRecur(newBoard, ref susLilyPos, ref resLilyPos, ref killSwitch);
            if (killSwitch) return;
            if (susLilyPos.x != -1) // roll back needed for next move
            {
                susLilyPos.x = -1;
                susLilyPos.y = -1;
            }
        }
    }


    // result
    //

}