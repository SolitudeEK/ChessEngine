namespace ChessEngine.Position.Hashing
{
    public  class ZobristHash
    {
        private ulong _value;
        public ZobristHash(Pieces pieces, bool blackTurn, bool wlCastling, bool wsCastling, bool blCastling, bool bsCastling)
        {
            _value = 0;

            if (blackTurn) InvertMove();

            if (wlCastling) InvertWLCastling();

            if (wsCastling) InvertWSCastling();

            if (blCastling) InvertBLCastling();

            if (bsCastling) InvertBSCastling();

            for (byte square = 0; square < 64; square++)
            {
                byte side;

                if (BOp.GetBit(pieces.GetSideBitboard(Side.White), square))
                    side = (byte)Side.White;
                else if (BOp.GetBit(pieces.GetSideBitboard(Side.Black), square))
                    side = (byte)Side.Black;
                else
                    continue;

                for (byte type = 0; type < 6; type++)
                {
                    if (BOp.GetBit(pieces.GetPieceBitboard(side, type), square))
                    {
                        InvertPiece(square, type, side);
                        break;
                    }
                }
            }
        }

        public void InvertPiece(byte square, byte type, byte side)
           => _value ^= ZobristHashConstants.CONSTANTS[square, side, type];

        public void InvertMove()
            => _value ^= ZobristHashConstants.BLACK_MOVE;

        public void InvertWLCastling()
            => _value ^= ZobristHashConstants.WL_CASTLING;

        public void InvertWSCastling()
            => _value ^= ZobristHashConstants.WS_CASTLING;

        public void InvertBLCastling()
            => _value ^= ZobristHashConstants.BL_CASTLING;

        public void InvertBSCastling()
            => _value ^= ZobristHashConstants.BS_CASTLING;

        public ulong Value
            => _value;

        public override bool Equals(object obj)
        {
            if (obj is ZobristHash other)
                return _value == other.Value;

            return false;
        }

        public override int GetHashCode()
            => _value.GetHashCode();
    }
}
