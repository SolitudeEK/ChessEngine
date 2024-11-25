using ChessEngine.MoveGeneration;
using ChessEngine.Position;

namespace ChessEngine.AI
{
    public static class StaticEvaluator
    {
        public static int Evaluate(Pieces pieces, bool showInfo = false)
        {
            int materialEvaluation = MaterialEvaluation(pieces);
            int mobilityEvaluation = MobilityEvaluation(pieces);
            int doublePawnEvaluation = DoublePawn(pieces);
            int connectedPawnEvaluation = ConnectedPawn(pieces);
            int pawnPromotionEvaluation = PawnPromotion(pieces);
            int kingSafetyEvaluation = KingSafetyEvaluation(pieces);
            int endgameEvaluation = EndgameEvaluation(pieces, materialEvaluation >= 0);

            int evaluation = materialEvaluation + mobilityEvaluation + doublePawnEvaluation + connectedPawnEvaluation
                + pawnPromotionEvaluation + kingSafetyEvaluation + endgameEvaluation;

            if (showInfo)
            {
                Console.WriteLine($"Material: {(float)materialEvaluation / 100f} pawns.");
                Console.WriteLine($"Mobility: {(float)mobilityEvaluation / 100f} pawns.");
                Console.WriteLine($"Double pawn: {(float)doublePawnEvaluation / 100f} pawns.");
                Console.WriteLine($"Connected pawn: {(float)connectedPawnEvaluation / 100f} pawns.");
                Console.WriteLine($"Pawn promotion: {(float)pawnPromotionEvaluation / 100f} pawns.");
                Console.WriteLine($"King safety: {(float)kingSafetyEvaluation / 100f} pawns.");
                Console.WriteLine($"Endgame: {(float)endgameEvaluation / 100f} pawns.");
                Console.WriteLine($"Total: {(float)evaluation / 100f} pawns.");
            }

            return evaluation;
        }

