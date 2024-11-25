using ChessEngine.Position;

namespace ChessEngine.MoveGeneration
{
    public class PseudoLegalMoveMaskGen
    {
        public static ulong GeneratePawnsDefaultMask(Pieces pieces, byte side)
        {
            if (side == (byte)Side.White)
                return (pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn) << 8) & pieces.GetEmptyBitboard();
            return (pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn) >> 8) & pieces.GetEmptyBitboard();
        }

        public static ulong GeneratePawnsLongMask(Pieces pieces, byte side)
        {
            ulong defaultMask = GeneratePawnsDefaultMask(pieces, side);
            if (side == (byte)Side.White)
                return ((defaultMask & BRows.Rows[2]) << 8) & pieces.GetEmptyBitboard();
            return ((defaultMask & BRows.Rows[5]) >> 8) & pieces.GetEmptyBitboard();
        }

        public static ulong GeneratePawnsLeftCapturesMask(Pieces pieces, byte side, bool includeAllAttacks)
        {
            ulong mask;
            if (side == (byte)Side.White)
            {
                mask = (pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn) << 7) & BColumns.InvColumns[7];
                if (!includeAllAttacks)
                    mask &= pieces.GetSideBitboard(Side.Black);
                return mask;
            }

            mask = (pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn) >> 9) & BColumns.InvColumns[7];
            if (!includeAllAttacks)
                mask &= pieces.GetSideBitboard(Side.White);
            return mask;
        }

        public static ulong GeneratePawnsRightCapturesMask(Pieces pieces, byte side, bool includeAllAttacks)
        {
            ulong mask;
            if (side == (byte)Side.White)
            {
                mask = (pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn) << 9) & BColumns.InvColumns[0];
                if (!includeAllAttacks)
                    mask &= pieces.GetSideBitboard(Side.Black);
                return mask;
            }

            mask = (pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn) >> 7) & BColumns.InvColumns[0];
            if (!includeAllAttacks)
                mask &= pieces.GetSideBitboard(Side.White);
            return mask;
        }

        public static ulong GenerateKnightMask(Pieces pieces, byte p, Side side, bool onlyCaptures)
        {
            if (onlyCaptures)
                return KnightMasks.Masks[p] & pieces.GetSideBitboard(Pieces.Inverse(side));
            return KnightMasks.Masks[p] & pieces.GetInvSideBitboard(side);
        }

        public static ulong GenerateKingMask(Pieces pieces, byte p, Side side, bool onlyCaptures)
        {
            if(onlyCaptures)
                return KingMasks.Masks[p] & pieces.GetSideBitboard(Pieces.Inverse(side));
            return KingMasks.Masks[p] & pieces.GetInvSideBitboard(side);
        }

        public static ulong GenerateBishopMask(Pieces pieces, byte p, byte side, bool onlyCaptures)
        {
            ulong nw = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.NorthWest, false);
            ulong ne = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.NorthEast, false);
            ulong sw = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.SouthWest, true);
            ulong se = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.SouthEast, true);

            return nw | ne | sw | se;
        }

        public static ulong GenerateRookMask(Pieces pieces, byte p, byte side, bool onlyCaptures)
        {
            ulong n = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.North, false);
            ulong s = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.South, true);
            ulong w = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.West, true);
            ulong e = CalcRay(pieces, p, side, onlyCaptures, SlidersMasks.Direction.East, false);

            return n | s | w | e;
        }

        public static ulong GenerateQueenMask(Pieces pieces, byte p, byte side, bool onlyCaptures)
        {
            ulong bishopMask = GenerateBishopMask(pieces, p, side, onlyCaptures);
            ulong rookMask = GenerateRookMask(pieces, p, side, onlyCaptures);

            return bishopMask | rookMask;
        }

        public static bool InDanger(Pieces pieces, byte p, Side side) // Not sure if it works corerctly
        {
            ulong oppositePawnsLeftCaptures = GeneratePawnsLeftCapturesMask(pieces, (byte)Pieces.Inverse(side), true);
            ulong oppositePawnsRightCaptures = GeneratePawnsRightCapturesMask(pieces, (byte)Pieces.Inverse(side), true);
            ulong oppositePawnsCaptures = oppositePawnsLeftCaptures | oppositePawnsRightCaptures;

            if(BOp.GetBit(oppositePawnsCaptures, p))
                return true;

            if ((GenerateKnightMask(pieces, p, side, true) & pieces.GetPieceBitboard((byte)Pieces.Inverse(side), (byte)Piece.Knight)) != 0)
                return true;
            if ((GenerateBishopMask(pieces, p, (byte)side, true) & pieces.GetPieceBitboard((byte)Pieces.Inverse(side), (byte)Piece.Bishop)) != 0)
                return true;
            if ((GenerateRookMask(pieces, p, (byte)side, true) & pieces.GetPieceBitboard((byte)Pieces.Inverse(side), (byte)Piece.Rook)) != 0)
                return true;
            if ((GenerateQueenMask(pieces, p, (byte)side, true) & pieces.GetPieceBitboard((byte)Pieces.Inverse(side), (byte)Piece.Queen)) != 0)
                return true;
            if ((GenerateKingMask(pieces, p, side, true) & pieces.GetPieceBitboard((byte)Pieces.Inverse(side), (byte)Piece.King)) != 0)
                return true;

            return false;
        }

        private static ulong CalcRay(Pieces pieces, byte p, byte side, bool onlyCaptures, SlidersMasks.Direction direction, bool bsr) 
        {
            ulong blockers = SlidersMasks.Masks[p,(byte)direction] & pieces.GetAllBitboard();
            if (blockers == 0)
            {
                if (onlyCaptures)
                {
                    return 0;
                }
                return SlidersMasks.Masks[p, (byte)direction];
            }

            byte blockingSquare;
            if (bsr)
                blockingSquare = BOp.Bsr(blockers);
            else
                blockingSquare = BOp.Bsf(blockers);

            ulong moves;
            if (onlyCaptures)
                moves = 0;
            else
                moves = SlidersMasks.Masks[p, (byte)direction] ^ SlidersMasks.Masks[blockingSquare, (byte)direction];

            if (BOp.GetBit(pieces.GetSideBitboard(side.Convert<Side>()), blockingSquare))
                moves = BOp.Set0(moves, blockingSquare);
            else
                moves = BOp.Set1(moves, blockingSquare);

            return moves;
        }
    }
}
