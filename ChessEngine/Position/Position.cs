using ChessEngine.Position.Hashing;
using System.Text;
using static ChessEngine.Position.Move;

namespace ChessEngine.Position
{
    public class Position
    {
        private Pieces _pieces;
        private byte _enPassant;
        private bool _wlCastling, _wsCastling, _blCastling, _bsCastling;
        private float _moveCounter;
        private ZobristHash _hash;
        private float _fiftyMovesCounter;
        private RepetitionHistory _repetitionHistory;

        public static readonly byte None = 255;

        public Position(string shortFen, byte enPassant, bool wlCastling, bool wsCastling, bool blCastling, bool bsCastling, float moveCounter)
        {
            _pieces = new Pieces(shortFen);
            _enPassant = enPassant;

            _wlCastling = wlCastling;
            _wsCastling = wsCastling;
            _blCastling = blCastling;
            _bsCastling = bsCastling;

            _moveCounter = moveCounter;
            _hash = new ZobristHash(_pieces, BlackToMove , wlCastling, wsCastling, blCastling, bsCastling);
            _repetitionHistory = new RepetitionHistory();
            _repetitionHistory.AddPosition(_hash.Value);
            _fiftyMovesCounter = 0;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_pieces.ToString());
            sb.AppendLine($"En passant: {_enPassant}");
            sb.AppendLine($"White long castling: {_wlCastling}");
            sb.AppendLine($"White short castling: {_wsCastling}");
            sb.AppendLine($"Black long castling: {_blCastling}");
            sb.AppendLine($"Black short castling: {_bsCastling}");
            sb.AppendLine($"Move counter: {_moveCounter}");
            sb.AppendLine($"Fifty moves counter: {_fiftyMovesCounter}");
            sb.AppendLine($"Threefold repetition counter: {_repetitionHistory.GetRepetitionNumber(_hash.Value)}");

            return sb.ToString();
        }

        public void Move(Move move)
        {
            RemovePiece(move.From, move.AttackerType, move.AttackerSide);
            AddPiece(move.To, move.AttackerType, move.AttackerSide);
            if((byte)move.DefenderType != ChessEngine.Position.Move.None)
                RemovePiece(move.To, move.DefenderType, move.DefenderSide);

            switch (move.Flag)
            {
                case FLAG.Default: break;
                case FLAG.PawnLongMove: ChangeEnPassant((byte)((move.From + move.To) / 2)); break;
                case FLAG.EnPassantCapture:
                    if (move.AttackerSide == Side.White)
                        RemovePiece((byte)(move.To - 8), Piece.Pawn, Side.Black);
                    else
                        RemovePiece((byte)(move.To + 8), Piece.Pawn, Side.White);
                    break;
                case FLAG.WlCastling:
                    RemovePiece(0, Piece.Rook, Side.White);
                    AddPiece(3, Piece.Rook, Side.White);
                    break;
                case FLAG.WsCastling:
                    RemovePiece(7, Piece.Rook, Side.White);
                    AddPiece(5, Piece.Rook, Side.White);
                    break;
                case FLAG.BlCastling:
                    RemovePiece(56, Piece.Rook, Side.Black);
                    AddPiece(59, Piece.Rook, Side.Black);
                    break;
                case FLAG.BsCastling:
                    RemovePiece(63, Piece.Rook, Side.Black);
                    AddPiece(61, Piece.Rook, Side.Black);
                    break;
                case FLAG.PromoteToBishop:
                    RemovePiece(move.To, Piece.Pawn, move.AttackerSide);
                    AddPiece(move.To, Piece.Bishop, move.AttackerSide);
                    break;
                case FLAG.PromoteToKnight:
                    RemovePiece(move.To, Piece.Pawn, move.AttackerSide);
                    AddPiece(move.To, Piece.Knight, move.AttackerSide);
                    break;
                case FLAG.PromoteToQueen:
                    RemovePiece(move.To, Piece.Pawn, move.AttackerSide);
                    AddPiece(move.To, Piece.Queen, move.AttackerSide);
                    break;
                case FLAG.PromoteToRook:
                    RemovePiece(move.To, Piece.Pawn, move.AttackerSide);
                    AddPiece(move.To, Piece.Rook, move.AttackerSide);
                    break;
            }

            _pieces.UpdateBitboards();

            if (move.Flag != FLAG.PawnLongMove) ChangeEnPassant(None);

            switch (move.From) 
            {
                case 0: RemoveWLCastling(); break;
                case 4: RemoveWLCastling(); RemoveWSCastling(); break;
                case 7: RemoveWSCastling(); break;
                case 56: RemoveBLCastling(); break;
                case 60: RemoveBLCastling(); RemoveBSCastling(); break;
                case 63: RemoveBSCastling(); break;
            }

            UpdateMoveCounter();

            Update50MoveCounter(move.AttackerType == Piece.Pawn || (byte)move.DefenderType != ChessEngine.Position.Move.None);

            if(move.AttackerType == Piece.Pawn || (byte)move.DefenderType != ChessEngine.Position.Move.None)
                _repetitionHistory.Clear();
            _repetitionHistory.AddPosition(_hash.Value);
        }

        public Pieces Pieces => _pieces;

        public byte EnPassant => _enPassant;

        public bool WLCastling => _wlCastling;

        public bool WSCastling => _wsCastling;

        public bool BLCastling => _blCastling;

        public bool BSCastling => _bsCastling;

        public bool WhiteToMove => !BlackToMove;

        private bool BlackToMove
            => _moveCounter - Math.Floor(_moveCounter) > 0.0001;

        public ZobristHash Hash => _hash;

        public bool FiftyMovesRuleDraw => _fiftyMovesCounter == 50;

        public bool ThreefoldRepetitionDraw => _repetitionHistory.GetRepetitionNumber(_hash.Value) == 3;

        private void AddPiece(byte square, Piece type, Side side)
        {
            if (!BOp.GetBit(_pieces.GetPieceBitboard((byte)side, (byte)type), square))
            {
                _pieces.SetPieceBitboard((byte)side, (byte)type, BOp.Set1(_pieces.GetPieceBitboard((byte)side, (byte)type), square));
                _hash.InvertPiece(square, (byte)type, (byte)side);
            }
        }

        private void RemovePiece(byte square, Piece type, Side side)
        {
            if (BOp.GetBit(_pieces.GetPieceBitboard((byte)side, (byte)type), square))
            {
                _pieces.SetPieceBitboard((byte)side, (byte)type, BOp.Set0(_pieces.GetPieceBitboard((byte)side, (byte)type), square));
                _hash.InvertPiece(square, (byte)type, (byte)side);
            }
        }

        private void ChangeEnPassant(byte enPassant)
            => _enPassant = enPassant;

        private void RemoveWLCastling()
        {
            if (_wlCastling)
            {
                _wlCastling = false;
                _hash.InvertWLCastling();
            }
        }

        private void RemoveWSCastling()
        {
            if (_wsCastling)
            {
                _wsCastling = false;
                _hash.InvertWSCastling();
            }
        }

        private void RemoveBLCastling()
        {
            if (_blCastling)
            {
                _blCastling = false;
                _hash.InvertBLCastling();
            }
        }

        private void RemoveBSCastling()
        {
            if (_bsCastling)
            {
                _bsCastling = false;
                _hash.InvertBSCastling();
            }
        }

        private void UpdateMoveCounter()
            => _moveCounter += 0.5f;

        private void Update50MoveCounter(bool breakEvent)
            => _fiftyMovesCounter = breakEvent ? 0 : _fiftyMovesCounter + 0.5f;

    }
}
