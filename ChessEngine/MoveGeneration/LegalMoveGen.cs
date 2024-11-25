using ChessEngine.Position;
using static ChessEngine.Position.Move;

namespace ChessEngine.MoveGeneration
{
    public static class LegalMoveGen
    {
        public static MoveList Generate(Position.Position position, Side side, bool onlyCaptures = false)
        {
            MoveList moves = new MoveList();
            ulong pawnsLeftCaptures = PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(position.Pieces, (byte)side, false);
            ulong pawnsRightCaptures = PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(position.Pieces, (byte)side, false);

            sbyte pawnsLeftCaptureIndex;
            sbyte pawnsRightCaptureIndex;

            if (side == Side.White)
            {
                pawnsLeftCaptureIndex = -7;
                pawnsRightCaptureIndex = -9;
            }
            else
            {
                pawnsLeftCaptureIndex = 9;
                pawnsRightCaptureIndex = 7;
            }

            PawnsMaskToMoves(position.Pieces, pawnsLeftCaptures, side, pawnsLeftCaptureIndex, true, Move.FLAG.Default, moves);
            PawnsMaskToMoves(position.Pieces, pawnsRightCaptures, side, pawnsRightCaptureIndex, true, Move.FLAG.Default, moves);

            if (!onlyCaptures)
            {
                ulong pawnsDefault = PseudoLegalMoveMaskGen.GeneratePawnsDefaultMask(position.Pieces, (byte)side);
                ulong pawnsLong = PseudoLegalMoveMaskGen.GeneratePawnsLongMask(position.Pieces, (byte)side);

                sbyte pawnDefaultIndex;
                sbyte pawnLongIndex;
                if (side == Side.White)
                {
                    pawnDefaultIndex = -8;
                    pawnLongIndex = -16;
                }
                else
                {
                    pawnDefaultIndex = 8;
                    pawnLongIndex = 16;
                }
                PawnsMaskToMoves(position.Pieces, pawnsDefault, side, pawnDefaultIndex, false, Move.FLAG.Default, moves);
                PawnsMaskToMoves(position.Pieces, pawnsLong, side, pawnLongIndex, false, Move.FLAG.PawnLongMove, moves);
            }

            ulong allKnights = position.Pieces.GetPieceBitboard((byte)side, (byte)Piece.Knight);
            ulong allBishops = position.Pieces.GetPieceBitboard((byte)side, (byte)Piece.Bishop);
            ulong allRooks = position.Pieces.GetPieceBitboard((byte)side, (byte)Piece.Rook);
            ulong allQueens = position.Pieces.GetPieceBitboard((byte)side, (byte)Piece.Queen);
            byte attackerP;
            ulong mask;
            while (allKnights != 0)
            {
                attackerP = BOp.Bsf(allKnights);
                allKnights = BOp.Set0(allKnights, attackerP);
                mask = PseudoLegalMoveMaskGen.GenerateKnightMask(position.Pieces, attackerP, side, onlyCaptures);
                PieceMaskToMoves(position.Pieces, mask, attackerP, Piece.Knight, side, moves);
            }
            while (allBishops != 0)
            {
                attackerP = BOp.Bsf(allBishops);
                allBishops = BOp.Set0(allBishops, attackerP);
                mask = PseudoLegalMoveMaskGen.GenerateBishopMask(position.Pieces, attackerP, (byte)side, onlyCaptures);
                PieceMaskToMoves(position.Pieces, mask, attackerP, Piece.Bishop, side, moves);
            }
            while (allRooks != 0)
            {
                attackerP = BOp.Bsf(allRooks);
                allRooks = BOp.Set0(allRooks, attackerP);
                mask = PseudoLegalMoveMaskGen.GenerateRookMask(position.Pieces, attackerP, (byte)side, onlyCaptures);
                PieceMaskToMoves(position.Pieces, mask, attackerP, Piece.Rook, side, moves);
            }
            while (allQueens != 0)
            {
                attackerP = BOp.Bsf(allQueens);
                allQueens = BOp.Set0(allQueens, attackerP);
                mask = PseudoLegalMoveMaskGen.GenerateQueenMask(position.Pieces, attackerP, (byte)side, onlyCaptures);
                PieceMaskToMoves(position.Pieces, mask, attackerP, Piece.Queen, side, moves);
            }
            attackerP = BOp.Bsf(position.Pieces.GetPieceBitboard((byte)side, (byte)Piece.King));
            mask = PseudoLegalMoveMaskGen.GenerateKingMask(position.Pieces, attackerP, side, onlyCaptures);
            PieceMaskToMoves(position.Pieces, mask, attackerP, Piece.King, side, moves);

            AddEnPassantCaptures(position.Pieces, side, position.EnPassant, moves);
            if (!onlyCaptures)
            {
                if (side == Side.White)
                {
                   AddCastlingMoves(position.Pieces, Side.White, position.WLCastling, position.WSCastling, moves);
                }
                else
                {
                    AddCastlingMoves(position.Pieces, Side.Black, position.BLCastling, position.BSCastling, moves);
                }
            }

            return moves;
        }

