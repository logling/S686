public class PuzzleGenerator
{
    private int seed = Environment.TickCount;
    // int seed = 412423500;
    private Random rand;
    private Board gameBoard;
    private Vector2Int origin = new Vector2Int(); // ending position
    private List<Move> solution = new List<Move>();
    private bool puzzleGenerated = false; // escape recursion
    private List<Board> multiBoards = new List<Board>(); // used to print board each step
    private HashSet<BoardKey> validBoards = new HashSet<BoardKey>(); // memoization
    private System.Diagnostics.Stopwatch stopwatch; // used to time process

    public PuzzleGenerator(int inputPieceNum)
    {
        rand = new Random(seed);
        gameBoard = new Board(inputPieceNum);
        stopwatch = new System.Diagnostics.Stopwatch();
    }

    // produce result
    public void GenerateGame() // generate puzzle and print report
    {
        validBoards = new HashSet<BoardKey>();
        puzzleGenerated = false;

        stopwatch.Start();
        ChooseTypeAndMoveRecur(0); // start recursion
        stopwatch.Stop();

        PrintReport();
    }

    // functions for game and piece
    private void ChooseTypeAndMoveRecur(int currentPieceCount) // choose piece type and execute move
    {
        if (currentPieceCount == gameBoard.N) // if enough pieces, escape recursion
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

            foreach (string pieceType in PieceTypeCandidates(gameBoard)) // for every typeCandidate,
            {
                List<Vector2Int> positions = new List<Vector2Int>(piecePositions);

                if (pieceType == "pawn") // pawn has restricted origin
                    positions.RemoveAll(pos => pos.y == 0);

                foreach (var pos in positions) // for every position,
                {
                    origin.x = pos.x; // update origin
                    origin.y = pos.y;
                    Piece piece = new Piece(pieceType, pos.x, pos.y); // generate first piece
                    gameBoard.RegisterPiece(piece);

                    var (isValid, _) = PuzzleSolver(gameBoard); // solve puzzle
                    if (isValid) // if valid,
                    {
                        multiBoards.Add(gameBoard.Clone()); // update multiBoards

                        ChooseTypeAndMoveRecur(1); // start next recursion
                        if (puzzleGenerated) return; // if done, escape recursion
                        else gameBoard.RemovePiece(pos.x, pos.y); // if not done, remove piece and try other position
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
            Shuffle(piecePositions);

            foreach (var pos in piecePositions) // for every position,
            {
                Piece movingPiece = gameBoard.Grid[pos.x, pos.y]!; // select movingPiece
                List<Move> validMoves = gameBoard.GetValidMoves(movingPiece, true); // GetValidMoves()
                Shuffle(validMoves);

                foreach (Move move in validMoves) // for every move,
                {
                    gameBoard.ExecuteMove(move); // ExecuteMove()

                    foreach (string pieceType in PieceTypeCandidates(gameBoard)) // for every typeCandidate,
                    {
                        Piece newPiece = new Piece(pieceType, move.i.x, move.i.y); // get newPiece
                        gameBoard.RegisterPiece(newPiece);

                        if (currentPieceCount < gameBoard.N - 1 && // if not last piece & no valid moves,
                            gameBoard.GetAllValidMoves(true).Count == 0)
                        {
                            gameBoard.RemovePiece(move.i.x, move.i.y);
                            continue; // try next typeCandidate
                        }

                        var (isValid, _) = PuzzleSolver(gameBoard); // solve puzzle
                        if (isValid) // if valid,
                        {
                            solution.Add(move); // update solution
                            multiBoards.Add(gameBoard.Clone()); // update multiBoards

                            ChooseTypeAndMoveRecur(currentPieceCount + 1); // start next recursion
                            if (puzzleGenerated) return; // if done, escape

                            solution.RemoveAt(solution.Count - 1); // if failed, roll back solution
                        }

                        gameBoard.RemovePiece(move.i.x, move.i.y); // if all pieceType failed, remove newPiece, need to try next move
                    }

                    gameBoard.ExecuteMove(move.reverseMove()); // if all move failed, reverseMove(), need to try next position
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

    // check unique solution
    private (bool isUniqueSolution, List<Move> foundSolution) PuzzleSolver(Board inputBoard) // check uniqueness of inputBoard, get solution
    {
        List<Move> foundSolution = new List<Move>(); // answer moveSet
        List<Move> moveSet = new List<Move>(); // multiverse board moveSet
        bool killSwitch = false;

        CheckBoardRecur(inputBoard, foundSolution, moveSet, ref killSwitch);
        if (!killSwitch) // if after all multiverse killSwitch remained false,
        {
            BoardKey boardKey = GetBoardKey(inputBoard);
            validBoards.Add(boardKey); // update validBoards
        }

        return (!killSwitch, foundSolution);
    }

    private void CheckBoardRecur(Board currentBoard, List<Move> solution, List<Move> moveSet, ref bool killSwitch)
    {
        BoardKey boardKey = GetBoardKey(currentBoard);
        if (validBoards.Contains(boardKey)) // memoization magic
            return;
            
        if (currentBoard.CountPieces() == 1) // if reached end,
        {
            Piece lastPiece = currentBoard.GetLastPiece(); // need to check ending position

            if (lastPiece.x != origin.x || lastPiece.y != origin.y) // if different ending, invalid, killSwitch on
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
            CheckBoardRecur(newBoard, solution, moveSet, ref killSwitch); // start next recursion
            if (killSwitch) return; // if killSwitch, escape
            if (moveSet.Count > 0)
                moveSet.RemoveAt(moveSet.Count - 1); // roll back needed for next move
        }
    }

    // print on terminal
    private void PrintBoardsInGroup() // print boards in 4 columns
    {
        Console.WriteLine("\n--- printing boards... ---");

        for (int boardGroup = 0; boardGroup < multiBoards.Count; boardGroup += 4)
        {
            List<Board> currentBoards = new List<Board>();
            for (int i = 0; i < 4 && boardGroup + i < multiBoards.Count; i++)
            {
                currentBoards.Add(multiBoards[boardGroup + i]);
            }

            for (int y = gameBoard.Size - 1; y >= 0; y--)
            {
                for (int boardIndex = 0; boardIndex < currentBoards.Count; boardIndex++)
                {
                    Board board = currentBoards[boardIndex];

                    for (int x = 0; x < board.Size; x++)
                    {
                        if (board.Grid[x, y] != null)
                        {
                            string symbol = board.Grid[x, y]!.pieceType switch
                            {
                                "king" => "K",
                                "queen" => "Q",
                                "rook" => "R",
                                "bishop" => "B",
                                "knight" => "N",
                                "pawn" => "P",
                                _ => "?"
                            };
                            Console.Write($"[{symbol}]");
                        }
                        else Console.Write("[ ]");
                    }

                    if (boardIndex < currentBoards.Count - 1) Console.Write("   ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    private void PrintReport() // print boards, seed, time, solution
    {
        PrintBoardsInGroup();
        Console.WriteLine($"seed : {seed}");
        Console.WriteLine($"time : {stopwatch.Elapsed.TotalSeconds:F2}s");
        Console.WriteLine("solution : ");
        for (int i = solution.Count - 1; i >= 0; i--)
        {
            Move move = solution[i];
            Console.WriteLine($"  {solution.Count - i}. ({move.f.x},{move.f.y}) → ({move.i.x},{move.i.y})");
        }

        // PrintValidBoards();
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

    private void PrintValidBoards()
    {
        Console.WriteLine("\n=== Valid Boards ===");
        List<BoardKey> validBoardsList = validBoards.ToList();
        for (int boardGroup = 0; boardGroup < validBoards.Count; boardGroup += 4)
        {
            List<Board> currentBoards = new List<Board>();
            for (int i = 0; i < 4 && boardGroup + i < validBoards.Count; i++)
            {
                currentBoards.Add(BoardKeyToBoard(validBoardsList[boardGroup + i]));
            }

            for (int y = gameBoard.Size - 1; y >= 0; y--)
            {
                for (int boardIndex = 0; boardIndex < currentBoards.Count; boardIndex++)
                {
                    Board board = currentBoards[boardIndex];

                    for (int x = 0; x < board.Size; x++)
                    {
                        if (board.Grid[x, y] != null)
                        {
                            string symbol = board.Grid[x, y]!.pieceType switch
                            {
                                "king" => "K",
                                "queen" => "Q",
                                "rook" => "R",
                                "bishop" => "B",
                                "knight" => "N",
                                "pawn" => "P",
                                _ => "?"
                            };
                            Console.Write($"[{symbol}]");
                        }
                        else Console.Write("[ ]");
                    }

                    if (boardIndex < currentBoards.Count - 1) Console.Write("   ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
    }

    // boardKey
    private struct BoardKey : IEquatable<BoardKey> // describes board state
    {
        public long part1; // square 0-15
        public long part2; // square 16-31  
        public long part3; // square 32-47
        public long part4; // square 48-63

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

    private Board BoardKeyToBoard(BoardKey key) // transform BoardKey to Board
    {
        Board board = new Board(gameBoard.N);

        Dictionary<int, string> pieceMap = new Dictionary<int, string>
    {
        { 1, "king" }, { 2, "queen" }, { 3, "rook" },
        { 4, "bishop" }, { 5, "knight" }, { 6, "pawn" }
    };

        int position = 0;
        for (int y = 0; y < board.Size; y++)
        {
            for (int x = 0; x < board.Size; x++)
            {
                int partIndex = position / 16;
                int bitOffset = (position % 16) * 4;

                long part = partIndex switch
                {
                    0 => key.part1,
                    1 => key.part2,
                    2 => key.part3,
                    3 => key.part4,
                    _ => 0
                };

                int pieceValue = (int)((part >> bitOffset) & 0xF);

                if (pieceValue > 0)
                {
                    Piece piece = new Piece(pieceMap[pieceValue], x, y);
                    board.RegisterPiece(piece);
                }

                position++;
            }
        }

        return board;
    }

    private BoardKey GetBoardKey(Board board) // transform Board to BoardKey
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
    private void Shuffle<T>(List<T> inputList) // shuffle any List
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