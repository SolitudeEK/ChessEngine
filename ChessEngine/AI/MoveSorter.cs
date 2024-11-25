using ChessEngine.MoveGeneration;
using ChessEngine.Position;

namespace ChessEngine.AI
{
    public static class MoveSorter
    {
        public static MoveList Sort(Pieces pieces, MoveList moves)
        {
            byte n = moves.GetSize();

            for (byte i = 1; i < n; i++)
                for (byte j = 1; j < n - i; j++)
                    if (EvaluateMove(pieces, moves[j]) < EvaluateMove(pieces, moves[(byte)(j - 1)]))
                        moves.Swap(j, (byte)(j - 1));

            return moves;
        }

        private static int EvaluateMove(Pieces pieces, Move move)
        {
            int evaluation = 0;
            if (move.AttackerType != Piece.Pawn)
            {
                var opponentPawnsAttacks = PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(
                                               pieces, (byte)Pieces.Inverse(move.AttackerSide), true) |
                                           PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(
                                               pieces, (byte)Pieces.Inverse(move.AttackerSide), true);

                if (BOp.GetBit(opponentPawnsAttacks, move.To))
                {
                    evaluation -= move.AttackerType switch
                    {
                        Piece.Knight => Material.Knight,
                        Piece.Bishop => Material.Bishop,
                        Piece.Rook => Material.Rook,
                        Piece.Queen => Material.Queen,
                        _ => 0
                    };
                }
            }

            if (move.DefenderType != Piece.None)
            {
                evaluation += move.DefenderType switch
                {
                    Piece.Pawn => 1000 * Material.Pawn,
                    Piece.Knight => 1000 * Material.Knight,
                    Piece.Bishop => 1000 * Material.Bishop,
                    Piece.Rook => 1000 * Material.Rook,
                    Piece.Queen => 1000 * Material.Queen,
                    _ => 0
                };

                evaluation -= move.AttackerType switch
                {
                    Piece.Pawn => Material.Pawn,
                    Piece.Knight => Material.Knight,
                    Piece.Bishop => Material.Bishop,
                    Piece.Rook => Material.Rook,
                    Piece.Queen => Material.Queen,
                    _ => 0
                };
            }

            return evaluation;
        }
    }
}
