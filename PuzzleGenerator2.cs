using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PuzzleGenerator
{
    private System.Random rand = new System.Random();

    private List<Move> moveCandidates = new List<Move>();
    private List<string> pieceTypeCandidates = new List<string> { "king","queen","rook","bishop","knight","pawn" };

    private bool checkOrNo = true;
    private int m=0;

    public void GenerateGame(int pieceNum)
    {
        pieceTypeCandidates = new List<string> { "king","queen","rook","bishop","knight","pawn" };
        Vector2Int startingPos = new Vector2Int();
        Board board = new Board(pieceNum); // generate board
        
        while(true) // generate first piece
        {
            int x = rand.Next(board.Size);
            int y = rand.Next(board.Size);
            Piece newPiece = new Piece(RandomPieceType(board, pieceTypeCandidates),x,y);

            if (board.GetValidMoves(newPiece,true).Count != 0) // first piece could be immovable
            {
                board.RegisterPiece(newPiece);
                startingPos.x = x;
                startingPos.y = y;
                Debug.Log(newPiece.PieceType);
                Debug.Log(startingPos);
                break;
            }
            pieceTypeCandidates = new List<string> { "king","queen","rook","bishop","knight","pawn" };
        }
        
        
        for (int i = 1; i < pieceNum; i++) // repeatedly generate until enough pieces
        {
            bool IsNewPieceGood = false;
            board.GridToList();
            List<Piece> pieceCandidates = board.GridList;
            int j=0; int k=0; int l=0;

            while ( pieceCandidates.Count > 0 && !IsNewPieceGood && j < 5) // select movingPiece
            {
                Piece movingPiece = RandomPiece(pieceCandidates);
                moveCandidates = board.GetValidMoves(movingPiece,true);
                j++;

                while ( moveCandidates.Count > 0 && !IsNewPieceGood && k < 5) // choose move
                {
                    Move chosenMove = RandomMove(moveCandidates);
                    List<string> generateTypeCandidates = pieceTypeCandidates;
                    k++;

                    while ( generateTypeCandidates.Count > 0 && !IsNewPieceGood && l < 5) // generate new piece and execute move
                    {
                        Piece newPiece = new Piece(RandomPieceType(board, generateTypeCandidates),chosenMove.A.x,chosenMove.A.y); // get newPiece
                        l++;
                        Debug.Log(newPiece.PieceType);
                        //Board newBoard = board.Clone();
                        //newBoard.ExecuteMove(chosenMove);
                        //newBoard.RegisterPiece(newPiece);
                        
                        //CheckSolution(newBoard,startingPos); // check board; is multiple answer?
                        Debug.Log(checkOrNo);
                        if (checkOrNo)
                        {
                            IsNewPieceGood = true;
                            board.ExecuteMove(chosenMove); // move moving piece
                            board.RegisterPiece(newPiece); // place newPiece
                        }
                    }
                }
            }
            Debug.Log("i: "+i);
            Debug.Log("j: "+j);
            Debug.Log("k: "+k);
            Debug.Log("l: "+l);
            Debug.Log("m: "+m);
        }

        PlayDirector playDirector = GameObject.FindObjectOfType<PlayDirector>(); 

        foreach (var piece in board.Grid) // Realize each piece
        {
            if (piece != null)
                playDirector.Realize(piece.PieceType,piece.X,piece.Y);
        }
    }

    private Piece RandomPiece(List<Piece> pieces)
    {
        Piece piece = pieces[rand.Next(pieces.Count)];
        pieces.Remove(piece);
        return piece;
    }

    private Move RandomMove(List<Move> moves)
    {
        Move move = moves[rand.Next(moves.Count)];
        moves.Remove(move);
        return move;
    }

    private string RandomPieceType(Board board, List<string> pieceTypes) // get pieceType according to pieceNum rules
    {
        string randomPieceType = pieceTypes[rand.Next(pieceTypes.Count)]; // get random pieceType from candidates list
        board.GridToList();

        while (board.N >= 4 && board.N <= 8)
        {
            if (randomPieceType == "king" || randomPieceType == "queen") // for king and queen, only 1 allowed
                pieceTypes.Remove(randomPieceType);
            if (board.GridList.Count(x => x.PieceType == randomPieceType) < 2) // when less than 2 duplicates, get pieceType
                return randomPieceType;
            else
                pieceTypes.Remove(randomPieceType);
        }
        while (board.N == 9)
        {
            if (board.GridList.Count == 8 && pieceTypes.Contains("king")) // always 1 king
                return "king";
            if (randomPieceType == "king" || randomPieceType == "queen") // for queen, only 1 allowed
                pieceTypes.Remove(randomPieceType);
            if (board.GridList.Count(x => x.PieceType == randomPieceType) < 2)
                return randomPieceType;
            else
                pieceTypes.Remove(randomPieceType);
        }
        while (board.N == 10) // always 1 king, 1 queen, 2x other pieces
        {
            if (randomPieceType == "king" || randomPieceType == "queen") // for king and queen, only 1 allowed
                pieceTypes.Remove(randomPieceType);
            if (board.GridList.Count(x => x.PieceType == randomPieceType) < 2) // when less than 2 duplicates, get pieceType
                return randomPieceType;
            else
                pieceTypes.Remove(randomPieceType);
        }
        while (board.N == 11)
        {
            if (randomPieceType == "king") // only 1 king allowed
            {
                pieceTypes.Remove("king");
                return "king";
            }
            else if (randomPieceType == "queen") // only 2 queen allowed
            {
                if (board.GridList.Count(x => x.PieceType == "queen") < 2)
                    return "queen";
                else
                    pieceTypes.Remove("queen");
            }
            else if (randomPieceType == "pawn") // only 5 pawns allowed
            {
                if (board.GridList.Count(x => x.PieceType == "pawn") < 5)
                    return "pawn";
                else
                    pieceTypes.Remove("pawn");
            }
            else // for other kind, 4 allowed
            {
                if (board.GridList.Count(x => x.PieceType == randomPieceType) < 4)
                    return randomPieceType;
                else
                    pieceTypes.Remove(randomPieceType);
            }
        }

        Debug.Log("error");
        return "pawn";
    }

    private void CheckSolution(Board board, Vector2Int answer)
    {
        m++;
        board.GridToList();
        // when 1 piece left, different ending, return false.
        if (board.GridList.Count == 1 && (board.GridList[0].X != answer.x || board.GridList[0].Y != answer.y) && m > 5)
        {
            checkOrNo = false;
            return;
        }

        List<Move> allValidMoves = board.GetAllValidMoves();
        
        foreach (Move move in allValidMoves)
        {
            Board newBoard = board.Clone();
            newBoard.ExecuteMove(move);
            CheckSolution(newBoard, answer);

            if (!checkOrNo)
                return;
        }

        checkOrNo = true;
    }
}