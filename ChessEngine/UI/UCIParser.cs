using ChessEngine.Position;
using System;
using static ChessEngine.Position.Move;

namespace ChessEngine.UI
{
    public static class UCIParser
    {
        public static Move Convert(this string notation, Position.Position position, Side side)
        {
            byte from = ConvertToIndex(notation.Substring(0, 2));
            byte to = ConvertToIndex(notation.Substring(2, 2));
            char promotion = notation.Length == 5 ? notation[4] : '*';

            var boards = position.Pieces.GetPieceBitboards();

            Piece attacker = GetPieceType(from, boards, (byte)side);
            Piece deffender = GetPieceType(to, boards, (byte)Pieces.Inverse(side));

            var flag = GetFlag(from, to, attacker, promotion, position);

            return new Move(from, to, attacker, side, deffender, Pieces.Inverse(side), flag);
        }

        public static string Convert(Move move)
            => $"{ConvertToSquare(move.From)}{ConvertToSquare(move.To)}{FlagToPiece(move.Flag)}";

        private static Move.FLAG GetFlag(byte from, byte to, Piece piece, char promotion, Position.Position position)
        {
            if(piece == Piece.Pawn) { 
                if (promotion != '*')
                {
                    return promotion switch
                    {
                        'q' => Move.FLAG.PromoteToQueen,
                        'r' => Move.FLAG.PromoteToRook,
                        'b' => Move.FLAG.PromoteToBishop,
                        'n' => Move.FLAG.PromoteToKnight,
                        _ => throw new ArgumentException("Invalid promotion piece")
                    };
                }

                if (position.EnPassant == to)
                    return Move.FLAG.EnPassantCapture;

                if (Math.Abs(from - to) == 16)
                    return Move.FLAG.PawnLongMove;
            }
            else if(piece == Piece.King)
            {
                if (from == 4 && to == 6) return FLAG.WsCastling;
                if (from == 4 && to == 2) return FLAG.WlCastling;
                if (from == 60 && to == 62) return FLAG.BsCastling;
                if (from == 60 && to == 58) return FLAG.BlCastling;
            }

            return Move.FLAG.Default;
        }

        private static byte ConvertToIndex(string square)
        {
            int file = square[0] - 'a';
            int rank = (square[1] - '1');
            return (byte)(rank * 8 + file);
        }

        private static string ConvertToSquare(byte index)
        {
            char file = (char)('a' + index % 8);
            char rank = (char)('1' + index / 8);
            return $"{file}{rank}";
        }

        private static Piece GetPieceType(byte index, ulong[,] boards, byte side)
        {

            for (byte piece = 0; piece < 6; piece++)
            {
                if (BOp.GetBit(boards[side, piece], index))
                    return (Piece)piece;
            }
            
            return Piece.None;
        }

        private static string FlagToPiece(FLAG flag)
            => flag switch
            {
                FLAG.PromoteToQueen => "q",
                FLAG.PromoteToRook => "r",
                FLAG.PromoteToBishop => "b",
                FLAG.PromoteToKnight => "n",
                _ => string.Empty
            };
        
    }
}
