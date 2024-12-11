using ChessEngine.Position;

namespace ChessEngine.Engine
{
    public class Engine
    {
        private Position.Position _position;
        
        public string GetMove(Move move)
        {
            throw new NotImplementedException();
        }

        public void SetMove(Move move)
            => _position.Move(move);

    }
}
