public class Board
{
    public int Size; // length of board. 4*4 or 6*6 board?
    public int N; // total number of pieces
    public Piece?[,] Grid; // pieces on board


    public Board(int inputPieceNum)
    {
        N = inputPieceNum;

        if (inputPieceNum < 9) // board size is either 4*4 or 6*6
            Size = 4;
        else
            Size = 6; 

        Grid = new Piece?[Size, Size];
    }

    public void RegisterPiece(Piece piece)
    {
        Grid[piece.x, piece.y] = piece;
    }

    public void RemovePiece(int x, int y)
    {
        Grid[x, y] = null;
    }

    public void ClearBoard()
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Grid[i, j] != null)
                    RemovePiece(i, j);
            }
        }
    }

    public bool IsWithinBounds(int x, int y)
    {
        return x >= 0 && x < Size && y >= 0 && y < Size;
    }

    public int CountPieces() // count current number of pieces on board
    {
        int count = 0;
        foreach (var piece in Grid)
        {
            if (piece != null)
                count++;
        }
        return count;
    }

    public int CountPiece(string inputPieceType) // count specific type of piece
    {
        int count = 0;
        foreach (var piece in Grid)
        {
            if (piece != null && piece.pieceType == inputPieceType)
                count++;
        }
        return count;
    }

    public Board Clone()
    {
        Board newBoard = new Board(N);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Grid[i, j] != null)
                {
                    Piece piece = new Piece(Grid[i, j]!.pieceType, Grid[i, j]!.x, Grid[i, j]!.y);
                    newBoard.RegisterPiece(piece);
                }
            }
        }
        return newBoard;
    }
    
    public Piece GetLastPiece()
    {
        Piece piece = new Piece("dummy",-1,-1);

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Grid[i, j] != null)
                {
                    piece = Grid[i, j]!;
                    return piece;
                }
            }
        }
        return piece;
    }

    public void ExecuteMove(Move move) // works when target either exists or not
    {
        Piece piece = Grid[move.i.x, move.i.y]!;
        RemovePiece(move.i.x, move.i.y);
        RemovePiece(move.f.x, move.f.y);
        piece.x = move.f.x;
        piece.y = move.f.y;
        RegisterPiece(piece);
    }

    public List<Move> GetValidMoves(Piece piece, bool backwards = false) // get valid moves of a piece on board.
    {
        List<Move> moves = new List<Move>();

        if (piece.pieceType == "knight")
        {
            int[,] knightMoves = { { 2, 1 }, { 1, 2 }, { -1, 2 }, { -2, 1 },
                                { -2, -1 }, { -1, -2 }, { 1, -2 }, { 2, -1 } };
            for (int i = 0; i < knightMoves.GetLength(0); i++)
            {
                int newX = piece.x + knightMoves[i, 0];
                int newY = piece.y + knightMoves[i, 1];
                if (IsWithinBounds(newX, newY) && (backwards ? Grid[newX, newY] == null : Grid[newX, newY] != null))
                {
                    Move move = new Move(new Vector2Int(piece.x, piece.y), new Vector2Int(newX, newY));
                    moves.Add(move);
                }
            }
        }
        else if (piece.pieceType == "king")
        {
            int[,] kingMoves = { { 1, 0 }, { 1, 1 }, { 1, -1 }, { 0, 1 },
                                { 0, -1 }, { -1, 0 }, { -1, 1 }, { -1, -1 } };
            for (int i = 0; i < kingMoves.GetLength(0); i++)
            {
                int newX = piece.x + kingMoves[i, 0];
                int newY = piece.y + kingMoves[i, 1];
                if (IsWithinBounds(newX, newY) && (backwards ? Grid[newX, newY] == null : Grid[newX, newY] != null))
                {
                    Move move = new Move(new Vector2Int(piece.x, piece.y), new Vector2Int(newX, newY));
                    moves.Add(move);
                }
            }
        }
        else if (piece.pieceType == "pawn")
        {
            int[,] pawnMoves = backwards ? new int[,] { { 1, -1 }, { -1, -1 } } : new int[,] { { 1, 1 }, { -1, 1 } };
            for (int i = 0; i < pawnMoves.GetLength(0); i++)
            {
                int newX = piece.x + pawnMoves[i, 0];
                int newY = piece.y + pawnMoves[i, 1];
                if (IsWithinBounds(newX, newY) && (backwards ? Grid[newX, newY] == null : Grid[newX, newY] != null))
                {
                    Move move = new Move(new Vector2Int(piece.x, piece.y), new Vector2Int(newX, newY));
                    moves.Add(move);
                }
            }
        }
        else if (piece.pieceType == "queen")
        {
            int[,] queenMoves = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 },
                                { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };
            for (int i = 0; i < queenMoves.GetLength(0); i++)
            {
                int step = 1;
                while (step < 10)
                {
                    int newX = piece.x + (queenMoves[i, 0] * step);
                    int newY = piece.y + (queenMoves[i, 1] * step);
                    if (!IsWithinBounds(newX, newY)) break;
                    if (backwards ? Grid[newX, newY] == null : Grid[newX, newY] != null)
                    {
                        Move move = new Move(new Vector2Int(piece.x, piece.y), new Vector2Int(newX, newY));
                        moves.Add(move);
                        if (backwards == false) break;
                        else step++;
                    }
                    else
                    {
                        if (backwards == false) step++;
                        else break;
                    }
                }
            }
        }
        else if (piece.pieceType == "rook")
        {
            int[,] rookMoves = { { 1, 0 }, { -1, 0 }, { 0, 1 }, { 0, -1 } };
            for (int i = 0; i < rookMoves.GetLength(0); i++)
            {
                int step = 1;
                while (step < 10)
                {
                    int newX = piece.x + (rookMoves[i, 0] * step);
                    int newY = piece.y + (rookMoves[i, 1] * step);
                    if (!IsWithinBounds(newX, newY)) break;
                    if (backwards ? Grid[newX, newY] == null : Grid[newX, newY] != null)
                    {
                        Move move = new Move(new Vector2Int(piece.x, piece.y), new Vector2Int(newX, newY));
                        moves.Add(move);
                        if (backwards == false) break;
                        else step++;
                    }
                    else
                    {
                        if (backwards == false) step++;
                        else break;
                    }
                }
            }
        }
        else if (piece.pieceType == "bishop")
        {
            int[,] bishopMoves = { { 1, 1 }, { -1, -1 }, { 1, -1 }, { -1, 1 } };
            for (int i = 0; i < bishopMoves.GetLength(0); i++)
            {
                int step = 1;
                while (step < 10)
                {
                    int newX = piece.x + (bishopMoves[i, 0] * step);
                    int newY = piece.y + (bishopMoves[i, 1] * step);
                    if (!IsWithinBounds(newX, newY)) break;
                    if (backwards ? Grid[newX, newY] == null : Grid[newX, newY] != null)
                    {
                        Move move = new Move(new Vector2Int(piece.x, piece.y), new Vector2Int(newX, newY));
                        moves.Add(move);
                        if (backwards == false) break;
                        else step++;
                    }
                    else
                    {
                        if (backwards == false) step++;
                        else break;
                    }
                }
            }
        }

        return moves;
    }

    public List<Move> GetAllValidMoves() // GetValidMoves() for all pieces on board
    {
        var allMoves = new List<Move>();

        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Grid[i, j] != null)
                {
                    Piece piece = Grid[i, j]!;
                    var moves = GetValidMoves(piece);
                    foreach (var move in moves)
                        allMoves.Add(move);
                }
            }
        }
        return allMoves;
    }
}