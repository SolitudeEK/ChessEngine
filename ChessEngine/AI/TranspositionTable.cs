using ChessEngine.Position;
using ChessEngine.Position.Hashing;

namespace ChessEngine.AI
{
    public class TranspositionTable
    {
        private static TranspositionTable _instance;
        private readonly Dictionary<ulong, (int Depth, byte BestMoveIndex)> _map;
        private Mutex _mutex = new Mutex();

        private TranspositionTable()
            => _map = new Dictionary<ulong, (int, byte)>();

        public static TranspositionTable Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TranspositionTable();
                return _instance;
            }
        }

        public void AddEntry(ZobristHash hash, int depth, byte bestMoveIndex)
        {
            _mutex.WaitOne();
            ulong hashValue = hash.Value;

            if (!_map.TryGetValue(hashValue, out var existingEntry))
                _map[hashValue] = (depth, bestMoveIndex);
            else if (existingEntry.Depth < depth)
                _map[hashValue] = (depth, bestMoveIndex);
            _mutex.ReleaseMutex();
        }

        public byte GetBestMoveIndex(ZobristHash hash)
        {
            if (_map.TryGetValue(hash.Value, out var entry))
                return entry.BestMoveIndex;
            return Move.None;
        }
    }
}
