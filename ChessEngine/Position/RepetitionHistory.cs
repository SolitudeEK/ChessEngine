namespace ChessEngine.Position
{
    public class RepetitionHistory
    {
        private List<ulong> _hashes = new List<ulong>();

        public void AddPosition(ulong hash)
            => _hashes.Add(hash);

        public void Clear()
            => _hashes.Clear();

        public byte GetRepetitionNumber(ulong hash)
            => (byte) _hashes.Count(h => h == hash);
    }
}
