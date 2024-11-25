namespace ChessEngine.Position
{
    using System.Numerics;

    public static class BOp
    {
        private static readonly byte[] BitScanTable ={
            0, 47,  1, 56, 48, 27,  2, 60,
            57, 49, 41, 37, 28, 16,  3, 61,
            54, 58, 35, 52, 50, 42, 21, 44,
            38, 32, 29, 23, 17, 11,  4, 62,
            46, 55, 26, 59, 40, 36, 15, 53,
            34, 51, 20, 43, 31, 22, 10, 45,
            25, 39, 14, 33, 19, 30,  9, 24,
            13, 18,  8, 12,  7,  6,  5, 63
        };

        public static ulong Set1(ulong bb, byte square)
            => bb | (1UL << square);


        public static ulong Set0(ulong bb, byte square)
            => bb & ~(1UL << square);

        public static bool GetBit(ulong bb, byte square)
            => (bb & (1UL << square)) != 0;

        public static int Count1(ulong bb)
            => BitOperations.PopCount(bb);

        public static byte Bsf(ulong bb)
            => BitScanTable[((bb ^ (bb - 1)) * 0x03f79d71b4cb0a89UL) >> 58];

        public static byte Bsr(ulong bb)
        {
            bb |= bb >> 1;
            bb |= bb >> 2;
            bb |= bb >> 4;
            bb |= bb >> 8;
            bb |= bb >> 16;
            bb |= bb >> 32;
            return BitScanTable[(bb * 0x03f79d71b4cb0a89UL) >> 58];
        }
    }

    public static class BRows
    {
        public static readonly ulong[] Rows = CalculateRows();

        public static readonly ulong[] InvRows = CalculateInvRows();

        private static ulong[] CalculateRows()
        {
            var rows = new ulong[8];
            for (byte y = 0; y < 8; y++)
            {
                for (byte x = 0; x < 8; x++)
                {
                    rows[y] = BOp.Set1(rows[y], (byte)(y * 8 + x));
                }
            }
            return rows;
        }
        private static ulong[] CalculateInvRows()
        {
            var invRows = new ulong[8];
            for (byte i = 0; i < 8; i++)
                invRows[i] = ~Rows[i];
            return invRows;
        }
    }

    public static class BColumns 
    {
        public static readonly ulong[] Columns = CalculateColumns();

        public static readonly ulong[] InvColumns = CalculateInvColumns();

        private static ulong[] CalculateColumns()
        {
            var cols = new ulong[8];
            for(byte i = 0; i < 8; i++)
            {
                for (byte j = 0; j < 8; j++)
                    cols[i] = BOp.Set1(cols[i], (byte)(j * 8 + i));
            }
            return cols;
        }

        private static ulong[] CalculateInvColumns()
        {
            var invCols = new ulong[8];
            for(byte i = 0; i < 8; i++)
                invCols[i] = ~Columns[i];
            return invCols;
        }
    }
}
