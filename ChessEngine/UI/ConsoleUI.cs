using ChessEngine.MoveGeneration;
using ChessEngine.Position;

namespace ChessEngine.UI
{
    public class ConsoleUI
    {
        private Position.Position position = new Position.Position("rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR", 255, true, true, true, true, 0);
        private Side selectedSide = Side.White;
        public void SetUpGame()
        {
            bool correctInput = false;

            Console.WriteLine("Select your side? [b/w]");

            while (!correctInput)
            {
                var response = Console.ReadLine();
                if (response == "b")
                {
                    Console.WriteLine("You selected black side");
                    correctInput = true;
                    selectedSide = Side.Black;
                }
                else if (response == "w")
                {
                    Console.WriteLine("You selected white side");
                    correctInput = true;
                    selectedSide = Side.White;
                }
                else
                    Console.WriteLine("Incorrect input");
            }

            StartGame();
        }

        private async void StartGame() 
        { 
            Side currentSide = Side.White;

            while (true)
            {
                if (currentSide == selectedSide)
                {
                    Console.WriteLine("Input your move:");
                    var input = Console.ReadLine();

                    var move = input.Convert(position, currentSide);

                    Console.WriteLine(move);

                    if (!LegalMoveGen.IsLegal(position.Pieces, move))
                    {
                        Console.WriteLine("Incorrect move");
                        continue;
                    }

                    position.Move(move);
                    currentSide = Pieces.Inverse(currentSide);
                }
                else
                {
                    var move = await AI.AI.GetBestMovePharallel(position, currentSide, 10000);

                    Console.WriteLine($"Opponent move: {UCIParser.Convert(move)}");

                    position.Move(move);
                    currentSide = Pieces.Inverse(currentSide);
                }
            }
        }
    }
}
