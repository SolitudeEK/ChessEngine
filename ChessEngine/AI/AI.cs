using ChessEngine.MoveGeneration;
using ChessEngine.Position;

namespace ChessEngine.AI
{
    public static class AI
    {
        private static readonly SearchInterrupter interrupter = SearchInterrupter.Instance;

        public static async Task<Move> GetBestMove(Position.Position position, Side side, int ms)
        {
            Console.WriteLine(position);
            StaticEvaluator.Evaluate(position.Pieces, true);

            long start = DateTime.Now.Ticks;
            interrupter.Resume();

            Console.WriteLine("Search started.");

            int eval = 0;
            bool gameWasFinished = false;
            Move move = null;

            for (int i = 1; i < 1000; i++)
            {
                var task = Task.Run(() => AlphaBeta(position, side, i));
                bool continueSearch = true;
                while (!task.IsCompleted)
                {
                    if ((DateTime.Now.Ticks - start) / 10000 >= ms)
                    {
                        continueSearch = false;
                        break;
                    }

                    await Task.Delay(20);
                }

                if (continueSearch || i == 1)
                {
                    var result = await task;
                    eval = result.Item1;
                    gameWasFinished = result.Item2;
                    move = result.Item3;
                }
                else
                {
                    interrupter.Interrupt();
                    var _ = await task;
                    break;
                }

                Console.WriteLine($"Base depth: {i,2}. Evaluation: {(float)eval / 100.0f,6} pawns. Time: {(DateTime.Now.Ticks - start) / 10000,10} ms.");
                if (gameWasFinished)
                    break;
            }

            Console.WriteLine("Search finished");
            Console.WriteLine(move.ToString());
            return move;
        }

        public static Task<Move> GetBestMovePharallel(Position.Position position, Side side, int ms)
        {
            Console.WriteLine(position);
            StaticEvaluator.Evaluate(position.Pieces, true);

            long start = DateTime.Now.Ticks;

            Console.WriteLine("Parallel search started.");
            
            using CancellationTokenSource cts = new CancellationTokenSource();
            cts.CancelAfter(ms);
            CancellationToken token = cts.Token;
            interrupter.Resume();
            token.Register(() => interrupter.Interrupt());

            int eval = 0;
            int currentDepth = 0;
            Move bestMove = new Move();
            bool gameWasFinished = false;

            try
            {
                ParallelOptions parallelOptions = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                };

                Parallel.For(1, 10, parallelOptions, depth =>
                {
                    try
                    {
                        //Console.WriteLine($"Task started for depth {depth}");
                        var result = AlphaBeta(position, side, depth);
                        lock (cts)
                        {
                            if (currentDepth < depth && !token.IsCancellationRequested)
                            {
                                currentDepth = depth;
                                eval = result.Item1;
                                gameWasFinished = result.Item2;
                                bestMove = result.Item3;
                                Console.WriteLine($"Base depth: {depth,2}. Evaluation: {(float)eval / 100.0f,6} pawns. Time: {(DateTime.Now.Ticks - start) / 10000,10} ms.");
                            }
                            if(gameWasFinished)
                                cts.Cancel();
                        }
                        
                    }
                    catch (OperationCanceledException)
                    {
                        Console.WriteLine($"Task for depth {depth} canceled.");
                    }
                });
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Search interrupted due to time limit.");
            }
            finally
            {
                cts.Dispose();
            }

            Console.WriteLine("Parallel search finished.");
            return Task<Move>.FromResult(bestMove);
        }

        private static Tuple<int, bool, Move> AlphaBeta(in Position.Position position, Side side, int depthLeft)
            => side == Side.White ? AlphaBetaMax(position, int.MinValue, int.MaxValue, depthLeft) :
                                    AlphaBetaMin(position, int.MinValue, int.MaxValue, depthLeft);

        private static Tuple<int, bool, Move> AlphaBetaMin(in Position.Position position, int alpha, int beta, int depthLeft, int currentDepth = 0)
        {
            if (SearchInterrupter.Instance.Interrupted)
                return new Tuple<int, bool, Move>(0, false, new Move());

            if (depthLeft == 0)
                return new Tuple<int, bool, Move>(AlphaBetaMinOnlyCaptures(position, alpha, beta), false, new Move());

            if (position.FiftyMovesRuleDraw || position.ThreefoldRepetitionDraw)
                return new Tuple<int, bool, Move>(0, true, new Move());

            MoveList moves = LegalMoveGen.Generate(position, Side.Black);
            moves = MoveSorter.Sort(position.Pieces, moves);
            Move bestMove = null;
            byte bestMoveIndex = 0;
            bool gameWasFinishedOnBestMove = false;

            byte tableResult = TranspositionTable.Instance.GetBestMoveIndex(position.Hash);
            if (tableResult < moves.GetSize())
                moves.Swap(0, tableResult);

            bool check = PseudoLegalMoveMaskGen.InDanger(position.Pieces, BOp.Bsf(position.Pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.King)), Side.Black);

            if (moves.GetSize() == 0)
            {
                if (check)
                    return new Tuple<int, bool, Move>(int.MaxValue - depthLeft, true, new Move());
                return new Tuple<int, bool, Move>(0, true, new Move());
            }

