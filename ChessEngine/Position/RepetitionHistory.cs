namespace ChessEngine.Position
{
    public class RepetitionHistory
    {
        private Dictionary<ulong, int> _hashCounts = new Dictionary<ulong, int>();

        public void AddPosition(ulong hash)
        {
            if (_hashCounts.ContainsKey(hash))
                _hashCounts[hash]++;
            else
                _hashCounts[hash] = 1;
        }

        public void Clear()
            => _hashCounts.Clear();

        public byte GetRepetitionNumber(ulong hash)
            => _hashCounts.TryGetValue(hash, out int count) ? (byte)count : (byte)0;
    }
}
