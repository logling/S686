public class PuzzleGenerator
{
    int seed = Environment.TickCount;
    // int seed = 351238515;
    Random rand;
    private Board gameBoard;
    private List<Move> solution = new List<Move>();
    private System.Diagnostics.Stopwatch stopwatch;
    private List<BoardKey> validBoards = new List<BoardKey>();
    private bool puzzleGenerated = false; // 퍼즐 생성 성공 여부
    private Vector2Int origin = new Vector2Int();

    public PuzzleGenerator(int inputPieceNum)
    {
        rand = new Random(seed);
        gameBoard = new Board(inputPieceNum);
        stopwatch = new System.Diagnostics.Stopwatch();
    }

    // produce result
    public void GenerateGame()
    {
        validBoards = new List<BoardKey>();
        puzzleGenerated = false;
        Console.WriteLine($"Seed: {seed}");

        stopwatch.Start();
        ChooseTypeAndMoveRecur(0); // 첫 번째 기물부터 시작
        stopwatch.Stop();
        PrintReport();
    }

    // functions for game and piece
    private void ChooseTypeAndMoveRecur(int currentPieceCount) // choose piece type and execute move
    {
        if (currentPieceCount == gameBoard.N) // 모든 기물을 성공적으로 배치했으면 완료
        {
            puzzleGenerated = true;
            return;
        }

        if (currentPieceCount == 0)
        {
            List<Vector2Int> piecePositions = new List<Vector2Int>(); // get all piece positions
            for (int y = 0; y < gameBoard.Size; y++)
                for (int x = 0; x < gameBoard.Size; x++)
                    piecePositions.Add(new Vector2Int(x, y));
            Shuffle(piecePositions);

            foreach (string pieceType in PieceTypeCandidates(gameBoard))
            {
                List<Vector2Int> positions = new List<Vector2Int>(piecePositions);

                if (pieceType == "pawn") // Pawn은 첫 번째 줄에 놓을 수 없음
                    positions.RemoveAll(pos => pos.y == 0);

                foreach (var pos in positions)
                {
                    origin.x = pos.x; // update origin
                    origin.y = pos.y;
                    Piece piece = new Piece(pieceType, pos.x, pos.y); // generate first piece
                    gameBoard.RegisterPiece(piece);

                    var (isValid, _) = PuzzleSolver(gameBoard); // solve puzzle
                    if (isValid)
                    {
                        BoardKey boardKey = GetBoardKey(gameBoard);
                        validBoards.Add(boardKey); // update validBoards
                        PrintBoard(gameBoard);

                        ChooseTypeAndMoveRecur(1); // start recursion
                        if (puzzleGenerated) return;
                        else gameBoard.RemovePiece(pos.x, pos.y); // 실패했으면 제거하고 다음 시도
                        Console.WriteLine("error at PlaceFirstPiece() : failed at first pos");
                    }
                }
            }
            Console.WriteLine("error at PlaceFirstPiece() : failed at all pos");
        }
        else
        {
            List<Vector2Int> piecePositions = new List<Vector2Int>(); // get all piece positions
            for (int y = 0; y < gameBoard.Size; y++)
                for (int x = 0; x < gameBoard.Size; x++)
                    if (gameBoard.Grid[x, y] != null)
                        piecePositions.Add(new Vector2Int(x, y));

            foreach (var pos in piecePositions) // for every piece position
            {
                Piece movingPiece = gameBoard.Grid[pos.x, pos.y]!; // select movingPiece
                List<Move> validMoves = gameBoard.GetValidMoves(movingPiece, true); // GetValidMoves()
                Shuffle(validMoves);

                foreach (Move move in validMoves) // for every move
                {
                    gameBoard.ExecuteMove(move); // ExecuteMove()

                    foreach (string pieceType in PieceTypeCandidates(gameBoard)) // for every typeCandidate
                    {
                        Piece newPiece = new Piece(pieceType, move.i.x, move.i.y); // get newPiece
                        gameBoard.RegisterPiece(newPiece);

                        if (currentPieceCount < gameBoard.N - 1 && // if not last piece & no valid moves
                            gameBoard.GetAllValidMoves(true).Count == 0)
                        {
                            gameBoard.RemovePiece(move.i.x, move.i.y);
                            continue; // try next typeCandidate
                        }

                        var (isValid, _) = PuzzleSolver(gameBoard);
                        if (isValid)
                        {
                            BoardKey boardKey = GetBoardKey(gameBoard);
                            validBoards.Add(boardKey);
                            solution.Add(move); // solution에 이동 추가
                            PrintBoard(gameBoard);

                            ChooseTypeAndMoveRecur(currentPieceCount + 1); // 다음 기물 배치 시도
                            if (puzzleGenerated) return;

                            solution.RemoveAt(solution.Count - 1); // 실패했으면 solution에서 제거
                        }

                        gameBoard.RemovePiece(move.i.x, move.i.y); // 새 기물 제거
                    }

                    gameBoard.ExecuteMove(move.reverseMove()); // reverseMove()
                }
            }
        }
    }

    // check unique solution
    private (bool isUniqueSolution, List<Move> foundSolution) PuzzleSolver(Board inputBoard)
    {
        List<Move> foundSolution = new List<Move>(); // answer moveSet
        List<Move> moveSet = new List<Move>(); // multiverse board moveSet
        List<Vector2Int> endingPos = new List<Vector2Int>();
        bool killSwitch = false;

        CheckBoardRecur(inputBoard, foundSolution, moveSet, ref killSwitch);
            return (!killSwitch, foundSolution);
    }

    private void CheckBoardRecur(Board currentBoard, List<Move> solution, List<Move> moveSet, ref bool killSwitch)
    {
        BoardKey boardKey = GetBoardKey(currentBoard);
        if (validBoards.Contains(boardKey))
            return;
            
        if (currentBoard.CountPieces() == 1)
        {
            Piece lastPiece = currentBoard.GetLastPiece(); // need to check ending position

            if (lastPiece.x != origin.x || lastPiece.y != origin.y) // if different ending, killSwitch on
                killSwitch = true;
            return;
        }

        List<Move> allValidMoves = currentBoard.GetAllValidMoves();

        if (allValidMoves.Count == 0) return;// dead end

        foreach (Move move in allValidMoves) // try all possible moves
        {
            Board newBoard = currentBoard.Clone();
            newBoard.ExecuteMove(move);
            moveSet.Add(move);
            CheckBoardRecur(newBoard, solution, moveSet, ref killSwitch);
            if (killSwitch) return;
            if (moveSet.Count > 0) // roll back needed for next move
                moveSet.RemoveAt(moveSet.Count - 1);
        }
    }

    // print on terminal
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

    // boardKey
    // 8x8 = 64칸, 각 칸 4비트 = 256비트 필요 → long 4개 사용
    public struct BoardKey : IEquatable<BoardKey>
    {
        public long part1; // 칸 0-15
        public long part2; // 칸 16-31  
        public long part3; // 칸 32-47
        public long part4; // 칸 48-63

        public bool Equals(BoardKey other)
        {
            return part1 == other.part1 && part2 == other.part2 &&
                   part3 == other.part3 && part4 == other.part4;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(part1, part2, part3, part4);
        }
    }

    private BoardKey GetBoardKey(Board board)
    {
        BoardKey key = new BoardKey();

        // 기물 타입을 숫자로 매핑
        Dictionary<string, int> pieceMap = new Dictionary<string, int>
        {
            { "king", 1 }, { "queen", 2 }, { "rook", 3 },
            { "bishop", 4 }, { "knight", 5 }, { "pawn", 6 }
        };

        int position = 0;

        for (int y = 0; y < board.Size; y++)
        {
            for (int x = 0; x < board.Size; x++)
            {
                int pieceValue = 0;
                if (board.Grid[x, y] != null)
                    pieceValue = pieceMap[board.Grid[x, y]!.pieceType];

                // 어느 part에 저장할지 결정
                int partIndex = position / 16;
                int bitOffset = (position % 16) * 4;

                // 해당 part에 4비트 값 저장
                switch (partIndex)
                {
                    case 0:
                        key.part1 |= ((long)pieceValue << bitOffset);
                        break;
                    case 1:
                        key.part2 |= ((long)pieceValue << bitOffset);
                        break;
                    case 2:
                        key.part3 |= ((long)pieceValue << bitOffset);
                        break;
                    case 3:
                        key.part4 |= ((long)pieceValue << bitOffset);
                        break;
                }

                position++;
            }
        }

        return key;
    }

    // miscel
    private void Shuffle<T>(List<T> inputList)
    {
        for (int i = inputList.Count - 1; i > 0; i--) // Fisher-Yates shuffle
        {
            int k = this.rand.Next(i + 1);
            T value = inputList[k];
            inputList[k] = inputList[i];
            inputList[i] = value;
        }
    }

}