            for (byte i = 0; i < moves.GetSize(); i++)
            {
                Move move = moves[i];

                Position.Position copy = position.Copy();
                copy.Move(move);
                var result = AlphaBetaMax(copy, alpha, beta, depthLeft - (check ? 0 : 1), currentDepth + 1);
                int evaluation = result.Item1;
                bool gameWasFinished = result.Item2;

                if (evaluation <= alpha)
                {
                    TranspositionTable.Instance.AddEntry(position.Hash, currentDepth, bestMoveIndex);
                    return new Tuple<int, bool, Move>(alpha, gameWasFinishedOnBestMove, bestMove);
                }

                if (evaluation < beta)
                {
                    bestMove = move;
                    bestMoveIndex = i;
                    gameWasFinishedOnBestMove = gameWasFinished;
                    beta = evaluation;
                }
            }

            TranspositionTable.Instance.AddEntry(position.Hash, currentDepth, bestMoveIndex);
            return new Tuple<int, bool, Move>(beta, gameWasFinishedOnBestMove, bestMove);
        }

        private static Tuple<int, bool, Move> AlphaBetaMax(in Position.Position position, int alpha, int beta, int depthLeft, int currentDepth = 0)
        {
            if (SearchInterrupter.Instance.Interrupted)
                return new Tuple<int, bool, Move>(0, false, new Move());

            if (depthLeft == 0)
                return new Tuple<int, bool, Move>(AlphaBetaMaxOnlyCaptures(position, alpha, beta), false, new Move());

            if (position.FiftyMovesRuleDraw || position.ThreefoldRepetitionDraw)
                return new Tuple<int, bool, Move>(0, true, new Move());

            MoveList moves = LegalMoveGen.Generate(position, Side.White);
            moves = MoveSorter.Sort(position.Pieces, moves);
            Move bestMove = null;
            byte bestMoveIndex = 0;
            bool gameWasFinishedOnBestMove = false;

            byte tableResult = TranspositionTable.Instance.GetBestMoveIndex(position.Hash);
            if (tableResult < moves.GetSize())
                moves.Swap(0, tableResult);

            bool check = PseudoLegalMoveMaskGen.InDanger(position.Pieces, BOp.Bsf(position.Pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.King)), Side.White);

            if (moves.GetSize() == 0)
            {
                if (check)
                    return new Tuple<int, bool, Move>(int.MinValue + depthLeft, true, new Move());
                return new Tuple<int, bool, Move>(0, true, new Move());
            }

            for (byte i = 0; i < moves.GetSize(); i++)
            {
                Move move = moves[i];

                Position.Position copy = position.Copy();
                copy.Move(move);
                var result = AlphaBetaMin(copy, alpha, beta, depthLeft - (check ? 0 : 1), currentDepth + 1);
                int evaluation = result.Item1;
                bool gameWasFinished = result.Item2;

                if (evaluation >= beta)
                {
                    TranspositionTable.Instance.AddEntry(position.Hash, currentDepth, bestMoveIndex);
                    return new Tuple<int, bool, Move>(beta, gameWasFinishedOnBestMove, bestMove);
                }

                if (evaluation > alpha)
                {
                    bestMove = move;
                    bestMoveIndex = i;
                    gameWasFinishedOnBestMove = gameWasFinished;
                    alpha = evaluation;
                }
            }

            TranspositionTable.Instance.AddEntry(position.Hash, currentDepth, bestMoveIndex);
            return new Tuple<int, bool, Move>(alpha, gameWasFinishedOnBestMove, bestMove);
        }

        private static int AlphaBetaMinOnlyCaptures(in Position.Position position, int alpha, int beta)
        {
            if(SearchInterrupter.Instance.Interrupted)
                return 0;

            int evaluation = StaticEvaluator.Evaluate(position.Pieces);

            if (evaluation <= alpha)
                return alpha;

            if(evaluation < beta)
                beta = evaluation;

            MoveList moves = LegalMoveGen.Generate(position, Side.Black, true);
            moves = MoveSorter.Sort(position.Pieces, moves);

            for (byte i = 0; i < moves.GetSize(); i++)
            {
                Move move = moves[i];

                Position.Position copy = position.Copy();
                copy.Move(move);
                evaluation = AlphaBetaMaxOnlyCaptures(copy, alpha, beta);

                if(evaluation <= alpha)
                    return alpha;
                if (evaluation < beta)
                    beta = evaluation;
            }

            return beta;
        }

        private static int AlphaBetaMaxOnlyCaptures(in Position.Position position, int alpha, int beta)
        {
            if (SearchInterrupter.Instance.Interrupted)
                return 0;

            int evaluation = StaticEvaluator.Evaluate(position.Pieces);

            if (evaluation >= beta)
                return beta;

            if (evaluation > alpha)
                alpha = evaluation;

            MoveList moves = LegalMoveGen.Generate(position, Side.White, true);
            moves = MoveSorter.Sort(position.Pieces, moves);

            for (byte i = 0; i < moves.GetSize(); i++)
            {
                Move move = moves[i];

                Position.Position copy = position.Copy();
                copy.Move(move);
                evaluation = AlphaBetaMaxOnlyCaptures(copy, alpha, beta);

                if (evaluation >= beta)
                    return beta;
                if (evaluation > alpha)
                    alpha = evaluation;
            }

            return alpha;
        }
    }
}
