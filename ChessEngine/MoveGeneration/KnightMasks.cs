using ChessEngine.Position;
namespace ChessEngine.MoveGeneration
{
    public class KnightMasks
    {
        public static readonly ulong[] Masks = CalcMasks();
        private static ulong[] CalcMasks()
        {
            var masks = new ulong[64];

            for (byte x0 = 0; x0 < 8; x0++)
            {
                for (byte y0 = 0; y0 < 8; y0++)
                {
                    for (byte x1 = 0; x1 < 8; x1++)
                    {
                        for (byte y1 = 0; y1 < 8; y1++)
                        {
                            byte dx = Common.AbsSubtract(x0, x1);
                            byte dy = Common.AbsSubtract(y0, y1);

                            if ((dx == 2 && dy == 1) || (dx == 1 && dy == 2))
                            {
                                masks[y0 * 8 + x0] = BOp.Set1(masks[y0 * 8 + x0], (byte)(y1 * 8 + x1));
                            }
                        }
                    }
                }
            }

            return masks;
        }

       
    }
}
