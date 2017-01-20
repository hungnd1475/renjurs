using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIAssignment2.GameLogic.Programs
{
    using Foundations;
    public class TestProgram : IProgram
    {
        private readonly IRenju renju;
        private readonly bool zeroBase;

        public TestProgram(IRenju renju, bool zeroBase)
        {
            this.renju = renju;
            this.zeroBase = zeroBase;
        }

        public void Run()
        {
            Move ourMove, oppMove;
            int moveCount = 0;
            while (true)
            {
                Console.Write("{0}. Opponent's move: ", moveCount++);
                oppMove = getMoveFromString(Console.ReadLine());
                ourMove = renju.GetNextMove(oppMove);
                int x = zeroBase ? ourMove.X : ourMove.X + 1;
                int y = zeroBase ? ourMove.Y : ourMove.Y + 1;
                Console.WriteLine("{2}. Our move: {0} {1}", x, y, moveCount++);
                renju.PrintBoard(zeroBase);
                Console.WriteLine();
            }
        }

        private Move getMoveFromString(string input)
        {
            input = input.TrimEnd(' ');
            var result = input.Split(' ');
            var x = int.Parse(result[0]);
            var y = int.Parse(result[1]);
            return zeroBase ? new Move(x, y) : new Move(x - 1, y - 1);
        }
    }
}
