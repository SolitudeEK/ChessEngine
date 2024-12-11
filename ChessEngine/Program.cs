using ChessEngine.AI;
using ChessEngine.Position;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;


string f1 = "8/2p5/3p4/KP5r/1R3p1k/8/4P1P1/8";
string f2 = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR";
var p1 = new Position(f1,255, false, false, false, false, 0);

for(int i =0; i < 70; i++)
{
    var m1 = await AI.GetBestMove(p1, Side.White, 12000);

    p1.Move(m1);
    var m2 = await AI.GetBestMovePharallel(p1, Side.Black, 12000);

    p1.Move(m2);
}

//ConsoleUI input = new ConsoleUI();

//input.SetUpGame();