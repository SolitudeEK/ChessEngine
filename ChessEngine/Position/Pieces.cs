namespace ChessEngine.Position
{
    using System.Text;

    public enum Piece : byte
    {
        Pawn = 0,
        Knight = 1,
        Bishop = 2,
        Rook = 3,
        Queen = 4,
        King = 5,
        None = 255
    }

    public enum Side : byte
    {
        White = 0,
        Black = 1,
        None = 255
    }

    public class Pieces
    {
        private readonly ulong[,] _pieceBitboards = new ulong[2, 6];
        private readonly ulong[] _sideBitboards = new ulong[2];
        private readonly ulong[] _invSideBitboards = new ulong[2];
        private ulong _all;
        private ulong _empty;

        public Pieces(string shortFen)
        {
            byte x = 0;
            byte y = 7;

            byte side;

            foreach (var buff in shortFen)
            {
                if (buff == '/')
                {
                    x = 0;
                    y--;
                }
                else if (char.IsDigit(buff))
                    x += (byte)(buff - '0');
                else
                {
                    side = char.IsUpper(buff) ? (byte)Side.White : (byte)Side.Black;

                    switch (char.ToLower(buff))
                    {
                        case 'p':
                            _pieceBitboards[side, (byte)Piece.Pawn] = BOp.Set1(_pieceBitboards[side, (byte)Piece.Pawn], (byte)(y * 8 + x));
                            break;
                        case 'n':
                            _pieceBitboards[side, (byte)Piece.Knight] = BOp.Set1(_pieceBitboards[side, (byte)Piece.Knight], (byte)(y * 8 + x));
                            break;
                        case 'b':
                            _pieceBitboards[side, (byte)Piece.Bishop] = BOp.Set1(_pieceBitboards[side, (byte)Piece.Bishop], (byte)(y * 8 + x));
                            break;
                        case 'r':
                            _pieceBitboards[side, (byte)Piece.Rook] = BOp.Set1(_pieceBitboards[side, (byte)Piece.Rook], (byte)(y * 8 + x));
                            break;
                        case 'q':
                            _pieceBitboards[side, (byte)Piece.Queen] = BOp.Set1(_pieceBitboards[side, (byte)Piece.Queen], (byte)(y * 8 + x));
                            break;
                        case 'k':
                            _pieceBitboards[side, (byte)Piece.King] = BOp.Set1(_pieceBitboards[side, (byte)Piece.King], (byte)(y * 8 + x));
                            break;
                    }

                    x++;
                }
            }

            UpdateBitboards();
        }

        private Pieces(ulong[,] pieceBitboards)
        {
            _pieceBitboards = pieceBitboards;
            UpdateBitboards();
        }

        public void UpdateBitboards()
        {
            _sideBitboards[(byte)Side.White] = _pieceBitboards[(byte)Side.White, (int)Piece.Pawn] |
                                               _pieceBitboards[(byte)Side.White, (int)Piece.Knight] |
                                               _pieceBitboards[(byte)Side.White, (int)Piece.Bishop] |
                                               _pieceBitboards[(byte)Side.White, (int)Piece.Rook] |
                                               _pieceBitboards[(byte)Side.White, (int)Piece.Queen] |
                                               _pieceBitboards[(byte)Side.White, (int)Piece.King];

            _sideBitboards[(byte)Side.Black] = _pieceBitboards[(byte)Side.Black, (int)Piece.Pawn] |
                                               _pieceBitboards[(byte)Side.Black, (int)Piece.Knight] |
                                               _pieceBitboards[(byte)Side.Black, (int)Piece.Bishop] |
                                               _pieceBitboards[(byte)Side.Black, (int)Piece.Rook] |
                                               _pieceBitboards[(byte)Side.Black, (int)Piece.Queen] |
                                               _pieceBitboards[(byte)Side.Black, (int)Piece.King];

            _invSideBitboards[(byte)Side.White] = ~_sideBitboards[(byte)Side.White];
            _invSideBitboards[(byte)Side.Black] = ~_sideBitboards[(byte)Side.Black];

            _all = _sideBitboards[(byte)Side.White] | _sideBitboards[(byte)Side.Black];
            _empty = ~_all;
        }

        public void SetPieceBitboard(byte side, byte piece, ulong bb)
            => _pieceBitboards[side, piece] = bb;

        public ulong[,] GetPieceBitboards()
            => (ulong[,])_pieceBitboards.Clone();

        public ulong GetPieceBitboard(byte side, byte piece)
            => _pieceBitboards[side, piece];

        public ulong GetSideBitboard(Side side)
            => side == Side.White ? _sideBitboards[0] : _sideBitboards[1];

        public ulong GetInvSideBitboard(Side side)
            => side == Side.White ?  _invSideBitboards[0] : _invSideBitboards[1];

        public ulong GetAllBitboard()
            => _all;

        public ulong GetEmptyBitboard()
            => _empty;

        public static Side Inverse(Side side)
            => side == Side.White ? Side.Black : Side.White;

        public override string ToString()
        {
            var sb = new StringBuilder();

            for (int y = 7; y >= 0; y--)
            {
                for (int x = 0; x < 8; x++)
                {
                    sb.Append("| ");

                    byte index = (byte)(y * 8 + x);


                    if (BOp.GetBit(_pieceBitboards[(byte)Side.Black, (byte)Piece.Pawn], index))
                        sb.Append("♙");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.Black, (byte)Piece.Knight], index))
                        sb.Append("♘");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.Black, (byte)Piece.Bishop], index))
                        sb.Append("♗");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.Black, (byte)Piece.Rook], index))
                        sb.Append("♖");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.Black, (byte)Piece.Queen], index))
                        sb.Append("♕");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.Black, (byte)Piece.King], index))
                        sb.Append("♔");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.White, (byte)Piece.Pawn], index))
                        sb.Append("♟");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.White, (byte)Piece.Knight], index))
                        sb.Append("♞");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.White, (byte)Piece.Bishop], index))
                        sb.Append("♝");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.White, (byte)Piece.Rook], index))
                        sb.Append("♜");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.White, (byte)Piece.Queen], index))
                        sb.Append("♛");
                    else if (BOp.GetBit(_pieceBitboards[(byte)Side.White, (byte)Piece.King], index))
                        sb.Append("♚");
                    else
                        sb.Append(" ");

                    sb.Append(" ");
                }
                sb.AppendLine("|");
            }
            return sb.ToString();
        }
    }
}
