using ChessEngine.Position;

namespace ChessEngine.MoveGeneration
{
    public class MoveList
    {
        private Move[] _moves;
        private byte _size;

        public MoveList()
        {
            _moves = new Move[218];
            _size = 0;
        }

        public Move this[byte index]
        {
            get => _moves[index];
            set => _moves[index] = value;
        }

        public void Push(Move move)
        {
            if (_size < 220)
            {
                _moves[this._size] = move;
                _size++;
            }
            else
                throw new InvalidOperationException("MoveList is full.");
        }

        public void Swap(byte i, byte k)
        {
            var temp = _moves[i];
            _moves[i] = _moves[k];
            _moves[k] = temp;
        }

        public byte GetSize() => this._size;
    }
}
