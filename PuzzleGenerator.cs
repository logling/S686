public class PuzzleGenerator
{
    int seed = 9;
    Random rand;
    private Board gameBoard;

    public PuzzleGenerator(int inputPieceNum)
    {
        gameBoard = new Board(inputPieceNum);
        rand = new Random(seed);
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

    private void PrintCurrentBoard(Board inputBoard)
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
        Vector2Int generatedPiecePos = new Vector2Int(-1, -1);
        Piece generatedPiece = new Piece("dummy", -1, -1); // initialize generatedPiece
        Piece movingPiece = new Piece("dummy", -1, -1); // initialize movingPiece
        
        for (int moveCount = 1; moveCount < gameBoard.N; moveCount++) // for every move count
        {
            bool isValid1 = false;
            bool isValid2 = false;

            if (moveCount == 1) // if first move, piecePos is everywhere
            {
                for (int y = 0; y < gameBoard.Size; y++)
                    for (int x = 0; x < gameBoard.Size; x++)
                        piecePos.Add(new Vector2Int(x, y));
                
            }

            foreach (string typeCandidate in PieceTypeCandidates(gameBoard)) // for every piece type candidate
            {
                generatedPiece.pieceType = typeCandidate; // render pieceType

                // if this isn't last piece and no possible move, continue
                if (moveCount != gameBoard.N - 1 && gameBoard.GetAllValidMoves(true).Count == 0) continue;

                (isValid2, List<Move> partialSol2) = PuzzleSolver(gameBoard);

                if (isValid2) break; // if valid, move on
            }
        }
        /*
        List<Vector2Int> piecePos = new List<Vector2Int>(); // positions of pieces
        Piece movingPiece = new Piece("dummy", -1, -1); // initialize movingPiece

        for (int moveCount = 1; moveCount < gameBoard.N; moveCount++) // for every move count
        {
            bool isValid1 = false;
            bool isValid2 = false;
            
            if (moveCount == 1) // if first move, piecePos is everywhere
            {
                for (int y = 0; y < gameBoard.Size; y++)
                    for (int x = 0; x < gameBoard.Size; x++)
                        piecePos.Add(new Vector2Int(x, y));
            }

            Piece generatedPiece = new Piece("dummy", -1, -1); // initialize generatedPiece

            Shuffle(piecePos);
            foreach (Vector2Int pos in piecePos.ToList()) // for every piece position
            {
                if (moveCount == 1) // if first move, generate new piece
                {
                    foreach (string typeCandidate in PieceTypeCandidates(gameBoard))
                    {
                        movingPiece = new Piece(typeCandidate, pos.x, pos.y);

                        if (movingPiece.pieceType == "pawn" && gameBoard.GetValidMoves(movingPiece, true).Count == 0)
                        {
                            continue;
                        }
                        else
                        {
                            gameBoard.RegisterPiece(movingPiece);
                            piecePos.Clear(); // update piecePos
                            piecePos.Add(new Vector2Int(pos.x, pos.y));
                            PrintCurrentBoard(gameBoard); // test print
                            break;
                        }
                    }
                }
                else
                    movingPiece = gameBoard.Grid[pos.x, pos.y]!;

                List<Move> validMoves = gameBoard.GetValidMoves(movingPiece, true);
                if (validMoves.Count == 0) continue;

                Shuffle(validMoves);
                foreach (Move move in validMoves) // for every possible move of movingPiece
                {
                    gameBoard.ExecuteMove(move); // execute move

                    generatedPiece.x = move.i.x; // place generatedPiece at initial position
                    generatedPiece.y = move.i.y;
                    gameBoard.RegisterPiece(generatedPiece);

                    (isValid1, List<Move> partialSol1) = PuzzleSolver(gameBoard); // check validity of path

                    if (!isValid1) // if not valid, wrong move, so continue next move
                    {
                        gameBoard.ExecuteMove(move.reverseMove()); // reverse move
                    }
                    else  // if valid, move on to change dummy
                    {
                        piecePos.Add(move.f); // update piecePos
                        break;
                    }
                }

                if (!isValid1) continue;

                foreach (string typeCandidate in PieceTypeCandidates(gameBoard)) // for every piece type candidate
                {
                    generatedPiece.pieceType = typeCandidate; // render pieceType

                    // if this isn't last piece and no possible move, continue
                    if (moveCount != gameBoard.N - 1 && gameBoard.GetAllValidMoves(true).Count == 0) continue;

                    (isValid2, List<Move> partialSol2) = PuzzleSolver(gameBoard);

                    if (isValid2) break; // if valid, move on
                }

                if (isValid2)
                {
                    PrintCurrentBoard(gameBoard); // test print
                    break;
                }
            }
        }*/
    }
    
}