        private static void PieceMaskToMoves(Pieces pieces, ulong mask, byte attackerP, Piece attackerType, Side attackerSide, MoveList moves)
        {
            while (mask != 0)
            {
                byte defenderP = BOp.Bsf(mask);
                mask = BOp.Set0(mask, defenderP);

                Piece defenderType = Piece.None;
                for (byte i = 0; i < 6; i++)
                {
                    if (BOp.GetBit(pieces.GetPieceBitboard((byte)Pieces.Inverse(attackerSide), i), defenderP))
                    {
                        defenderType = i.Convert<Piece>();
                        break;
                    }
                }

                Move move = new Move(attackerP, defenderP, attackerType, attackerSide, defenderType, Pieces.Inverse(attackerSide));

                if (IsLegal(pieces.Copy(), move))
                    moves.Push(move);
            }
        }

        private static void PawnsMaskToMoves(Pieces pieces, ulong mask, Side attackerSide, sbyte attackerIndex, bool checkDefender, FLAG flag, MoveList moves)// THIS METHOD RESULTING IN from needed to be +1, to is fine
        {
            Piece defenderType = Piece.None;

            while (mask != 0)
            {
                byte defenderP = BOp.Bsf(mask);
                mask = BOp.Set0(mask, defenderP);

                if (checkDefender)
                {
                    defenderType = Piece.None;
                    for (byte i = 0; i < 6; i++)
                    {
                        if (BOp.GetBit(pieces.GetPieceBitboard((byte)Pieces.Inverse(attackerSide), i), defenderP))
                        {
                            defenderType = i.Convert<Piece>();
                            break;
                        }
                    }
                }

                Move move = new Move((byte)(defenderP + attackerIndex), defenderP, Piece.Pawn, attackerSide, defenderType, Pieces.Inverse(attackerSide), flag);

                if (IsLegal(pieces.Copy(), move))
                {
                    if (defenderP < 8 || defenderP > 55)
                    {
                        moves.Push(new Move((byte)(defenderP + attackerIndex), defenderP, 0, attackerSide, defenderType, Pieces.Inverse(attackerSide), Move.FLAG.PromoteToKnight));
                        moves.Push(new Move((byte)(defenderP + attackerIndex), defenderP, 0, attackerSide, defenderType, Pieces.Inverse(attackerSide), Move.FLAG.PromoteToBishop));
                        moves.Push(new Move((byte)(defenderP + attackerIndex), defenderP, 0, attackerSide, defenderType, Pieces.Inverse(attackerSide), Move.FLAG.PromoteToRook));
                        moves.Push(new Move((byte)(defenderP + attackerIndex), defenderP, 0, attackerSide, defenderType, Pieces.Inverse(attackerSide), Move.FLAG.PromoteToQueen));
                    }
                    else
                        moves.Push(move);
                }
            }
        }

