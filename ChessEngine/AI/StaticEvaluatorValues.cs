namespace ChessEngine.AI
{
    public static class Material
    {
        public const int Pawn = 100;
        public const int Knight = 300;
        public const int Bishop = 325;
        public const int Rook = 550;
        public const int Queen = 950;
    }

    public static class Mobility
    {
        public const int Knight = 12;
        public const int Bishop = 8;
        public const int Rook = 5;
        public const int Queen = 5;
    }

    public static class PawnStructure
    {
        public const int DoublePawn = -25;
        public const int ConnectedPawn = 10;

        public static readonly int[] DefaultPawnPromotion = { 0, 0, 0, 0, 10, 20, 30, 0 };
        public static readonly int[] PassedPawnPromotion = { 0, 50, 50, 50, 70, 90, 110, 0 };
    }

    public static class KingSafety
    {
        public const int Knight = 25;
        public const int Bishop = 25;
        public const int Rook = 25;
        public const int Queen = 50;
    }

    public static class Endgame
    {
        public const int MaximumPiecesForEndgame = 8;
        public const int ProximityKings = 10;
        public const int DistanceWeakKingMiddle = 10;
    }
}
