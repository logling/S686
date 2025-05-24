public class PuzzleGenerator
{
    int seed = Environment.TickCount;
    // int seed = 223814843;
    Random rand;
    private Board gameBoard;
    private List<Move> solution = new List<Move>();
    private System.Diagnostics.Stopwatch stopwatch;

    public PuzzleGenerator(int inputPieceNum)
    {
        rand = new Random(seed);
        gameBoard = new Board(inputPieceNum);
        stopwatch = new System.Diagnostics.Stopwatch();
    }

    // miscel
    //
    private void Shuffle<T>(List<T> inputList)
    {
        Random rand = new Random(seed);

        for (int i = inputList.Count - 1; i > 0; i--) // Fisher-Yates shuffle
        {
            int k = rand.Next(i + 1);
            T value = inputList[k];
            inputList[k] = inputList[i];
            inputList[i] = value;
        }
    }

    public Board getGameBoard()
    {
        return gameBoard;
    }

    private void PrintBoard(Board inputBoard)
    {
        Console.WriteLine("\n--- 현재 보드 ---\n");

        for (int y = inputBoard.Size - 1; y >= 0; y--)
        {
            // 행 인덱스 표시
            Console.Write($"{y} ");

            for (int x = 0; x < inputBoard.Size; x++)
            {
                if (inputBoard.Grid[x, y] != null)
                {
                    string symbol = inputBoard.Grid[x, y]!.pieceType;

                    symbol = symbol switch
                    {
                        "king" => "K",
                        "queen" => "Q",
                        "rook" => "R",
                        "bishop" => "B",
                        "knight" => "N",
                        "pawn" => "P",
                        "dummy" => "D",
                        _ => "?"
                    };

                    Console.Write($"[{symbol}]");
                    Console.ResetColor();
                }
                else Console.Write("[ ]");
            }
            Console.WriteLine();
        }

        // 열 인덱스 표시
        Console.Write("  ");
        for (int x = 0; x < inputBoard.Size; x++)
        {
            Console.Write($" {x} ");
        }
        Console.WriteLine();
    }

    private void PrintReport()
    {
        Console.WriteLine($"seed : {seed}");
        Console.WriteLine($"time : {stopwatch.Elapsed.TotalSeconds:F2}s");

        Console.WriteLine("solution : ");
        for (int i = solution.Count - 1; i >= 0; i--)
        {
            Move move = solution[i];
            Console.WriteLine($"  {solution.Count - i}. ({move.f.x},{move.f.y}) → ({move.i.x},{move.i.y})");
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

    // produce result
    //
    public void GenerateGame()
{
    Console.WriteLine($"Seed: {seed}");
    stopwatch.Start();
    
    // 첫 번째 기물부터 시작
    if (GenerateGameRecursive(0))
    {
        stopwatch.Stop();
        PrintBoard(gameBoard);
        PrintReport();
    }
    else
    {
        stopwatch.Stop();
        Console.WriteLine("퍼즐 생성 실패!");
    }
}

    private bool GenerateGameRecursive(int currentPieceCount)
    {
        // 모든 기물을 성공적으로 배치했으면 완료
        if (currentPieceCount == gameBoard.N)
        {
            // 마지막 검증
            var (isValid, _) = PuzzleSolver(gameBoard);
            return isValid;
        }

        // 현재 상태 출력 (디버깅용)
        if (currentPieceCount > 0)
            PrintBoard(gameBoard);

        // 첫 번째 기물 배치
        if (currentPieceCount == 0)
        {
            return PlaceFirstPiece();
        }

        // 두 번째 이후 기물들 배치
        return PlaceNextPiece(currentPieceCount);
    }

    private bool PlaceFirstPiece()
    {
        List<Vector2Int> allPositions = new List<Vector2Int>();
        for (int y = 0; y < gameBoard.Size; y++)
            for (int x = 0; x < gameBoard.Size; x++)
                allPositions.Add(new Vector2Int(x, y));
        Shuffle(allPositions);

        foreach (string pieceType in PieceTypeCandidates(gameBoard))
        {
            List<Vector2Int> positions = new List<Vector2Int>(allPositions);

            // Pawn은 첫 번째 줄에 놓을 수 없음
            if (pieceType == "pawn")
                positions.RemoveAll(pos => pos.y == 0);

            foreach (var pos in positions)
            {
                Piece piece = new Piece(pieceType, pos.x, pos.y);
                gameBoard.RegisterPiece(piece);

                var (isValid, _) = PuzzleSolver(gameBoard);
                if (isValid)
                {
                    // 다음 기물 배치 시도
                    if (GenerateGameRecursive(1))
                        return true;
                }

                // 실패했으면 제거하고 다음 시도
                gameBoard.RemovePiece(pos.x, pos.y);
            }
        }

        return false;
    }

    private bool PlaceNextPiece(int currentPieceCount)
    {
        // 현재 보드의 모든 기물 위치 수집
        List<Vector2Int> piecePositions = new List<Vector2Int>();
        for (int y = 0; y < gameBoard.Size; y++)
            for (int x = 0; x < gameBoard.Size; x++)
                if (gameBoard.Grid[x, y] != null)
                    piecePositions.Add(new Vector2Int(x, y));

        // 각 기물을 움직여보기
        foreach (var pos in piecePositions)
        {
            Piece movingPiece = gameBoard.Grid[pos.x, pos.y]!;
            List<Move> validMoves = gameBoard.GetValidMoves(movingPiece, true);
            Shuffle(validMoves);

            foreach (Move move in validMoves)
            {
                // 이동 실행
                gameBoard.ExecuteMove(move);

                // 빈 자리에 새 기물 시도
                foreach (string pieceType in PieceTypeCandidates(gameBoard))
                {
                    Piece newPiece = new Piece(pieceType, move.i.x, move.i.y);
                    gameBoard.RegisterPiece(newPiece);

                    // 마지막 기물이 아니면 backwards 이동 가능해야 함
                    if (currentPieceCount < gameBoard.N - 1 &&
                        gameBoard.GetAllValidMoves(true).Count == 0)
                    {
                        gameBoard.RemovePiece(move.i.x, move.i.y);
                        continue;
                    }

                    var (isValid, _) = PuzzleSolver(gameBoard);
                    if (isValid)
                    {
                        // solution에 이동 추가
                        solution.Add(move);

                        // 다음 기물 배치 시도
                        if (GenerateGameRecursive(currentPieceCount + 1))
                            return true;

                        // 실패했으면 solution에서 제거
                        solution.RemoveAt(solution.Count - 1);
                    }

                    // 새 기물 제거
                    gameBoard.RemovePiece(move.i.x, move.i.y);
                }

                // 이동 되돌리기
                gameBoard.ExecuteMove(move.reverseMove());
            }
        }

        return false;
    }
}