using ChessChallenge.API;
using System;
using System.Linq;

public class MyBot : IChessBot
{//                       Pawn|Knight|Bishop|Rook|Queen|King
    int[] pieceValue = { 0, 100, 320, 330, 500, 900, 2000 };
    int gameState = 0;
    int bigNum = 999999999;
    int mIntelligence;
    Move bestMove;

    // Set intelligence and use best move
    public Move Think(Board board, Timer timer)
    {
        Move[] legalMoves = board.GetLegalMoves();
        mIntelligence = 5;

        int check = EvaluateBoardState(board, mIntelligence, -bigNum, bigNum, board.IsWhiteToMove ? 1 : -1);

        return bestMove;
    }

    // Evaluate best Move
    int EvaluateBoardState(Board board, int intelligence, int first, int second, int color)
    {
        Move[] legalMoves;

        if (board.IsDraw())
            return 0;

        // If no more legal moves available
        if (intelligence == 0 || (legalMoves = board.GetLegalMoves()).Length == 0)
        {
            int sumBlack = 0;
            int sumWhite = 0;
            int sum = 0;

            if (board.IsInCheckmate())
            {
                return -bigNum + board.PlyCount * -color;
            }

            // Count pieces and add to sum
            for (int i = 0; ++i < 7;)
            {
                sumWhite += board.GetPieceList((PieceType)i, true).Count * pieceValue[i];
                sumBlack += board.GetPieceList((PieceType)i, false).Count * pieceValue[i];

                sum += sumWhite - sumBlack;
            }

            // Checking which sum is the lowest
            if (sumWhite < sumBlack && sumWhite <= 22500)
            {
                gameState = 2;
            }
            else if (sumBlack > sumWhite && sumBlack <= 22500)
            {
                gameState = 2;
            }
            else if (sumWhite < sumBlack && sumWhite <= 23500)
            {
                gameState = 1;
            }
            else if (sumBlack < sumWhite && sumBlack <= 23500)
            {
                gameState = 1;
            }

            // Check where Queen is
            if (gameState == 0)
            {
                Piece whiteQueenSquare = board.GetPiece(new Square("d1"));
                if (!whiteQueenSquare.IsQueen && whiteQueenSquare.IsWhite)
                    sum -= 200 * color;

                Piece blackQueenSquare = board.GetPiece(new Square("d8"));
                if (!blackQueenSquare.IsQueen && !blackQueenSquare.IsWhite)
                    sum -= 200 * color;
            }

            // Check where King is
            if (gameState == 1)
            {
                Piece whiteKingSquare = board.GetPiece(new Square("e1"));
                if (!whiteKingSquare.IsKing)
                    sum -= 200 * color;

                Piece blackKingSquare = board.GetPiece(new Square("e8"));
                if (!blackKingSquare.IsKing)
                    sum -= 200 * color;
            }

            // Check where Pawns are
            if (gameState == 2)
            {
                var playerPawns = board.GetPieceList(PieceType.Pawn, board.IsWhiteToMove);
                var opponentPawns = board.GetPieceList(PieceType.Pawn, !board.IsWhiteToMove);
                if(board.IsWhiteToMove)
                    sum += (2 * playerPawns.Sum(p => p.Square.Rank) * color) + (2 * opponentPawns.Sum(p => 7 - p.Square.Rank) * (color * -1));
                if(!board.IsWhiteToMove)
                    sum += (2 * playerPawns.Sum(p => 7 - p.Square.Rank) * color) + (2 * opponentPawns.Sum(p => p.Square.Rank) * (color * -1));

                var kingSquare = board.GetKingSquare(board.IsWhiteToMove);
                var distance = Math.Min(Math.Abs(kingSquare.File), Math.Abs(kingSquare.File - 7));
                sum -= (10 - distance) * 20 * color;
            }
            return color * sum;
        }

        // Evaluation which move is the best
        int eval = -bigNum;
        foreach (Move move in legalMoves)
        {
            board.MakeMove(move);
            int evaluation = -EvaluateBoardState(board, intelligence - 1, -second, -first, -color);
            board.UndoMove(move);

            if (eval < evaluation)
            {
                eval = evaluation;
                if (intelligence == mIntelligence) bestMove = move;
            }
            first = Math.Max(first, eval);
            if (first >= second) 
                break;
        }
        return eval;
    }
}
