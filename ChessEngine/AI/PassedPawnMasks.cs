using ChessEngine.Position;

namespace ChessEngine.AI
{
    public static class PassedPawnMasks
    {
        public static ulong[] CalculateWhitePassedPawnMasks()
        {
            ulong[] masks = new ulong[64];

            for (byte x = 0; x < 8; x++)
            {
                for (byte y = 0; y < 8; y++)
                {
                    for (byte y1 = (byte)(y + 1); y1 < 8; y1++)
                    {
                        if (x != 0)
                        {
                            masks[y * 8 + x] = BOp.Set1(masks[y * 8 + x], (byte)(y1 * 8 + x - 1));
                        }
                        if (x != 7)
                        {
                            masks[y * 8 + x] = BOp.Set1(masks[y * 8 + x], (byte)(y1 * 8 + x + 1));
                        }
                        masks[y * 8 + x] = BOp.Set1(masks[y * 8 + x], (byte)(y1 * 8 + x));
                    }
                }
            }

            return masks;
        }

        public static ulong[] CalculateBlackPassedPawnMasks()
        {
            ulong[] masks = new ulong[64];

            for (byte x = 0; x < 8; x++)
            {
                for (byte y = 0; y < 8; y++)
                {
                    for (sbyte y1 = (sbyte)(y - 1); y1 >= 0; y1--)
                    {
                        if (x != 0)
                        {
                            masks[y * 8 + x] = BOp.Set1(masks[y * 8 + x], (byte)(y1 * 8 + x - 1));
                        }
                        if (x != 7)
                        {
                            masks[y * 8 + x] = BOp.Set1(masks[y * 8 + x], (byte)(y1 * 8 + x + 1));
                        }
                        masks[y * 8 + x] = BOp.Set1(masks[y * 8 + x], (byte)(y1 * 8 + x));
                    }
                }
            }

            return masks;
        }

        public static readonly ulong[] WhitePassedPawnMasks = CalculateWhitePassedPawnMasks();
        public static readonly ulong[] BlackPassedPawnMasks = CalculateBlackPassedPawnMasks();
    }
}
