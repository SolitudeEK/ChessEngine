namespace ChessEngine.MoveGeneration
{
    public static class Common
    {
        public static byte AbsSubtract(byte left, byte right) =>
            (byte)(left >= right ? left - right : right - left);

        public static T Convert<T>(this byte value) where T : Enum
        {
            if (Enum.IsDefined(typeof(T), value))
                return (T)Enum.ToObject(typeof(T), value);
            throw new ArgumentException("No such a enum");
        }
    }
}
