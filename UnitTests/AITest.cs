using ChessEngine.AI;
using ChessEngine.Position;

namespace UnitTests
{
    public class AITest
    {
        private const string fen1 = "r3k2r/p1ppqpb1/bn2pnp1/3PN3/1p2P3/2N2Q1p/PPPBBPPP/R3K2R";
        private Position position1 = new Position(fen1, 255, true, true, true, true, 0);

        [Test]
        public async Task AIMoveTest1()
        {
            var move = await AI.GetBestMove(position1, Side.White, 1000);

            Assert.True(move != null);
        }
    }
}
