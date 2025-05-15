public class PuzzleGenerator
{
    // 
    public (bool isUniqueSolution, List<Move> key) PuzzleSolver(Board board)
    {
        List<Move> key = new List<Move>();
        List<Move> moveSet = new List<Move>();
        List<Vector2Int> endingPos = new List<Vector2Int>();
        bool killCode = false;

        PossibleBoard(board, key, moveSet, endingPos, ref killCode);

        return (endingPos.Count == 1, key);
    }

    private void PossibleBoard(Board board, List<Move> key, List<Move> moveSet, List<Vector2Int> endingPos, ref bool killCode)
    {
        if (killCode)
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
                killCode = true;
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
            PossibleBoard(newBoard, key, moveSet, endingPos, ref killCode);
            if (moveSet.Count > 0)
                moveSet.RemoveAt(moveSet.Count - 1);
        }
    }

    private List<string> GetPieceSet()
    {

    }
}
