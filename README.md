# Chess Engine Project

This repository contains a robust chess engine designed for high-performance move evaluation, game state analysis, and AI-based gameplay. The project incorporates advanced techniques like **Zobrist hashing**, **alpha-beta pruning**, and **transposition tables** for efficient decision-making.

## Features

- **Move Generation**: Generates all legal moves for a given board state using bitboards.
- **Evaluation Function**: Static evaluation of positions based on material and other heuristics.
- **Alpha-Beta Pruning**: Optimized tree search with depth-first strategy.
- **Zobrist Hashing**: Efficient board state representation and lookup.
- **Transposition Table**: Avoids redundant calculations by caching previously evaluated positions.
- **Threefold Repetition Detection**: Ensures draw detection per chess rules.
- **Multithreading**: Supports parallelized move evaluation for improved performance.

---

## Code Structure
- **AI**: Contains the implementation of the Alpha-Beta pruning algorithm.
- **Position**: Represents the chessboard state.
- **Move**: Defines move generation and sorting logic.
- **TranspositionTable**: Manages cached positions for faster lookups.
- **RepetitionHistory**: Tracks board state repetitions for draw detection.
- **SearchInterrupter**: Provides interrupt/resume functionality for the search process.
- **ZobristHash**: Implements hashing for board states.

---

## Usage

### Key Classes and Methods

1. **Get the Best Move**
   ```csharp
   Move bestMove = AI.GetBestMove(position, side, timeLimit);
   ```

2. **Generate legal moves**
	```csharp
	MoveList moves = LegalMoveGen.Generate(position, Side.White);
	```

3. **Create a Board Position**
	```csharp
	string shortFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR"
	Position position = new Position(shortFen, enPassantIndex, wlCastling, wsCastling, blCastling, bsCastling, moveCounter);
	```

---   

## Acknowledgments
- Inspired by popular chess engines and algorithms like Stockfish and Minimax.
- Based on the principles of game tree search and board evaluation.