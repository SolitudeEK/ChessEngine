namespace ChessEngine.UI
{
    public class ConsoleUI
    {
        public void StartGame()
        {
            bool correctInput = false;
            bool whiteSelected = true;
            Console.WriteLine("Select your side? [b/w]");

            while (!correctInput)
            {
                var response = Console.ReadLine();
                if (response == "b")
                {
                    Console.WriteLine("You selected black side");
                    correctInput = true;
                    whiteSelected = true;
                }
                else if (response == "w")
                {
                    Console.WriteLine("You selected white side");
                    correctInput = true;
                    whiteSelected = false;
                }
                else
                    Console.WriteLine("Incorrect input");
            }


            while (true)
            {
                if (whiteSelected)
                {
                    Console.WriteLine("Input your move:");
                    var move = Console.ReadLine();

                }
            }
        }
    }
}
