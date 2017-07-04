using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public Pawn(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Pawn(Pawn pawnCopy) : base(pawnCopy) { }

        public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            ValidMoves.Clear();

            CheckForwardMovingSquares(board, turn);
            CheckAttackingSquares(board, turn);
            CheckEnPassantCaptures(board, previousMoves, turn);
        }

        private void CheckForwardMovingSquares(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            testSquare.AddVector(0, this.Side == Side.White ? 1 : -1);
            if (!testSquare.IsOccupied(board) && CheckRules.ObeysCheckRules(board, testMove, turn))
            {
                if (this.Position.Rank == (this.Side == Side.White ? 7 : 2))
                {
                    // TODO call to gui method which gets user promotion piece choice
                    // ElectedPiece userElection = GUI.getElectionChoice();

                    //for now will default to Queen election
                    ElectedPiece userElection = ElectedPiece.Queen;
                    ValidMoves.Add(new PromotionMove(new Square(testSquare), this, userElection));
                }
                else
                {
                    ValidMoves.Add(new Movement(testMove));

                    if (!this.HasMoved)
                    {
                        testSquare.AddVector(0, this.Side == Side.White ? 1 : -1);
                        if (!testSquare.IsOccupied(board) && CheckRules.ObeysCheckRules(board, testMove, turn))
                        {
                            ValidMoves.Add(new Movement(testMove));
                        }
                    }
                }
            }
        }

        private void CheckAttackingSquares(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            foreach (int i in new int[] { -1, 1} )
            {
                testSquare.CopyPosition(this.Position);
                testSquare.AddVector(i, this.Side == Side.White ? 1 : -1);

                if (testSquare.IsValid() && testSquare.IsOccupiedByFriendly(board, this.Side == Side.White ? Side.Black : Side.White) && CheckRules.ObeysCheckRules(board, testMove, turn))
                {
                    if (this.Position.Rank == (this.Side == Side.White ? 7 : 2))
                    {
                        // TODO call to gui method which gets user promotion piece choice
                        // ElectedPiece userElection = GUI.getElectionChoice();

                        //for now will default to Queen election
                        ElectedPiece userElection = ElectedPiece.Queen;
                        ValidMoves.Add(new PromotionMove(new Square(testSquare), this, userElection));
                    }
                    else
                    {
                        ValidMoves.Add(new Movement(testMove));
                    }
                }
            }
        }

        private void CheckEnPassantCaptures(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            if (this.Side == Side.White ? this.Position.Rank == 5 : this.Position.Rank == 4)
            {
                Square testSquare = new Square(this.Position);

                foreach (int i in new int[] { -1, 1 })
                {
                    testSquare.CopyPosition(this.Position);
                    testSquare.AddVector(i, 0);

                    if (testSquare.IsValid() && board.BasePieceList[testSquare.AsIndex()] is Pawn && (board.BasePieceList[testSquare.AsIndex()] as Pawn).Side != this.Side)
                    {
                        Pawn enemyLateralPawn = board.BasePieceList[testSquare.AsIndex()] as Pawn;
                        Piece pieceLastMoved = previousMoves.Last.Value.Piece;

                        if (pieceLastMoved is Pawn && (pieceLastMoved as Pawn) == enemyLateralPawn && pieceLastMoved.Position.Rank == (pieceLastMoved.Side == Side.White ? 2 : 7))
                        {
                            EnPassantMove testMove = new EnPassantMove(new Square(testSquare.Rank + (this.Side == Side.White ? 1 : -1)), this, enemyLateralPawn);

                            if (CheckRules.ObeysCheckRules(board, testMove, turn))
                            {
                                ValidMoves.Add(new EnPassantMove(new Square(testSquare.Rank + (this.Side == Side.White ? 1 : -1)), this, enemyLateralPawn));
                            }
                        }
                    }
                }
            }
        }

        public override Piece Clone()
        {
            return new Pawn(this);
        }
    }
}