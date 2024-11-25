using ChessEngine.MoveGeneration;
using ChessEngine.Position;
using UnitTests.Utilities;

namespace UnitTests
{
    public class PseudoMoveTest
    {
        private Pieces pieces1 = new Pieces("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR");
        private Pieces pieces2 = new Pieces("8/8/8/8/8/8/8/8");
        private Pieces pieces3 = new Pieces("rnbqkbnr/pppppp2/6pp/8/8/6PP/PPPPPP2/RNBQKBNR");
        private Pieces pieces4 = new Pieces("8/8/8/pppppppp/PPPPPPPP/8/8/8");
        private Pieces pieces5 = new Pieces("r7/8/8/8/8/8/8/8");

        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void TestBishopNoMove()
        {
            var a = PseudoLegalMoveMaskGen.GenerateBishopMask(pieces1, 2, (byte)Side.White, false);

            a.ShowAsBoard();

            Assert.IsTrue(a == (ulong)0);
        }

        [Test]
        public void TestBishop()
        {
            var a = PseudoLegalMoveMaskGen.GenerateBishopMask(pieces2, 20, (byte)Side.White, false);

            a.ShowAsBoard();

            Assert.IsTrue(a == 424704217196612);
        }

        [Test]
        public void TestRook()
        {
            var moves = PseudoLegalMoveMaskGen.GenerateRookMask(pieces2, 30, (byte)Side.White, false);

            moves.ShowAsBoard();

            Assert.IsTrue(moves == 4629771063767613504);
        }

        [Test]
        public void TestQueen()
        {
            var moves = PseudoLegalMoveMaskGen.GenerateQueenMask(pieces1, 28, (byte)Side.Black, false);

            moves.ShowAsBoard();

            Assert.IsTrue(moves == 92603508347904);
        }

        [Test]
        public void TestKnight()
        {
            var moves = PseudoLegalMoveMaskGen.GenerateKnightMask(pieces1, 28, Side.Black, true);

            moves.ShowAsBoard();

            Assert.IsTrue(moves == 10240);
        }

        [Test]
        public void TestKing()
        {
            var moves = PseudoLegalMoveMaskGen.GenerateKingMask(pieces1, 20, Side.Black, false);

            moves.ShowAsBoard();

            Assert.IsTrue(moves == 942159872);
        }

        [Test]
        public void TestPawnsLong()
        {
            var moves = PseudoLegalMoveMaskGen.GeneratePawnsLongMask(pieces3, (byte)Side.Black);

            moves.ShowAsBoard();

            Assert.IsTrue(moves == 270582939648);
        }

        [Test]
        public void TestPawnsShort()
        {
            var moves = PseudoLegalMoveMaskGen.GeneratePawnsDefaultMask(pieces3, (byte)Side.Black);

            moves.ShowAsBoard();

            Assert.IsTrue(moves == 70093866270720);
        }

        [Test]
        public void TestPawnLeftTake()
        {
            var moves = PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(pieces4, (byte)Side.Black, false);

            moves.ShowAsBoard();

            Console.WriteLine(moves);
            Assert.IsTrue(moves == 2130706432);
        }

        [Test]
        public void TestPawnRightTake()
        {
            var moves = PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(pieces4, (byte)Side.Black, false);

            moves.ShowAsBoard();

            Console.WriteLine(moves);
            Assert.IsTrue(moves == 4261412864);
        }

        [Test]
        public void TestInDanger()
        {
            var isInDanger = PseudoLegalMoveMaskGen.InDanger(pieces5, 0, Side.White);
            Assert.IsTrue(isInDanger);
        }
    }
}