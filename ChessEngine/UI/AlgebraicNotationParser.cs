using ChessEngine.Position;
using System.Text.RegularExpressions;

namespace ChessEngine.UI
{
    public static class AlgebraicNotationParser
    {
        public static Move ConvertToMove(this string notation, Position.Position position , Side side)
        {
            if (notation == "O-O" || notation == "O-O-O")
                return HandleCastling(notation, side);
            else
                return HandleMove(notation, position, side);
        }

        private static Move HandleCastling(string notation, Side side)
        {
            throw new NotImplementedException();
        }

        private static Move HandleMove(string notation, Position.Position position, Side side)
        {
            var pieceRegex = new Regex(@"([KQRBN])?([a-h]?[1-8]?)x?([a-h][1-8])(=[QRBN])?");
            var match = pieceRegex.Match(notation);

            if (!match.Success)
                throw new ArgumentException("Invalid algebraic notation");

            throw new NotImplementedException();
        }
    }
}
