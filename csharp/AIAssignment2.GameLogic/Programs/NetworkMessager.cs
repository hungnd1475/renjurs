using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using AIAssignment2.GameLogic.Renjus;
using AIAssignment2.Foundations;
using System;

namespace AIAssignment2.GameLogic.Programs
{
    public class NetworkMessager : INetworkMessager
    {
        private readonly TcpClient client;
        private readonly StreamWriter writer;
        private readonly StreamReader reader;

        public NetworkMessager(int port, string address)
        {
            var serverAddress = new IPEndPoint(IPAddress.Parse(address), port);
            client = new TcpClient();
            client.Connect(serverAddress);
            writer = new StreamWriter(client.GetStream());
            reader = new StreamReader(client.GetStream());
        }

        public bool SendName(string name)
        {
            writer.WriteLine(string.Format("LOGIN {0}", name));
            writer.Flush();

            var result = reader.ReadLine();
            if (result.Contains("HELLO"))
                return true;
            else return false;
        }

        public void Disconnect()
        {
            client.Close();
        }

        public bool SendMove(Move move)
        {
            writer.WriteLine(string.Format("MOVE {0} {1}", move.X + 1, move.Y + 1));
            writer.Flush();

            var result = reader.ReadLine();
            if (result.Contains("ERROR")) return false;
            return true;
        }

        public bool ReadMove(out Move move, out GameResult gameResult)
        {
            move = new Move();
            gameResult = GameResult.Draw;

            var result = reader.ReadLine();
            if (result.Contains("MOVE"))
            {
                var temp = result.Split(' ');
                int x = int.Parse(temp[1]) - 1;
                int y = int.Parse(temp[2]) - 1;
                move = new Move(x, y);
                return true;
            }

            if (result.Contains("WIN"))
            {
                gameResult = GameResult.Win;
            }
            else if (result.Contains("LOSE"))
            {
                gameResult = GameResult.Lose;
            }
            return false;
        }

        public void ReadInfo(out int size, out float timeOut)
        {
            size = 0; timeOut = 0;
            var result = reader.ReadLine();
            if (result.Contains("INFO"))
            {
                var temp = result.Split(' ');
                size = int.Parse(temp[1]);
                timeOut = float.Parse(temp[2]);
            }
        }
    }
}
