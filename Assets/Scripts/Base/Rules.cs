using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityChess
{
    /// <summary>
    /// Contains methods for checking legality of moves and board positions.
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// Determines whether the move obeys all checking rules.
        /// </summary>
        /// <param name="turn">The side of the player whose turn it currently is.</param>
        /// <returns></returns>
        public static bool MoveObeysRules(Board board, Movement move, Side turn)
        {
            return !DoesMoveCauseCheck(board, move, move.Piece.Side) && DoesMoveRemoveCheck(board, move, move.Piece.Side);
        }

        /// <summary>
        /// Checks if the player of the given side has been checkmated.
        /// </summary>
        public static bool IsPlayerCheckmated(Board board, Side side)
        {
            return IsPlayerStalemated(board, side) && IsPlayerInCheck(board, side);
        }

        /// <summary>
        /// Checks if the player of the given side has been stalemated.
        /// </summary>
        public static bool IsPlayerStalemated(Board board, Side side)
        {
            int sumOfValidMoves = 0;

            foreach (Piece p in board.BasePieceList.OfType<Piece>().ToList().FindAll(p => p.Side == side))
            {
                sumOfValidMoves += p.ValidMoves.Count;
            }

            return sumOfValidMoves == 0;
        }

        /// <summary>
        /// Checks if the player of the given side is in check.
        /// </summary>
        public static bool IsPlayerInCheck(Board board, Side side)
        {
            return IsKingInCheck(board, side == Side.Black ? board.BlackKing : board.WhiteKing);
        }

        /// <summary>
        /// Checks if a move made by the player of the given side relieves him of the threat of check, if it exists.
        /// </summary>
        public static bool DoesMoveRemoveCheck(Board board, Movement move, Side side)
        {
            if (!IsPlayerInCheck(board, side)) { return true; }

            Board resultingBoard = new Board(board);
            Piece analogPiece = resultingBoard.BasePieceList.Single(bp => bp is Piece && (bp as Piece).Position.Equals(move.Piece.Position)) as Piece;
            resultingBoard.MovePiece(new Movement(move.End, analogPiece));

            return !IsPlayerInCheck(resultingBoard, side);
        }

        /// <summary>
        /// Checks if a move made by the player of the given side exposes his king to the threat of check.
        /// </summary>
        public static bool DoesMoveCauseCheck(Board board, Movement move, Side side)
        {
            Board resultingBoard = new Board(board);
            Piece analogPiece = resultingBoard.BasePieceList.Single(bp => bp is Piece && (bp as Piece).Position.Equals(move.Piece.Position)) as Piece;
            resultingBoard.MovePiece(new Movement(move.End, analogPiece));

            return IsPlayerInCheck(resultingBoard, side);
        }

        /// <summary>
        /// Checks whether this King instance is under threat of check.
        /// </summary>
        private static bool IsKingInCheck(Board board, King king)
        {
            return IsCheckedRoseDirections(board, king) || IsCheckedKnightDirections(board, king);
        }

        private static bool IsCheckedRoseDirections(Board board, King king)
        {
            Square testSquare = new Square(king.Position);
            List<Square> surroundingSquares = new List<Square>();
            List<Square> pawnAttackingSquares = new List<Square>();

            GenerateSquareLists(surroundingSquares, pawnAttackingSquares, king);

            foreach (int i in new int[] { -1, 0, 1 })
            {
                foreach (int j in new int[] { -1, 0, 1 })
                {
                    if (i == 0 && j == 0) { continue; }
                    testSquare.CopyPosition(king.Position);
                    testSquare.AddVector(i, j);

                    while (testSquare.IsValid())
                    {
                        if (testSquare.IsOccupiedBySide(board, king.Side == Side.White ? Side.Black : Side.White))
                        {
                            Piece piece = board.BasePieceList[testSquare.AsIndex()] as Piece;

                            //diagonal direction
                            if (Math.Abs(i) == Math.Abs(j))
                            {
                                if (piece is Bishop || piece is Queen ||
                                    (((testSquare.Rank == king.Position.Rank + (king.Side == Side.White ? 1 : -1)) &&
                                    (testSquare.File == king.Position.File + 1 || testSquare.File == king.Position.File - 1)) ? piece is Pawn : false))
                                {
                                    return true;
                                }
                                else if (piece is King && surroundingSquares.Contains(piece.Position))
                                {
                                    return true;
                                }
                                else if (piece is Pawn && pawnAttackingSquares.Contains(piece.Position))
                                {
                                    return true;
                                }
                            }
                            //cardinal directions
                            else
                            {
                                if (piece is Rook || piece is Queen)
                                {
                                    return true;
                                }
                                else if (piece is King && surroundingSquares.Contains(piece.Position))
                                {
                                    return true;
                                }
                            }

                            break;
                        }

                        testSquare.AddVector(i, j);
                    }
                }
            }

            return false;
        }

        private static bool IsCheckedKnightDirections(Board board, King king)
        {
            Square testSquare = new Square(king.Position);

            for (int i = -2; i <= 2; i++)
            {
                if (i == 0) { continue; }
                foreach (int j in (Math.Abs(i) == 2 ? new int[] { -1, 1 } : new int[] { -2, 2 }))
                {
                    testSquare.CopyPosition(king.Position);
                    testSquare.AddVector(i, j);

                    if (testSquare.IsValid() && testSquare.IsOccupiedBySide(board, king.Side == Side.White ? Side.Black : Side.White))
                    {
                        Piece piece = board.BasePieceList[testSquare.AsIndex()] as Piece;
                        if (piece is Knight)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static void GenerateSquareLists(List<Square> surroundingSquares, List<Square> pawnAttackingSquares, King king)
        {
            Square testSquare = new Square(king.Position);

            foreach (int file in new int[] { -1, 0, 1 })
            {
                foreach (int rank in new int[] { -1, 0, 1 })
                {
                    if (file == 0 && rank == 0) { continue; }

                    testSquare.CopyPosition(king.Position);
                    testSquare.AddVector(file, rank);

                    if (testSquare.IsValid())
                    {
                        if ((file == 1 || file == -1) && rank == (king.Side == Side.White ? 1 : -1)) { pawnAttackingSquares.Add(new Square(testSquare)); }
                        surroundingSquares.Add(new Square(testSquare));
                    }
                }
            }
        }

    }
}