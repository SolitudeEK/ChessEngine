using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTests.Utilities
{
    internal static class Converter
    {
        public static void ShowAsBoard(this ulong board)
        {
            for (int row = 7; row >= 0; row--)
            {
                for (int col = 0; col < 8; col++)
                {
                    int position = row * 8 + col;
                    bool isSet = (board & (1UL << position)) != 0;
                    Console.Write(isSet ? "* " : ". ");
                }
                Console.WriteLine();
            }
        }
    }
}