        private static int MaterialEvaluation(Pieces pieces)
        {
            int material = 0;

            material += Material.Pawn * (BOp.Count1(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn)) -
                                         BOp.Count1(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn)));
            material += Material.Knight * (BOp.Count1(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Knight)) -
                                           BOp.Count1(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Knight)));
            material += Material.Bishop * (BOp.Count1(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Bishop)) -
                                           BOp.Count1(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Bishop)));
            material += Material.Rook * (BOp.Count1(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Rook)) -
                                         BOp.Count1(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Rook)));
            material += Material.Queen * (BOp.Count1(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Queen)) -
                                          BOp.Count1(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Queen)));

            return material;
        }

        private static int MobilityEvaluation(in Pieces pieces)
        {
            int mobility = 0;

            ulong[,] masks = pieces.GetPieceBitboards();

            ulong safeForBlack = ~(PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(pieces, (byte)Side.White, true) |
                                  PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(pieces, (byte)Side.White, true));
            ulong safeForWhite = ~(PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(pieces, (byte)Side.Black, true) |
                                  PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(pieces, (byte)Side.Black, true));

            int knightMoves = 0;
            int bishopMoves = 0;
            int rookMoves = 0;
            int queenMoves = 0;

            while (masks[(byte)Side.White, (byte) Piece.Knight] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Knight]);
                masks[(byte)Side.White, (byte)Piece.Knight] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Knight], index);
                knightMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateKnightMask(pieces, index, (byte)Side.White, false) & safeForWhite);
            }
            while (masks[(byte)Side.White, (byte)Piece.Bishop] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Bishop]);
                masks[(byte)Side.White, (byte)Piece.Bishop] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Bishop], index);
                bishopMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateBishopMask(pieces, index, (byte)Side.White, false) & safeForWhite);
            }
            while (masks[(byte)Side.White, (byte)Piece.Rook] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Rook]);
                masks[(byte)Side.White, (byte)Piece.Rook] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Rook], index);
                rookMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateRookMask(pieces, index, (byte)Side.White, false) & safeForWhite);
            }
            while (masks[(byte)Side.White, (byte)Piece.Queen] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Queen]);
                masks[(byte)Side.White, (byte)Piece.Queen] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Queen], index);
                queenMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateQueenMask(pieces, index, (byte)Side.White, false) & safeForWhite);
            }

            while (masks[(byte)Side.Black, (byte)Piece.Knight] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Knight]);
                masks[(byte)Side.Black, (byte)Piece.Knight] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Knight], index);
                knightMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateKnightMask(pieces, index, Side.Black, false) & safeForBlack);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Bishop] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Bishop]);
                masks[(byte)Side.Black, (byte)Piece.Bishop] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Bishop], index);
                bishopMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateBishopMask(pieces, index, (byte)Side.Black, false) & safeForBlack);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Rook] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Rook]);
                masks[(byte)Side.Black, (byte)Piece.Rook] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Rook], index);
                rookMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateRookMask(pieces, index, (byte)Side.Black, false) & safeForBlack);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Queen] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Queen]);
                masks[(byte)Side.Black, (byte)Piece.Queen] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Queen], index);
                queenMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateQueenMask(pieces, index, (byte)Side.Black, false) & safeForBlack);
            }

            mobility += Mobility.Knight * knightMoves;
            mobility += Mobility.Bishop * bishopMoves;
            mobility += Mobility.Rook * rookMoves;
            mobility += Mobility.Queen * queenMoves;

            return mobility;
        }

        private static int DoublePawn(Pieces pieces)
        {
            int doublePawnsNumber = 0;

            for (byte x = 0; x < 8; x++)
            {
                int whitePawns = BOp.Count1(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn) & BColumns.Columns[x]);
                int blackPawns = BOp.Count1(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn) & BColumns.Columns[x]);

                doublePawnsNumber += Math.Max(0, whitePawns - 1);
                doublePawnsNumber -= Math.Max(0, blackPawns - 1);
            }

            return PawnStructure.DoublePawn * doublePawnsNumber;
        }

        private static int ConnectedPawn(Pieces pieces)
        {
            int connectedPawnsNumber = 0;

            ulong whiteCaptures = PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(pieces, (byte)Side.White, true) |
                                  PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(pieces, (byte)Side.White, true);
            ulong blackCaptures = PseudoLegalMoveMaskGen.GeneratePawnsLeftCapturesMask(pieces, (byte)Side.Black, true) |
                      PseudoLegalMoveMaskGen.GeneratePawnsRightCapturesMask(pieces, (byte)Side.Black, true);

            connectedPawnsNumber += BOp.Count1(whiteCaptures & pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn));
            connectedPawnsNumber -= BOp.Count1(blackCaptures & pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn));

            return PawnStructure.ConnectedPawn * connectedPawnsNumber;
        }

        private static int PawnPromotion(in Pieces pieces)
        {
            int pawnPromotion = 0;

            ulong whitePawns = pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn);
            ulong blackPawns = pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn);

            while(whitePawns != 0)
            {
                byte index = BOp.Bsf(whitePawns);
                whitePawns = BOp.Set0(whitePawns, index);
                if ((PassedPawnMasks.WhitePassedPawnMasks[index] & pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.Pawn)) != 0)
                    pawnPromotion += PawnStructure.DefaultPawnPromotion[index / 8];
                else
                    pawnPromotion += PawnStructure.PassedPawnPromotion[index / 8];
            }

            while (blackPawns != 0)
            {
                byte index = BOp.Bsf(blackPawns);
                blackPawns = BOp.Set0(blackPawns, index);
                if ((PassedPawnMasks.BlackPassedPawnMasks[index] & pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.Pawn)) != 0)
                    pawnPromotion -= PawnStructure.DefaultPawnPromotion[index / 8];
                else
                    pawnPromotion -= PawnStructure.PassedPawnPromotion[index / 8];
            }

            return pawnPromotion;
        }

        private static int KingSafetyEvaluation(in Pieces pieces)
        {
            int kingSafety = 0;

            if(BOp.Count1(pieces.GetAllBitboard()) <= Endgame.MaximumPiecesForEndgame)
                return kingSafety;

            byte whiteKingP = BOp.Bsf(pieces.GetPieceBitboard((byte)Side.White, (byte)Piece.King));
            byte blackKingP = BOp.Bsf(pieces.GetPieceBitboard((byte)Side.Black, (byte)Piece.King));

            ulong whiteKingArea = KingMasks.Masks[whiteKingP];
            ulong blackKingArea = KingMasks.Masks[blackKingP];

            ulong[,] masks = pieces.GetPieceBitboards();//.Copy(); // Potentially needs to be a copy
            int knightMoves = 0;
            int bishopMoves = 0;
            int rookMoves = 0;
            int queenMoves = 0;

            while (masks[(byte)Side.White, (byte)Piece.Knight] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Knight]);
                masks[(byte)Side.White, (byte)Piece.Knight] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Knight], index);
                knightMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateKnightMask(pieces, index, (byte)Side.White, false) & blackKingArea);
            }
            while (masks[(byte)Side.White, (byte)Piece.Bishop] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Bishop]);
                masks[(byte)Side.White, (byte)Piece.Bishop] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Bishop], index);
                bishopMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateBishopMask(pieces, index, (byte)Side.White, false) & blackKingArea);
            }
            while (masks[(byte)Side.White, (byte)Piece.Rook] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Rook]);
                masks[(byte)Side.White, (byte)Piece.Rook] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Rook], index);
                rookMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateRookMask(pieces, index, (byte)Side.White, false) & blackKingArea);
            }
            while (masks[(byte)Side.White, (byte)Piece.Queen] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.White, (byte)Piece.Queen]);
                masks[(byte)Side.White, (byte)Piece.Queen] = BOp.Set0(masks[(byte)Side.White, (byte)Piece.Queen], index);
                queenMoves += BOp.Count1(PseudoLegalMoveMaskGen.GenerateQueenMask(pieces, index, (byte)Side.White, false) & blackKingArea);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Knight] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Knight]);
                masks[(byte)Side.Black, (byte)Piece.Knight] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Knight], index);
                knightMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateKnightMask(pieces, index, Side.Black, false) & whiteKingArea);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Bishop] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Bishop]);
                masks[(byte)Side.Black, (byte)Piece.Bishop] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Bishop], index);
                bishopMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateBishopMask(pieces, index, (byte)Side.Black, false) & whiteKingArea);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Rook] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Rook]);
                masks[(byte)Side.Black, (byte)Piece.Rook] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Rook], index);
                rookMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateRookMask(pieces, index, (byte)Side.Black, false) & whiteKingArea);
            }
            while (masks[(byte)Side.Black, (byte)Piece.Queen] != 0)
            {
                byte index = BOp.Bsf(masks[(byte)Side.Black, (byte)Piece.Queen]);
                masks[(byte)Side.Black, (byte)Piece.Queen] = BOp.Set0(masks[(byte)Side.Black, (byte)Piece.Queen], index);
                queenMoves -= BOp.Count1(PseudoLegalMoveMaskGen.GenerateQueenMask(pieces, index, (byte)Side.Black, false) & whiteKingArea);
            }
            kingSafety += KingSafety.Knight * knightMoves;
            kingSafety += KingSafety.Bishop * bishopMoves;
            kingSafety += KingSafety.Rook * rookMoves;
            kingSafety += KingSafety.Queen * queenMoves;

            return kingSafety;
        }

        private static int EndgameEvaluation(Pieces pieces, bool whiteStronger)
        {
            int endgame = 0;

            if(BOp.Count1(pieces.GetAllBitboard()) > Endgame.MaximumPiecesForEndgame)
                return endgame;

            Side attackerSide = Side.Black;
            Side defenderSide = Side.White;

            if (whiteStronger)
            {
                attackerSide = Side.White;
                defenderSide = Side.Black;
            }

            byte attackerKingP = BOp.Bsf(pieces.GetPieceBitboard((byte)attackerSide, (byte)Piece.King));
            sbyte attackerKingX = (sbyte)(attackerKingP % 8);
            sbyte attackerKingY = (sbyte)(attackerKingP / 8);

            byte defenderKingP = BOp.Bsf(pieces.GetPieceBitboard((byte)defenderSide, (byte)Piece.King));
            sbyte defenderKingX = (sbyte)(defenderKingP % 8);
            sbyte defenderKingY = (sbyte)(defenderKingP / 8);

            endgame += Endgame.ProximityKings * (16 - Math.Abs(attackerKingX - defenderKingX) - Math.Abs(attackerKingY - defenderKingY));
            endgame += Endgame.DistanceWeakKingMiddle * (Math.Abs(defenderKingX - 3) + Math.Abs(defenderKingY - 4));

            return whiteStronger ? endgame : -endgame;
        }
    }
}
