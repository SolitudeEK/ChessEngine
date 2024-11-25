using ChessEngine.MoveGeneration;
using ChessEngine.Position;
using UnitTests.Utilities;

namespace UnitTests
{
    public class LegalMoveTest
    {
        private List<Test> Tests = new List<Test>
            {
            new Test(
                "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR",
                Position.None,
                true,
                true,
                true,
                true,
                Side.White,
                new ulong[] { 1, 20 , 400, 8902, 197281, 4865609 }
            ),
            new Test(
                "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R",
                Position.None,
                true,
                true,
                true,
                true,
                Side.White,
                new ulong[] { 1, 48, 2039, 97862, 4085603}
            ),
            new Test(
                "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8",
                Position.None,
                false,
                false,
                false,
                false,
                Side.White,
                new ulong[] { 1, 14, 191, 2812, 43238, 674624 }
            ),
            new Test(
                "r3k2r/Pppp1ppp/1b3nbN/nP6/BBP1P3/q4N2/Pp1P2PP/R2Q1RK1",
                Position.None,
                false,
                false,
                true,
                true,
                Side.White,
                new ulong[] { 1, 6, 264, 9467, 422333 }
            ),
            new Test(
                "rnbq1k1r/pp1Pbppp/2p5/8/2B5/8/PPP1NnPP/RNBQK2R",
                Position.None,
                true,
                true,
                false,
                false,
                Side.White,
                new ulong[] { 1, 44, 1486, 62379, 2103487}
            ),
            new Test(
                "r4rk1/1pp1qppp/p1np1n2/2b1p1B1/2B1P1b1/P1NP1N2/1PP1QPPP/R4RK1",
                Position.None,
                false,
                false,
                false,
                false,
                Side.White,
                new ulong[] { 1, 46, 2079, 89890, 3894594 }
            )
            };

        [Test]
        public void PossibleMovmentsTest()
        {
            foreach( var test in Tests)
                RunTest( test );
        }

        private void RunTest(Test test)
        {
            var position = new Position(
                test.ShortFen, test.EnPassant, test.WlCastling, test.WsCastling, test.BlCastling, test.BsCastling, 0f);

            for (int i = 0; i < test.Nodes.Length; i++)
            {
                var start = DateTime.UtcNow.Ticks;

                ulong correct = test.Nodes[i];
                ulong got = GetNodesNumber(position, test.Side, i);
                var speed = (float)got / ((float)(DateTime.UtcNow.Ticks - start) / (float)1e+7) / (float)1e+6;

                if (correct == got)
                {
                    Console.WriteLine($"Depth {i,4}. Correct: {correct,18}. Got: {got,18}. Speed: {speed,10} MNPS. OK.");
                }
                else
                {
                    Console.WriteLine($"Depth {i,4}. Correct: {correct,18}. Got: {got,18}. Speed: {speed,10} MNPS. Error.");
                }
                Assert.IsTrue( correct == got);
            }
            Console.WriteLine();
        }


        private ulong GetNodesNumber(Position position, Side side, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }

            var moves = LegalMoveGen.Generate(position, side);
            ulong ctr = 0;

            for (byte i = 0; i < moves.GetSize(); i++)
            {
                var copy = position.Copy();
                copy.Move(moves[i]);
                ctr += GetNodesNumber(copy, Pieces.Inverse(side), depth - 1);
            }


            return ctr;
        }
    }
    internal class Test
    {
        public string ShortFen { get; }
        public byte EnPassant { get; }
        public bool WlCastling { get; }
        public bool WsCastling { get; }
        public bool BlCastling { get; }
        public bool BsCastling { get; }
        public Side Side { get; }
        public ulong[] Nodes { get; }

        public Test(string shortFen, byte enPassant, bool wlCastling, bool wsCastling, bool blCastling, bool bsCastling, Side side, ulong[] nodes)
        {
            ShortFen = shortFen;
            EnPassant = enPassant;
            WlCastling = wlCastling;
            WsCastling = wsCastling;
            BlCastling = blCastling;
            BsCastling = bsCastling;
            Side = side;
            Nodes = nodes;
        }
    }
}
