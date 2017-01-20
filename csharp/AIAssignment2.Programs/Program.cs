using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Text.RegularExpressions;

namespace AIAssignment2.Programs
{
    using Foundations;
    using GameLogic.Renjus;
    using GameLogic.Programs;
    using GameLogic.Renjus.Threats;

    class Program
    {
        static void Main(string[] args)
        {
            //testRenju();
            seriousBusiness();
        }

        private static void testRenju()
        {
            var board = new GameBoard(13);
            var black = GameBoard.Black;
            var white = GameBoard.White;

            board[10, 5] = black;
            board[12, 5] = white;
            board[11, 6] = black;
            board[9, 4] = black;
            board[12, 7] = white;
            board[8, 3] = black;
            board[12, 6] = white;
            //board[2, 8] = black;
            //board[7, 2] = black;
            //board[2, 6] = black;

            Console.WriteLine(board);

            foreach (var l in board.GetLinesOf(black).Lines)
            {
                Console.WriteLine(l);
                foreach (var s in l.NextSquares.Keys)
                {
                    Console.Write(s);
                    Console.Write(" ");
                }
                Console.WriteLine();
            }

            Console.WriteLine(board.ContainsDisallowMoves());

            Console.Read();
        }

        private static void seriousBusiness()
        {
            do
            {
                Console.Clear();

                var port = 1235;
                var ip = "127.0.0.1";

                if (!askYesNoQuestion("Connect to local host at port 1235?"))
                {
                    Console.Write("Specify server port: ");
                    port = int.Parse(Console.ReadLine());
                    Console.Write("Specify server IP: ");
                    ip = Console.ReadLine();
                }

                //var checkDisallowMoves = askYesNoQuestion("Check disallow moves?");

                Console.WriteLine("Connecting to address {0}:{1}", ip, port);

                Console.Write("Client name: ");
                string name = Console.ReadLine();

                var messager = new NetworkMessager(port, ip);
                while (!messager.SendName(name))
                {
                    Console.Write("Error. Please enter another name: ");
                    name = Console.ReadLine();
                }

                try
                {
                    int size; float timeOut;
                    messager.ReadInfo(out size, out timeOut);
                    Console.WriteLine(string.Format("Size: {0}\nTimeout: {1}s", size, timeOut));
                    var renju = new ThreatRenju(size, timeOut);
                    var program = new MainProgram(renju, messager);
                    program.Run();
                }
                finally
                {
                    messager.Disconnect();
                }
            }
            while (askYesNoQuestion("Continue?"));
        }

        private static bool askYesNoQuestion(string question)
        {
            string ans = "n";

            do
            {
                Console.Write("{0} (Y/N): ", question);
                ans = Console.ReadLine().ToLower();
            }
            while (ans != "n" && ans != "y");

            return ans == "y";
        }
    }
}
