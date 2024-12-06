using ChessEngine.MoveGeneration;
using ChessEngine.Position;

namespace UnitTests
{
    public class GeneralPerformanceTest
    {
        [Test]
        public void TestDeepCopyTime()
        {
            Position pos = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", 255, true, true, true, true, 0);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for(int i = 0; i < 1000; i++)
                pos.Copy();
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }

        [Test]
        public void TestMoveCalcTime()
        {
            Position pos = new Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", 255, true, true, true, true, 0);
            var watch = System.Diagnostics.Stopwatch.StartNew();
            for (int i = 0; i < 100; i++)
                LegalMoveGen.Generate(pos, Side.White);
            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds);
        }
    }
}
