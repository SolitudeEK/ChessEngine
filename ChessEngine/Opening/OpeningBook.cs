using ChessEngine.Position;

namespace ChessEngine.Opening
{
    public class OpeningBook
    {
        private List<Move> _moves = new List<Move>();
        public OpeningBook(string path)
        {

        }

        public Tuple<Move, int> TryToFindMove(Position.Position position)
        {
            return null;
        }
    }
}
