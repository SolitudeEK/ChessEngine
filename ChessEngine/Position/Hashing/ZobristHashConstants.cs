namespace ChessEngine.Position.Hashing
{
    public static class ZobristHashConstants
    {
        public static readonly ulong[,,] CONSTANTS = CalculateConstants();

        public static readonly ulong BLACK_MOVE = PRNG.NextRandomNumber(CONSTANTS[63, 1, 5]);
        public static readonly ulong WL_CASTLING = PRNG.NextRandomNumber(BLACK_MOVE);
        public static readonly ulong WS_CASTLING = PRNG.NextRandomNumber(WL_CASTLING);
        public static readonly ulong BL_CASTLING = PRNG.NextRandomNumber(WS_CASTLING);
        public static readonly ulong BS_CASTLING = PRNG.NextRandomNumber(BL_CASTLING);

        private static ulong[,,] CalculateConstants()
        {
            var constants = new ulong[64, 2, 6];
            ulong previous = PRNG.SEED;

            for (int square = 0; square < 64; square++)
            {
                for (int side = 0; side < 2; side++)
                {
                    for (int pieceType = 0; pieceType < 6; pieceType++)
                    {
                        previous = PRNG.NextRandomNumber(previous);
                        constants[square, side, pieceType] = previous;
                    }
                }
            }

            return constants;
        }
    }

    public static class PRNG //Pseudo random number generation
    {
        public const ulong SEED = 0x98f107;
        public const ulong MULTIPLIER = 0x71abc9;
        public const ulong SUMMAND = 0xff1b3f;

        public static ulong NextRandomNumber(ulong previous)
            => MULTIPLIER * previous + SUMMAND;
    }
}
