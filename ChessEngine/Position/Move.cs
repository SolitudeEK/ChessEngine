namespace ChessEngine.Position
{
    public class Move
    {
        public enum FLAG : byte
        {
            Default,
            PawnLongMove,
            EnPassantCapture,
            WlCastling,
            WsCastling,
            BlCastling,
            BsCastling,
            PromoteToKnight,
            PromoteToBishop,
            PromoteToRook,
            PromoteToQueen
        }

        public static readonly byte None = 255;

        public byte From { get; set; }
        public byte To { get; set; }
        public Piece AttackerType { get; set; }
        public Side AttackerSide { get; set; }
        public Piece DefenderType { get; set; }
        public Side DefenderSide { get; set; }
        public FLAG Flag { get; set; }

        public Move(byte from, byte to, Piece attackerType, Side attackerSide, Piece defenderType, Side defenderSide, FLAG flag = FLAG.Default)
        {
            From = from;
            To = to;
            AttackerType = attackerType;
            AttackerSide = attackerSide;
            DefenderType = defenderType;
            DefenderSide = defenderSide;
            Flag = flag;
        }

        public Move() { }

        public override string ToString()
            => $"Move: From={From}, To={To}, AttackerType={AttackerType}, AttackerSide={AttackerSide}, " +
                   $"DefenderType={DefenderType}, DefenderSide={DefenderSide}, Flag={Flag}";
    }
}