        private static bool IsLegal(Pieces pieces, Move move)
        {
            pieces.SetPieceBitboard((byte)move.AttackerSide, (byte)move.AttackerType, BOp.Set0(pieces.GetPieceBitboard((byte)move.AttackerSide, (byte)move.AttackerType), move.From));
            pieces.SetPieceBitboard((byte)move.AttackerSide, (byte)move.AttackerType, BOp.Set1(pieces.GetPieceBitboard((byte)move.AttackerSide, (byte)move.AttackerType), move.To));
            if (move.DefenderType != Piece.None)
            {
                pieces.SetPieceBitboard((byte)move.DefenderSide, (byte)move.DefenderType, BOp.Set0(pieces.GetPieceBitboard((byte)move.DefenderSide, (byte)move.DefenderType), move.To));
            }
            if (move.Flag == FLAG.EnPassantCapture)
            {
                if (move.AttackerSide == Side.White)
                    pieces.SetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn, BOp.Set0(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn), (byte)(move.To - 8)));
                else
                    pieces.SetPieceBitboard((byte)Side.White, (byte)Piece.Pawn, BOp.Set0(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn), (byte)(move.To + 8)));
            }

            pieces.UpdateBitboards();

            return !PseudoLegalMoveMaskGen.InDanger(pieces, BOp.Bsf(pieces.GetPieceBitboard((byte)move.AttackerSide, (byte)Piece.King)), move.AttackerSide);
        }

        private static void AddEnPassantCaptures(Pieces pieces, Side side, byte enPassant, MoveList moves)
        {
            if (enPassant == Position.Position.None)
                return;

            if (side == Side.White)
            {
                if (enPassant % 8 != 7 && BOp.GetBit(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn), (byte)(enPassant - 7)))
                {
                    Move move = new Move((byte)(enPassant - 7), (byte)enPassant, Piece.Pawn, Side.White, Piece.None, Side.None, Move.FLAG.EnPassantCapture);
                    if (IsLegal(pieces.Copy(), move))
                        moves.Push(move);
                }

                if (enPassant % 8 != 0 && BOp.GetBit(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn), (byte)(enPassant - 9)))
                {
                    Move move = new Move((byte)(enPassant - 9), (byte)enPassant, Piece.Pawn, Side.White, Piece.None, Side.None, Move.FLAG.EnPassantCapture);
                    if (IsLegal(pieces.Copy(), move))
                        moves.Push(move);
                }
            }
            else
            {
                
                if (enPassant % 8 != 0 && BOp.GetBit(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn), (byte)(enPassant + 7)))
                {
                    Move move = new Move((byte)(enPassant + 7), (byte)enPassant, Piece.Pawn, Side.Black, Piece.None, Side.None, Move.FLAG.EnPassantCapture);
                    if (IsLegal(pieces.Copy(), move))
                        moves.Push(move);
                }

                if (enPassant % 8 != 7 && BOp.GetBit(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn), (byte)(enPassant + 9)))
                {
                    Move move = new Move((byte)(enPassant + 9), (byte)enPassant, Piece.Pawn, Side.Black, Piece.None, Side.None, Move.FLAG.EnPassantCapture);
                    if (IsLegal(pieces.Copy(), move))
                        moves.Push(move);
                }
            }
        }

        private static void AddCastlingMoves(Pieces pieces, Side side, bool lCastling, bool sCastling, MoveList moves)
        {
            byte index;
            Move.FLAG longCastlingFlag;
            Move.FLAG shortCastlingFlag;

            if (side == Side.White)
            {
                index = 0;
                longCastlingFlag = Move.FLAG.WlCastling;
                shortCastlingFlag = Move.FLAG.WsCastling;
            }
            else
            {
                index = 56;
                longCastlingFlag = Move.FLAG.BlCastling;
                shortCastlingFlag = Move.FLAG.BsCastling;
            }

            if (lCastling &&
                BOp.GetBit(pieces.GetPieceBitboard((byte)side, (byte)Piece.Rook), index) &&
                BOp.GetBit(pieces.GetEmptyBitboard(), (byte)(index + 1)) &&
                BOp.GetBit(pieces.GetEmptyBitboard(), (byte)(index + 2)) &&
                BOp.GetBit(pieces.GetEmptyBitboard(), (byte)(index + 3)) &&
                !PseudoLegalMoveMaskGen.InDanger(pieces, BOp.Bsf(pieces.GetPieceBitboard((byte)side, (byte)Piece.King)), side) &&
                !PseudoLegalMoveMaskGen.InDanger(pieces, (byte)(index + 2), side) &&
                !PseudoLegalMoveMaskGen.InDanger(pieces, (byte)(index + 3), side))
            {
                moves.Push(new Move((byte)(index + 4), (byte)(index + 2), Piece.King, side, Piece.None, Side.None, longCastlingFlag));
            }

            if (sCastling &&
                BOp.GetBit(pieces.GetPieceBitboard((byte)side, (byte)Piece.Rook), (byte)(index + 7)) &&
                BOp.GetBit(pieces.GetEmptyBitboard(), (byte)(index + 5)) &&
                BOp.GetBit(pieces.GetEmptyBitboard(), (byte)(index + 6)) &&
                !PseudoLegalMoveMaskGen.InDanger(pieces, BOp.Bsf(pieces.GetPieceBitboard((byte)side, (byte)Piece.King)), side) &&
                !PseudoLegalMoveMaskGen.InDanger(pieces, (byte)(index + 5), side) &&
                !PseudoLegalMoveMaskGen.InDanger(pieces, (byte)(index + 6), side))
            {
                moves.Push(new Move((byte)(index + 4), (byte)(index + 6), Piece.King, side, Piece.None, Side.None, shortCastlingFlag));
            }
        }
    }
}
