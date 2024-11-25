using ChessEngine.Position;
namespace ChessEngine.MoveGeneration
{
    public static class SlidersMasks
    {
        public static readonly ulong[,] Masks = CalcMasks();

        public enum Direction : byte
        {
            North,
            South,
            West,
            East,
            NorthWest,
            NorthEast,
            SouthWest,
            SouthEast
        }

        private static ulong CalcMask(byte position, Direction direction)
        {
            ulong mask = 0;

            byte x = (byte)(position % 8);
            byte y = (byte)(position / 8);

            while (true)
            {
                switch (direction)
                {
                    case Direction.North: y++; break;
                    case Direction.South: y--; break;
                    case Direction.West: x--; break;
                    case Direction.East: x++; break;

                    case Direction.NorthWest: y++; x--; break;
                    case Direction.NorthEast: y++; x++; break;
                    case Direction.SouthWest: y--; x--; break;
                    case Direction.SouthEast: y--; x++; break;
                }

                if (x > 7 || x < 0 || y > 7 || y < 0) break;

                mask = BOp.Set1(mask, (byte)(y * 8 + x));
            }

            return mask;
        }

        private static ulong[,] CalcMasks()
        {
            var masks = new ulong[64, 8];

            for (byte i = 0; i < 64; i++)
            {
                for (byte j = 0; j < 8; j++)
                {
                    masks[i, j] = CalcMask(i, j.Convert<Direction>());
                }
            }

            return masks;
        }
    }
}
