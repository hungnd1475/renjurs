using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using AIAssignment2.GameLogic.Renjus;
using AIAssignment2.Foundations;

namespace AIAssignment2.GameLogic.Programs
{
    public class MainProgram : IProgram
    {
        private readonly IRenju renju;
        private readonly INetworkMessager messager;

        public MainProgram(IRenju renju, INetworkMessager messager)
        {
            this.renju = renju;
            this.messager = messager;
        }

        public void Run()
        {
            Move ourMove, opponentMove;
            GameResult gameResult = GameResult.Draw;
            bool moveGood = true;

            while (moveGood && messager.ReadMove(out opponentMove, out gameResult))
            {
                Console.WriteLine("Opponent move: ({0} {1}).", opponentMove.X + 1, opponentMove.Y + 1);
                ourMove = renju.GetNextMove(opponentMove);
                Console.WriteLine("Sent move ({0}, {1}) to server.", ourMove.X + 1, ourMove.Y + 1);
#if DEBUG
                renju.PrintBoard(false);
#endif
                //Console.WriteLine();
                moveGood = messager.SendMove(ourMove);
            }

            if (!moveGood || gameResult == GameResult.Lose)
            {
                Console.WriteLine("WE LOST :(");
            }
            else if (gameResult == GameResult.Win)
            {
                Console.WriteLine("WE WON :)");
            }
            else if (gameResult == GameResult.Draw)
            {
                Console.WriteLine("DRAW GAME :|");
            }

            messager.Disconnect();
        }
    }
}
